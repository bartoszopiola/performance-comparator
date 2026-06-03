/**
 * charts.js — reusable Chart.js renderers for Performance Comparator.
 * Requires Chart.js + the date-fns adapter (loaded in _Layout).
 *
 * seriesData shape:
 *   [ { label: "Fund A", points: [ { x: "2024-01-02", y: 100.0 }, ... ] }, ... ]
 */
(function (global) {
    'use strict';

    const PALETTE = ['#0d6efd', '#dc3545', '#198754', '#fd7e14', '#6f42c1'];
    const plFmt = new Intl.NumberFormat('pl-PL', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function hexToRgba(hex, alpha) {
        const r = parseInt(hex.slice(1, 3), 16);
        const g = parseInt(hex.slice(3, 5), 16);
        const b = parseInt(hex.slice(5, 7), 16);
        return `rgba(${r}, ${g}, ${b}, ${alpha})`;
    }

    function baseTimeScale(yTitle) {
        return {
            x: {
                type: 'time',
                time: { unit: 'month', tooltipFormat: 'yyyy-MM-dd' },
                title: { display: true, text: 'Data' }
            },
            y: {
                title: { display: true, text: yTitle }
            }
        };
    }

    function renderCumulativeChart(canvasId, seriesData) {
        const ctx = document.getElementById(canvasId);
        if (!ctx || !Array.isArray(seriesData) || seriesData.length === 0) return null;

        const datasets = seriesData.map(function (s, i) {
            const color = PALETTE[i % PALETTE.length];
            return {
                label: s.label,
                data: s.points.map(function (p) { return { x: p.x, y: p.y }; }),
                borderColor: color,
                backgroundColor: color,
                fill: false,
                tension: 0.1,
                pointRadius: 0,
                borderWidth: 2
            };
        });

        return new Chart(ctx, {
            type: 'line',
            data: { datasets: datasets },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                interaction: { mode: 'index', intersect: false },
                scales: baseTimeScale('Zwrot skumulowany (start = 100)'),
                plugins: {
                    legend: { position: 'top' },
                    tooltip: {
                        callbacks: {
                            label: function (item) {
                                return item.dataset.label + ': ' + plFmt.format(item.parsed.y);
                            }
                        }
                    }
                }
            }
        });
    }

    function renderDrawdownChart(canvasId, seriesData) {
        const ctx = document.getElementById(canvasId);
        if (!ctx || !Array.isArray(seriesData) || seriesData.length === 0) return null;

        const datasets = seriesData.map(function (s, i) {
            const color = PALETTE[i % PALETTE.length];
            return {
                label: s.label,
                // values are decimals (e.g. -0.18) → show as percentage
                data: s.points.map(function (p) { return { x: p.x, y: p.y * 100 }; }),
                borderColor: color,
                backgroundColor: hexToRgba(color, 0.2),
                fill: 'origin',
                tension: 0.1,
                pointRadius: 0,
                borderWidth: 1.5
            };
        });

        return new Chart(ctx, {
            type: 'line',
            data: { datasets: datasets },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                interaction: { mode: 'index', intersect: false },
                scales: (function () {
                    const s = baseTimeScale('Obsunięcie (%)');
                    s.y.max = 0;
                    return s;
                })(),
                plugins: {
                    legend: { position: 'top' },
                    tooltip: {
                        callbacks: {
                            label: function (item) {
                                return item.dataset.label + ': ' + plFmt.format(item.parsed.y) + '%';
                            }
                        }
                    }
                }
            }
        });
    }

    // Expose globally
    global.renderCumulativeChart = renderCumulativeChart;
    global.renderDrawdownChart = renderDrawdownChart;
})(window);