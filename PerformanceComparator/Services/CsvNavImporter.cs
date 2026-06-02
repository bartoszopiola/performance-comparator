using System.Globalization;
using Microsoft.EntityFrameworkCore;
using PerformanceComparator.Data;
using PerformanceComparator.Models;

namespace PerformanceComparator.Services
{
    public class CsvNavImporter : ICsvNavImporter
    {
        private readonly ApplicationDbContext _context;

        // Accepted date formats (tried in order)
        private static readonly string[] DateFormats =
        [
            "yyyy-MM-dd",
            "dd.MM.yyyy"
        ];

        public CsvNavImporter(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ImportResult> ImportAsync(int fundId, Stream csvStream)
        {
            var result = new ImportResult();

            // ── Read all lines ─────────────────────────────────────────────────
            var lines = new List<string>();
            using (var reader = new StreamReader(csvStream))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) is not null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        lines.Add(line.Trim());
                }
            }

            if (lines.Count == 0)
            {
                result.Errors.Add("The file is empty.");
                return result;
            }

            // ── Detect format from header row ──────────────────────────────────
            var header = lines[0];
            var format = DetectFormat(header, out int dateColumn, out int valueColumn);

            if (format == CsvFormat.Unknown)
            {
                result.Errors.Add(
                    "Unrecognized header. Expected Stooq format " +
                    "(Data,Otwarcie,...,Zamkniecie,Wolumen) or simple format (date,value).");
                return result;
            }

            // ── Load existing dates for this fund (to skip duplicates) ─────────
            var existingDates = await _context.NavRecords
                .Where(n => n.FundId == fundId)
                .Select(n => n.Date)
                .ToListAsync();

            var existingSet = new HashSet<DateTime>(existingDates);

            // Track dates added within this same file (avoid in-file duplicates)
            var batchDates = new HashSet<DateTime>();
            var toAdd = new List<NavRecord>();

            // ── Parse data rows (skip header at index 0) ───────────────────────
            for (int i = 1; i < lines.Count; i++)
            {
                var raw = lines[i];
                var cells = raw.Split(',');

                if (cells.Length <= Math.Max(dateColumn, valueColumn))
                {
                    result.Errors.Add($"Row {i + 1}: not enough columns.");
                    continue;
                }

                var dateText = cells[dateColumn].Trim();
                var valueText = cells[valueColumn].Trim();

                if (!TryParseDate(dateText, out var date))
                {
                    result.Errors.Add($"Row {i + 1}: invalid date '{dateText}'.");
                    continue;
                }

                if (!TryParseDecimal(valueText, out var value))
                {
                    result.Errors.Add($"Row {i + 1}: invalid value '{valueText}'.");
                    continue;
                }

                // Skip if already in DB or already queued in this batch
                if (existingSet.Contains(date) || batchDates.Contains(date))
                {
                    result.Skipped++;
                    continue;
                }

                batchDates.Add(date);
                toAdd.Add(new NavRecord
                {
                    FundId = fundId,
                    Date = date,
                    Value = value
                });
            }

            // ── Validate at least some rows parsed ─────────────────────────────
            if (toAdd.Count == 0 && result.Skipped == 0)
            {
                if (result.Errors.Count == 0)
                    result.Errors.Add("No valid data rows found in the file.");
                return result;
            }

            // ── Persist in a transaction ───────────────────────────────────────
            if (toAdd.Count > 0)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.NavRecords.AddRange(toAdd);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    result.Added = toAdd.Count;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    result.Added = 0;
                    result.Errors.Add($"Database error, nothing was saved: {ex.Message}");
                }
            }

            return result;
        }

        // ── Format detection ────────────────────────────────────────────────────
        private enum CsvFormat { Unknown, Stooq, Simple }

        private static CsvFormat DetectFormat(string header, out int dateColumn, out int valueColumn)
        {
            dateColumn = -1;
            valueColumn = -1;

            var cols = header.Split(',')
                             .Select(c => c.Trim().ToLowerInvariant())
                             .ToArray();

            // Stooq: has "data" and "zamkniecie"
            int stooqDate = Array.IndexOf(cols, "data");
            int stooqClose = Array.IndexOf(cols, "zamkniecie");
            if (stooqDate >= 0 && stooqClose >= 0)
            {
                dateColumn = stooqDate;
                valueColumn = stooqClose;
                return CsvFormat.Stooq;
            }

            // Simple: "date" + ("value" or "nav")
            int simpleDate = Array.IndexOf(cols, "date");
            int simpleValue = Array.IndexOf(cols, "value");
            if (simpleValue < 0) simpleValue = Array.IndexOf(cols, "nav");

            if (simpleDate >= 0 && simpleValue >= 0)
            {
                dateColumn = simpleDate;
                valueColumn = simpleValue;
                return CsvFormat.Simple;
            }

            return CsvFormat.Unknown;
        }

        // ── Flexible date parsing ────────────────────────────────────────────────
        private static bool TryParseDate(string text, out DateTime date)
        {
            return DateTime.TryParseExact(
                text,
                DateFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date);
        }

        // ── Decimal parsing (comma OR dot separator) ─────────────────────────────
        private static bool TryParseDecimal(string text, out decimal value)
        {
            // Normalize: treat comma as decimal point, strip spaces (thousands)
            var normalized = text.Replace(" ", "").Replace(",", ".");

            return decimal.TryParse(
                normalized,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out value);
        }
    }
}