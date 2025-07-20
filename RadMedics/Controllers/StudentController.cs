using Microsoft.AspNetCore.Mvc;
using RadMedics.Models;
using RadMedics.Models.ViewModels;  // Make sure ViewModels are properly structured
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System; // Added for Exception
using Microsoft.AspNetCore.Mvc.ModelBinding; // Added for FromBody
using System.Collections.Generic; // Added for List

namespace RadMedics.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; // Use UserManager for getting logged-in user
        private readonly SignInManager<ApplicationUser> _signInManager;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.UserType = "Student";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && !user.IsAdmin)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Dashboard");
                }
            }
            ViewBag.Error = "Invalid login attempt.";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Student");
            }

            // Get or create student profile
            var profile = await _context.StudentProfiles
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (profile == null)
            {
                // Create default profile if none exists
                profile = new StudentProfile
                {
                    UserId = user.Id,
                    Name = user.FirstName + " " + user.LastName,
                    Course = "Bachelor of Science in Information Technology",
                    Sex = "Male",
                    Age = 25,
                    ContactNumber = "0920-208-2088",
                    EmailAddress = user.Email ?? ""
                };
                _context.StudentProfiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            // Get today's events for the sidebar calendar
            var today = DateTime.Today;
            var todaysEvents = await _context.CalendarEvents
                .Where(e => e.UserId == user.Id && e.StartDate.Date == today)
                .ToListAsync();

            ViewBag.TodaysEvents = todaysEvents;
            ViewBag.CurrentDate = today;

            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] StudentProfile model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                var profile = await _context.StudentProfiles
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);

                if (profile == null)
                {
                    profile = new StudentProfile { UserId = user.Id };
                    _context.StudentProfiles.Add(profile);
                }

                // Update profile data
                profile.Name = model.Name;
                profile.Course = model.Course;
                profile.Sex = model.Sex;
                profile.Age = model.Age;
                profile.ContactNumber = model.ContactNumber;
                profile.EmailAddress = model.EmailAddress;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Profile updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult Courses() => View();
        
        [HttpGet]
        public async Task<IActionResult> Calendar()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Student");
            }

            // Get current month's events
            var currentDate = DateTime.Now;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var events = await _context.CalendarEvents
                .Where(e => e.UserId == user.Id && e.StartDate >= startOfMonth && e.StartDate <= endOfMonth)
                .ToListAsync();

            // Prepare calendar data
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            var daysInMonth = lastDayOfMonth.Day;
            
            // Adjust for Monday start (0 = Monday, 6 = Sunday)
            var startOffset = firstDayOfWeek == 0 ? 6 : firstDayOfWeek - 1;
            var totalCells = startOffset + daysInMonth;
            var weeks = (totalCells + 6) / 7;

            var calendarDays = new List<CalendarDayViewModel>();
            for (int week = 0; week < weeks; week++)
            {
                for (int dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++)
                {
                    var cellIndex = week * 7 + dayOfWeek;
                    var dayNumber = cellIndex - startOffset + 1;
                    
                    if (cellIndex < startOffset || dayNumber > daysInMonth)
                    {
                        calendarDays.Add(new CalendarDayViewModel { IsEmpty = true });
                    }
                    else
                    {
                        var date = new DateTime(currentDate.Year, currentDate.Month, dayNumber);
                        var isToday = date.Date == DateTime.Today.Date;
                        var dayEvents = events.Where(e => e.StartDate.Date == date.Date).ToList();
                        
                        calendarDays.Add(new CalendarDayViewModel { 
                            IsEmpty = false, 
                            DayNumber = dayNumber, 
                            IsToday = isToday, 
                            Date = date.ToString("yyyy-MM-dd"),
                            Events = dayEvents
                        });
                    }
                }
            }

            ViewBag.Events = events;
            ViewBag.CurrentMonth = currentDate;
            ViewBag.CalendarDays = calendarDays;
            ViewBag.Weeks = weeks;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CalendarEvent model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // Log the incoming data
                Console.WriteLine($"Creating event: Title={model.Title}, StartDate={model.StartDate}, UserId={user.Id}");

                model.UserId = user.Id;
                model.User = user;
                
                // Set default values for removed fields
                model.IsRepeating = false;
                model.RepeatCount = null;
                model.DurationMinutes = null;

                _context.CalendarEvents.Add(model);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Event created successfully with ID: {model.Id}");

                return Json(new { success = true, message = "Event created successfully!", eventId = model.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating event: {ex.Message}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents(string date)
        {
            try
            {
                Console.WriteLine($"GetEvents called with date: {date}");
                
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    Console.WriteLine("User not found - returning error");
                    return Json(new { success = false, message = "User not found" });
                }

                Console.WriteLine($"Getting events for date: {date}, UserId: {user.Id}");

                if (DateTime.TryParse(date, out var targetDate))
                {
                    // Get all events for the user first to debug
                    var allEvents = await _context.CalendarEvents
                        .Where(e => e.UserId == user.Id)
                        .ToListAsync();
                    
                    Console.WriteLine($"Total events for user: {allEvents.Count}");
                    
                    // Then filter by date
                    var events = allEvents
                        .Where(e => e.StartDate.Date == targetDate.Date)
                        .Select(e => new { e.Id, e.Title, e.Description, e.StartDate, e.EndDate, e.Location })
                        .ToList();

                    Console.WriteLine($"Found {events.Count} events for date {targetDate.Date}");
                    
                    // Log each event for debugging
                    foreach (var evt in events)
                    {
                        Console.WriteLine($"Event: ID={evt.Id}, Title={evt.Title}, StartDate={evt.StartDate}");
                    }

                    return Json(new { success = true, events });
                }

                Console.WriteLine($"Invalid date format: {date}");
                return Json(new { success = false, message = "Invalid date format" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetEvents: {ex.Message}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEventsByMonth(int year, int month)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            var startOfMonth = new DateTime(year, month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var events = await _context.CalendarEvents
                .Where(e => e.UserId == user.Id && e.StartDate >= startOfMonth && e.StartDate <= endOfMonth)
                .Select(e => new { e.Id, e.Title, e.StartDate, e.EndDate, e.Location })
                .ToListAsync();

            return Json(new { success = true, events });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] CalendarEvent model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                var existingEvent = await _context.CalendarEvents
                    .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

                if (existingEvent == null)
                {
                    return Json(new { success = false, message = "Event not found" });
                }

                // Update event properties
                existingEvent.Title = model.Title;
                existingEvent.Description = model.Description;
                existingEvent.Location = model.Location;
                existingEvent.StartDate = model.StartDate;
                existingEvent.EndDate = model.EndDate;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Event updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                var existingEvent = await _context.CalendarEvents
                    .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

                if (existingEvent == null)
                {
                    return Json(new { success = false, message = "Event not found" });
                }

                _context.CalendarEvents.Remove(existingEvent);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Event deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }



        [HttpGet]
        public async Task<IActionResult> CreateTestEvent()
        {
            try
            {
                Console.WriteLine("CreateTestEvent called");
                
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    Console.WriteLine("User not found in CreateTestEvent");
                    return Json(new { success = false, message = "User not found" });
                }

                Console.WriteLine($"User found: {user.Email}");

                // Create a test event for today
                var testEvent = new CalendarEvent
                {
                    Title = "-Assignment",
                    Description = "Test assignment",
                    StartDate = DateTime.Today.AddHours(9),
                    UserId = user.Id,
                    User = user,
                    IsRepeating = false,
                    RepeatCount = null,
                    DurationMinutes = null
                };

                Console.WriteLine($"Creating test event for date: {testEvent.StartDate.Date}, UserId: {user.Id}");

                _context.CalendarEvents.Add(testEvent);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Test event created with ID: {testEvent.Id}");

                return Json(new { success = true, message = "Test event created!", eventId = testEvent.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating test event: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult Forum() => RedirectToAction("Index", "Forum");
        [HttpGet]
        public IActionResult Settings() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                TempData["ChangePasswordError"] = "New password and confirmation do not match.";
                return RedirectToAction("Settings");
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ChangePasswordError"] = "User not found.";
                return RedirectToAction("Settings");
            }
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                TempData["ChangePasswordSuccess"] = "Password changed successfully.";
            }
            else
            {
                TempData["ChangePasswordError"] = string.Join(" ", result.Errors.Select(e => e.Description));
            }
            return RedirectToAction("Settings");
        }

        // Dashboard action for student
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // Get the logged-in user
            var user = await _userManager.GetUserAsync(User);  // Get the logged-in user using UserManager

            if (user == null)
            {
                // If the user is not logged in, redirect to the student login page
                return RedirectToAction("Login", "Student");
            }

            // Get student profile for display name
            var profile = await _context.StudentProfiles
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            // Fetch courses for the student using the student's Id
            var courses = await _context.Courses
                                         .Where(c => c.StudentId == user.Id)  // Assuming the 'StudentId' field in 'Courses' table
                                         .ToListAsync();

            // Get today's events for the sidebar calendar
            var today = DateTime.Today;
            var todaysEvents = await _context.CalendarEvents
                .Where(e => e.UserId == user.Id && e.StartDate.Date == today)
                .ToListAsync();

            // Create the DashboardViewModel to pass to the view
            var viewModel = new DashboardViewModel
            {
                WelcomeMessage = $"Welcome, {profile?.Name ?? user.FirstName}!", // Use profile name if available
                CourseList = courses.Select(c => new CourseViewModel
                {
                    Name = c.Name,
                    Progress = c.Progress  // Ensure the 'Progress' property exists in the 'Courses' table
                }).ToList()
            };

            ViewBag.TodaysEvents = todaysEvents;
            ViewBag.CurrentDate = today;

            // Pass the ViewModel to the Dashboard view
            return View(viewModel);
        }
    }
}
