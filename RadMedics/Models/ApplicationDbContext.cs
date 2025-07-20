using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RadMedics.Models;  // Ensure you have this namespace to access your ApplicationUser

namespace RadMedics.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // Note the use of ApplicationUser here
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add DbSets for your other entities here
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<MailMessage> MailMessages { get; set; }
        // Other DbSets like CalendarEvents if required
    }
}
