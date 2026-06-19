using BookHive.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookHive.Controllers
{
    public class MISController : Controller
    {
        private readonly AppDbContext _db;

        public MISController(AppDbContext db)
        {
            _db = db;
        }

        // ── LOGIN ──────────────────────────────────
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("MISUser") != null)
                return RedirectToAction("Dashboard");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var facilitator = await _db.MISFacilitators
                .FirstOrDefaultAsync(f => f.Email == email && f.Password == password);

            if (facilitator != null)
            {
                HttpContext.Session.SetString("MISUser", email);
                HttpContext.Session.SetString("MISName",
                    $"{facilitator.FirstName} {facilitator.LastName}");
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        // ── DASHBOARD ──────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            if (HttpContext.Session.GetString("MISUser") == null)
                return RedirectToAction("Login");

            var allUsers = await _db.Users.ToListAsync();

            // Only count users that are NOT archived
            var visibleUsers = allUsers.Where(u => u.Status != "Archived").ToList();

            ViewBag.TotalUsers = visibleUsers.Count;
            ViewBag.ActiveUsers = visibleUsers.Count(u => u.Status == "Active");
            ViewBag.Librarians = visibleUsers.Count(u => u.Role == "Librarian");
            ViewBag.Students = visibleUsers.Count(u => u.Role == "Student");
            ViewBag.Users = visibleUsers;

            return View();
        }

        // ── REGISTRATION ───────────────────────────
        public IActionResult Registration()
        {
            if (HttpContext.Session.GetString("MISUser") == null)
                return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(
            string role, string studId, string empId,
            string lastName, string firstName,
            string email, string phone,
            string rfid, string department)
        {
            if (HttpContext.Session.GetString("MISUser") == null)
                return RedirectToAction("Login");

            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
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
                Phone = phone,
                RFIDCard = rfid,
                Department = department,
                Status = "Active"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            ViewBag.Success =
                $"{role} account for {firstName} {lastName} registered successfully!";
            return View();
        }

        // ── USER INFORMATION (LIST + DETAIL) ───────
        public async Task<IActionResult> UserInfo(int? id)
        {
            if (HttpContext.Session.GetString("MISUser") == null)
                return RedirectToAction("Login");

            if (id == null)
            {
                var users = await _db.Users
                    .Where(u => u.Status != "Archived")
                    .ToListAsync();
                return View("UserInfoList", users);
            }

            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return RedirectToAction("UserInfo");

            return View("UserInfoDetail", user);
        }

        // ── ARCHIVE LIST PAGE ───────────────────────
        public async Task<IActionResult> ArchivedUsers()
        {
            if (HttpContext.Session.GetString("MISUser") == null)
                return RedirectToAction("Login");

            var archived = await _db.Users
                .Where(u => u.Status == "Archived")
                .ToListAsync();

            return View(archived);
        }

        // ── ARCHIVE ────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> ArchiveUser(int id)
        {
            if (HttpContext.Session.GetString("MISUser") == null)
                return RedirectToAction("Login");

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
            if (HttpContext.Session.GetString("MISUser") == null)
                return RedirectToAction("Login");

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
            if (HttpContext.Session.GetString("MISUser") == null)
                return RedirectToAction("Login");

            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                user.Password = newPassword;
                await _db.SaveChangesAsync();
                TempData["Toast"] = $"Password reset successfully for {user.FullName}.";
            }

            return RedirectToAction("UserInfo", new { id });
        }

        // ── LOGOUT ─────────────────────────────────
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}