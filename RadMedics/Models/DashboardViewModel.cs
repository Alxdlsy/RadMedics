namespace RadMedics.Models.ViewModels
{
    public class DashboardViewModel
    {
        public string WelcomeMessage { get; set; } = string.Empty; // Default to empty string
        public List<CourseViewModel> CourseList { get; set; } = new List<CourseViewModel>(); // Default to empty list
    }
}
