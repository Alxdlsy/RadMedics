using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using RadMedics.Models;
using System.Threading.Tasks;

namespace RadMedics.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager; // Use UserManager to handle identity
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET Register Page
        public IActionResult Register()
        {
            return View();
        }

        // POST Register User
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Check if the email already exists in the database using UserManager
                if (string.IsNullOrEmpty(user.Email))
                {
                    ModelState.AddModelError("Email", "Email is required.");
                    return View(user);
                }

                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "An account with this email already exists.");
                    return View(user); // Return to the register page with error
                }

                // Create a new user using ASP.NET Identity UserManager
                var newUser = new ApplicationUser
                {
                    UserName = user.Email, // Use the email as the username
                    Email = user.Email,
                };

                if (string.IsNullOrEmpty(user.Password))
                {
                    ModelState.AddModelError("Password", "Password is required.");
                    return View(user);
                }

                var result = await _userManager.CreateAsync(newUser, user.Password);

                if (result.Succeeded)
                {
                    // If successful, redirect to login page
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(user); // Return to the register page with validation errors
        }

        // Login page
        public IActionResult Login()
        {
            return View();
        }

        // POST Login User
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email is required.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                // Validate password using UserManager
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

                if (result.Succeeded)
                {
                    // After successful login, redirect to Student Dashboard
                    return RedirectToAction("Dashboard", "Student");
                }
            }

            // If login fails, show error
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(); // Return to the login page with error message
        }
    }
}
