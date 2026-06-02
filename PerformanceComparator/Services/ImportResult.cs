namespace PerformanceComparator.Services
{
    public class ImportResult
    {
        public int Added { get; set; }
        public int Skipped { get; set; }
        public List<string> Errors { get; set; } = [];

        public bool HasErrors => Errors.Count > 0;
    }
}