using System.ComponentModel.DataAnnotations;

namespace RadMedics.Models
{
    public class CalendarEvent
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public string Location { get; set; } = string.Empty;
        
        public string EventType { get; set; } = "User";
        
        public int? DurationMinutes { get; set; }
        
        public bool IsRepeating { get; set; }
        
        public int? RepeatCount { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        
        public ApplicationUser User { get; set; } = null!;
    }
} 