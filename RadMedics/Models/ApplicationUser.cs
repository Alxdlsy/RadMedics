using Microsoft.AspNetCore.Identity;

namespace RadMedics.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }      // Add IsAdmin (if needed)
    }
}
