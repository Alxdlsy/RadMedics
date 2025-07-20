using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RadMedics.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RadMedics.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.UserType = "Admin";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && user.IsAdmin)
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
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsAdmin)
            {
                return RedirectToAction("Login");
            }

            // Get statistics
            var totalUsers = await _userManager.Users.CountAsync();
            var activeUsers = await _userManager.Users.Where(u => u.EmailConfirmed).CountAsync();
            var totalModules = await _context.Courses.CountAsync();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.ActiveUsers = activeUsers;
            ViewBag.TotalModules = totalModules;

            // Get modules for the admin
            var modules = await _context.Courses.ToListAsync();
            ViewBag.Modules = modules;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsAdmin)
            {
                return RedirectToAction("Login");
            }

            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Modules()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsAdmin)
            {
                return RedirectToAction("Login");
            }

            var modules = await _context.Courses.ToListAsync();
            ViewBag.InProgressModules = modules.Count(m => m.Progress < 100);
            ViewBag.TotalModules = modules.Count;

            return View(modules);
        }

        [HttpGet]
        public async Task<IActionResult> Assessments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsAdmin)
            {
                return RedirectToAction("Login");
            }

            var modules = await _context.Courses.ToListAsync();
            ViewBag.InProgressAssessments = modules.Count(m => m.Progress < 100);
            ViewBag.TotalAssessments = modules.Count;

            return View(modules);
        }

        [HttpGet]
        public IActionResult Settings()
        {
            return View();
        }

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

        [HttpPost]
        public async Task<IActionResult> AddModule(string title, string moduleCode)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsAdmin)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var module = new Course
            {
                Name = title,
                StudentId = "", // Will be updated when students are enrolled
                Progress = 0
            };

            _context.Courses.Add(module);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Module added successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteModule(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsAdmin)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var module = await _context.Courses.FindAsync(id);
            if (module == null)
            {
                return Json(new { success = false, message = "Module not found" });
            }

            _context.Courses.Remove(module);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Module deleted successfully!" });
        }
    }
} 