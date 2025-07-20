using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RadMedics.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RadMedics.Controllers
{
    public class ForumController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ForumController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Main forum page
        public IActionResult Index()
        {
            // Add some debugging
            System.Diagnostics.Debug.WriteLine("Forum Index action called");
            return View();
        }

        // Add some test data for debugging
        [HttpGet]
        public async Task<IActionResult> AddTestData()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                if (users.Count < 2) return Json(new { success = false, message = "Need at least 2 users" });

                var user1 = users[0];
                var user2 = users[1];

                // Add some test mails
                var testMails = new List<MailMessage>
                {
                    new MailMessage
                    {
                        FromUserId = user1.Id,
                        ToUserId = user2.Id,
                        Subject = "Welcome to the Forum!",
                        Content = "This is a test message to welcome you to our forum system.",
                        SentDate = DateTime.Now.AddDays(-1),
                        IsDraft = false,
                        IsRead = false
                    },
                    new MailMessage
                    {
                        FromUserId = user2.Id,
                        ToUserId = user1.Id,
                        Subject = "Re: Welcome to the Forum!",
                        Content = "Thank you! I'm excited to be part of this community.",
                        SentDate = DateTime.Now.AddHours(-2),
                        IsDraft = false,
                        IsRead = true
                    },
                    new MailMessage
                    {
                        FromUserId = user1.Id,
                        ToUserId = user2.Id,
                        Subject = "Draft Message",
                        Content = "This is a draft message that hasn't been sent yet.",
                        SentDate = null,
                        IsDraft = true,
                        IsRead = false
                    }
                };

                foreach (var mail in testMails)
                {
                    if (!_context.MailMessages.Any(m => m.Subject == mail.Subject))
                    {
                        _context.MailMessages.Add(mail);
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Test data added successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Inbox
        public async Task<IActionResult> Inbox()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Student");
            
            var mails = await _context.MailMessages
                .Include(m => m.FromUser)
                .Where(m => m.ToUserId == user.Id && !m.IsDraft)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();
            return PartialView("_MailListPartial", mails);
        }

        // Drafts
        public async Task<IActionResult> Drafts()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Student");
            
            var mails = await _context.MailMessages
                .Where(m => m.FromUserId == user.Id && m.IsDraft)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();
            return PartialView("_MailListPartial", mails);
        }

        // Sent
        public async Task<IActionResult> Sent()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Student");
            
            var mails = await _context.MailMessages
                .Include(m => m.ToUser)
                .Where(m => m.FromUserId == user.Id && !m.IsDraft)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();
            return PartialView("_MailListPartial", mails);
        }

        // View a single mail
        public async Task<IActionResult> ViewMail(int id)
        {
            var mail = await _context.MailMessages
                .Include(m => m.FromUser)
                .Include(m => m.ToUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mail == null) return NotFound();
            
            // Mark as read if recipient is viewing
            var user = await _userManager.GetUserAsync(User);
            if (user != null && mail.ToUserId == user.Id && !mail.IsRead)
            {
                mail.IsRead = true;
                await _context.SaveChangesAsync();
            }
            return PartialView("_MailDetailPartial", mail);
        }

        // Compose new mail (GET)
        public async Task<IActionResult> AddMail()
        {
            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = users;
            return PartialView("_ComposeMailPartial", null);
        }

        // Compose new mail (POST)
        [HttpPost]
        public async Task<IActionResult> AddMail(string toUserId, string subject, string content, bool isDraft = false)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false, message = "User not found" });
            
            var mail = new MailMessage
            {
                FromUserId = user.Id,
                ToUserId = toUserId,
                Subject = subject,
                Content = content,
                SentDate = isDraft ? (System.DateTime?)null : System.DateTime.Now,
                IsDraft = isDraft,
                IsRead = false
            };
            _context.MailMessages.Add(mail);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
} 