using Microsoft.AspNetCore.Identity;

namespace PerformanceComparator.Models
{
    /// <summary>
    /// Extended Identity user. Empty for Phase 1 — add properties in Phase 2 if needed.
    /// Registered in Program.cs via AddDefaultIdentity&lt;ApplicationUser&gt;().
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // Phase 2: add DisplayName, preferences, etc. here
    }
}