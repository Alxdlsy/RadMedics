using System.ComponentModel.DataAnnotations;

namespace RadMedics.Models
{
    public class StudentProfile
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Course { get; set; } = string.Empty;
        
        [Required]
        public string Sex { get; set; } = string.Empty;
        
        [Required]
        public int Age { get; set; }
        
        [Required]
        public string ContactNumber { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;
        
        // Foreign key to link to ApplicationUser
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }
} 