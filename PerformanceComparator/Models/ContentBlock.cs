namespace PerformanceComparator.Models
{
    /// <summary>
    /// Admin-editable text snippet displayed on public pages.
    /// Keyed by a string identifier so views can look them up by name.
    /// Example keys: "home.intro", "compare.disclaimer"
    /// </summary>
    public class ContentBlock
    {
        public int Id { get; set; }

        /// <summary>Unique string key used in views to retrieve this block.</summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// HTML content edited by the admin.
        /// Render with @Html.Raw() ONLY in admin-facing views.
        /// Consider sanitisation before displaying on public pages.
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }
}