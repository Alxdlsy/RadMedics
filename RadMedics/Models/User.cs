using System.ComponentModel.DataAnnotations;

namespace RadMedics.Models
{
    public class User
    {
        public int Id { get; set; } // Primary Key

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        // Add any other necessary fields, such as IsAdmin, etc.
        public bool IsAdmin { get; set; }
    }
}
