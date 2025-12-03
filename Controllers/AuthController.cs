using FitnessTracker.Models;
using FitnessTracker.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace FitnessTracker.Controllers
{
    /// <summary>
    /// Handles user authentication: registration, login, logout, and basic terms page.
    /// </summary>
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// Creates a new instance of <see cref="AuthController"/> using the EF Core database context.
        /// </summary>
        /// <param name="db">Application database context used for user queries and updates.</param>
        public AuthController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Displays the registration page where a new user or trainer can create an account.
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Handles registration form submission. Validates passwords, checks for duplicate email,
        /// hashes the password, and saves the new user.
        /// </summary>
        /// <param name="user">User model posted from the registration form.</param>
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (user.Password != user.ConfirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View(user);
            }

            if (await _db.Users.AnyAsync(x => x.Email == user.Email))
            {
                ViewBag.Error = "This email is already registered.";
                return View(user);
            }

            // Hash the plain text password before saving to the database
            user.PasswordHash = HashPassword(user.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Account created successfully.";
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Displays the login page.
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Handles login form submission. Verifies email and password, then stores the user
        /// identity and role in the session. Redirects trainers and normal users to different dashboards.
        /// </summary>
        /// <param name="email">Email address entered by the user.</param>
        /// <param name="password">Plain text password entered by the user.</param>
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            // Store basic identity in the session for later authorization checks
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("Role", user.Role);

            // Redirect based on role: trainers go to Trainer dashboard, normal users go to client dashboard
            if (user.Role == "Trainer")
                return RedirectToAction("Index", "Trainer");

            return RedirectToAction("Dashboard", "Home");
        }

        /// <summary>
        /// Logs the current user out by clearing the session and redirecting to the login page.
        /// </summary>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Hashes a plain-text password using SHA256 and returns the Base64 string.
        /// </summary>
        /// <param name="password">Plain-text password to hash.</param>
        /// <returns>Hashed password stored as a Base64-encoded string.</returns>
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        /// <summary>
        /// Verifies a password by hashing it and comparing it to a stored hash.
        /// </summary>
        /// <param name="password">Plain-text password provided during login.</param>
        /// <param name="hash">Hashed password stored in the database.</param>
        /// <returns>True if the password matches the hash, otherwise false.</returns>
        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        /// <summary>
        /// Displays a static terms and conditions page if needed.
        /// </summary>
        public IActionResult Terms()
        {
            return View();
        }
    }
}
