namespace RadMedics.Models
{
    public class CalendarDayViewModel
    {
        public bool IsEmpty { get; set; }
        public int DayNumber { get; set; }
        public bool IsToday { get; set; }
        public string Date { get; set; } = string.Empty;
        public List<CalendarEvent> Events { get; set; } = new List<CalendarEvent>();
    }
} 