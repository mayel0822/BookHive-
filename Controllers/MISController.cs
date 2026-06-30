using BookHive.Filters;
using BookHive.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookHive.Controllers
{
    [RequireMISLogin]
    public class MISController : Controller
    {
        private readonly AppDbContext _db;

        public MISController(AppDbContext db)
        {
            _db = db;
        }

        // ── LOGIN ──────────────────────────────────
        [AllowMISAnonymous]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("MISUser") != null)
                return RedirectToAction("Dashboard");

            return View();
        }


        // ── MICROSOFT LOGIN CALLBACK ────────────────
        [AllowMISAnonymous]
        public async Task<IActionResult> MicrosoftLogin()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var email = User.FindFirst("preferred_username")?.Value
                            ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                            ?? User.Identity.Name ?? "";

                var facilitator = await _db.MISFacilitators
                    .FirstOrDefaultAsync(f => f.Email == email);

                if (facilitator == null)
                {
                    await HttpContext.SignOutAsync("Cookies");
                    await HttpContext.SignOutAsync("OpenIdConnect");
                    ViewBag.Error = $"Access denied. '{email}' is not a registered MIS facilitator.";
                    return View("Login");
                }

                HttpContext.Session.SetString("MISUser", email);
                HttpContext.Session.SetString("MISName",
                    $"{facilitator.FirstName} {facilitator.LastName}");
                return RedirectToAction("Dashboard");
            }

            var props = new AuthenticationProperties { RedirectUri = "/MIS/MicrosoftLogin" };
            props.Items["prompt"] = "login";
            return Challenge(props, "OpenIdConnect");
        }

        // ── DASHBOARD ──────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            var allUsers = await _db.Users.ToListAsync();

            var visibleUsers = allUsers
                .Where(u => u.Status != "Archived")
                .ToList();

            ViewBag.TotalUsers = visibleUsers.Count;
            ViewBag.ActiveUsers = visibleUsers.Count(u => u.Status == "Active");
            ViewBag.Students = visibleUsers.Count(u => u.Role == "Student");
            ViewBag.Users = visibleUsers;

            return View();
        }

        // ── REGISTRATION ───────────────────────────
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(
            string role, string studId, string empId,
            string lastName, string firstName,
            string email, string rfid, string department)
        {
            var existing = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (existing != null)
            {
                ViewBag.Error = "A user with this email already exists.";
                return View();
            }

            var user = new User
            {
                Role = role,
                StudId = studId,
                EmpId = empId,
                LastName = lastName,
                FirstName = firstName,
                Email = email,
                RFIDCard = rfid,
                Department = department,
                Status = "Active"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            ViewBag.Success =
                $"{role} account for {firstName} {lastName} " +
                $"registered successfully!";

            return View();
        }

        // ── USER INFORMATION (LIST + DETAIL) ───────
        public async Task<IActionResult> UserInfo(int? id)
        {
            if (id == null)
            {
                var users = await _db.Users
                    .Where(u => u.Status != "Archived")
                    .ToListAsync();
                var archived = await _db.Users
                    .Where(u => u.Status == "Archived")
                    .ToListAsync();
                ViewBag.ArchivedUsers = archived;
                return View("UserInfoList", users);
            }

            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return RedirectToAction("UserInfo");

            return View("UserInfoDetail", user);
        }

        // ── ARCHIVED USERS ─────────────────────────
        public async Task<IActionResult> ArchivedUsers()
        {
            var archived = await _db.Users
                .Where(u => u.Status == "Archived")
                .ToListAsync();

            return View(archived);
        }

        // ── ARCHIVE ────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> ArchiveUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                user.Status = "Archived";
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("UserInfo");
        }

        // ── RESTORE ────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> RestoreUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                user.Status = "Active";
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("ArchivedUsers");
        }

        // ── RESET PASSWORD ──────────────────────────
        [HttpPost]
        public async Task<IActionResult> ResetPassword(int id, string newPassword)
        {
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                user.Password = newPassword;
                await _db.SaveChangesAsync();
                TempData["Toast"] =
                    $"Password reset successfully for {user.FullName}.";
            }

            return RedirectToAction("UserInfo", new { id });
        }

        [AllowMISAnonymous]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return SignOut(
                    new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                    {
                        RedirectUri = "/MIS/Login"
                    },
                    "Cookies",
                    "OpenIdConnect"
                );
            }

            return RedirectToAction("Login");
        }
    }
}