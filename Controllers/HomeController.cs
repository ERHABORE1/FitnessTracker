using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FitnessTracker.Models;
using Microsoft.EntityFrameworkCore;
using FitnessTracker.Service;

namespace FitnessTracker.Controllers
{
    /// <summary>
    /// Handles public pages (home, privacy) and the main dashboard for logged-in clients.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// Creates a new instance of <see cref="HomeController"/> with logging and database context.
        /// </summary>
        /// <param name="logger">Logger used to record diagnostic information.</param>
        /// <param name="db">Application database context used for queries.</param>
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        /// <summary>
        /// Displays the public landing page for the application.
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays the privacy policy page.
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays the main dashboard for a logged-in client.
        /// Shows any pending trainer access requests that require a decision.
        /// </summary>
        [HttpGet]
        public IActionResult Dashboard()
        {
            var uid = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(uid))
                return RedirectToAction("Login", "Auth");

            int clientId = int.Parse(uid);

            // Load trainer requests sent to this client that are still pending
            var requests = _db.TrainerClientRequests
                .Where(r => r.ClientId == clientId && r.Status == "Pending")
                .Include(r => r.Trainer)
                .ToList();

            ViewBag.Requests = requests;

            return View();
        }

        /// <summary>
        /// Handles the acceptance of a trainer access request from the client side.
        /// </summary>
        /// <param name="id">The primary key of the <see cref="TrainerClientRequest"/> record.</param>
        [HttpPost]
        public async Task<IActionResult> AcceptTrainer(int id)
        {
            var req = await _db.TrainerClientRequests.FindAsync(id);

            if (req != null)
            {
                req.Status = "Accepted";
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Dashboard");
        }

        /// <summary>
        /// Handles the decline of a trainer access request from the client side.
        /// </summary>
        /// <param name="id">The primary key of the <see cref="TrainerClientRequest"/> record.</param>
        [HttpPost]
        public async Task<IActionResult> DeclineTrainer(int id)
        {
            var req = await _db.TrainerClientRequests.FindAsync(id);

            if (req != null)
            {
                req.Status = "Declined";
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Dashboard");
        }

        /// <summary>
        /// Standard error handler used by ASP.NET Core to display an error view with a RequestId.
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
