using FitnessTracker.Models;
using FitnessTracker.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Controllers
{
    /// <summary>
    /// Handles trainer-specific workflows: managing clients, sending access requests,
    /// viewing client progress, and assigning workout templates.
    /// </summary>
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// Creates a new instance of <see cref="TrainerController"/>.
        /// </summary>
        /// <param name="db">Application database context used to query and update trainer-related data.</param>
        public TrainerController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Checks whether the current session belongs to a trainer (Role == "Trainer").
        /// </summary>
        /// <returns>True if the logged-in user is a trainer, otherwise false.</returns>
        private bool IsTrainer()
        {
            return HttpContext.Session.GetString("Role") == "Trainer";
        }

        /// <summary>
        /// Displays the trainer dashboard. Redirects to login if the user is not a trainer.
        /// </summary>
        public IActionResult Index()
        {
            if (!IsTrainer())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        /// <summary>
        /// Displays all potential client accounts, showing the latest request status
        /// between the trainer and each client.
        /// </summary>
        public async Task<IActionResult> Clients()
        {
            if (!IsTrainer())
                return RedirectToAction("Login", "Auth");

            int trainerId = int.Parse(HttpContext.Session.GetString("UserId")!);

            var clients = await _db.Users
                .Where(u => u.Role == "User")
                .ToListAsync();

            var requests = await _db.TrainerClientRequests
                .Where(r => r.TrainerId == trainerId)
                .ToListAsync();

            ViewBag.Requests = requests;
            return View(clients);
        }

        /// <summary>
        /// Sends or re-sends an access request from the trainer to a specific client.
        /// Allows re-requesting only if the last request was declined, but not if accepted.
        /// </summary>
        /// <param name="clientId">The ID of the client to whom the trainer is requesting access.</param>
        [HttpPost]
        public async Task<IActionResult> RequestAccess(int clientId)
        {
            if (!IsTrainer()) return Unauthorized();

            int trainerId = int.Parse(HttpContext.Session.GetString("UserId")!);

            // Get most recent request between trainer and client
            var lastRequest = await _db.TrainerClientRequests
                .Where(r => r.TrainerId == trainerId && r.ClientId == clientId)
                .OrderByDescending(r => r.SentDate)
                .FirstOrDefaultAsync();

            // CASE 1: No requests ever → create one
            if (lastRequest == null)
            {
                _db.TrainerClientRequests.Add(new TrainerClientRequest
                {
                    TrainerId = trainerId,
                    ClientId = clientId,
                    Status = "Pending",
                    SentDate = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
                return RedirectToAction("Clients");
            }

            // CASE 2: Last request was Declined → allow re-request
            if (lastRequest.Status == "Declined")
            {
                _db.TrainerClientRequests.Add(new TrainerClientRequest
                {
                    TrainerId = trainerId,
                    ClientId = clientId,
                    Status = "Pending",
                    SentDate = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
                return RedirectToAction("Clients");
            }

            // CASE 3: Last request is Pending → do nothing, just inform trainer
            if (lastRequest.Status == "Pending")
            {
                TempData["Info"] = "Request already sent and is pending.";
                return RedirectToAction("Clients");
            }

            // CASE 4: Last request was Accepted → DO NOT ALLOW re-request
            if (lastRequest.Status == "Accepted")
            {
                TempData["Info"] = "You already have access to this client.";
                return RedirectToAction("Clients");
            }

            return RedirectToAction("Clients");
        }

        /// <summary>
        /// Displays all progress logs for a specific client so that the trainer can review progress.
        /// </summary>
        /// <param name="clientId">The ID of the client whose progress is being viewed.</param>
        public async Task<IActionResult> ProgressView(int clientId)
        {
            if (!IsTrainer())
                return RedirectToAction("Login", "Auth");

            var logs = await _db.ProgressLogs
                .Where(p => p.UserId == clientId)
                .OrderBy(p => p.EntryDate)
                .ToListAsync();

            var client = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == clientId);

            ViewBag.ClientId = clientId;
            ViewBag.ClientName = client?.Name ?? "Client";

            return View("ProgressView", logs);
        }

        /// <summary>
        /// Saves trainer feedback text against a single progress log.
        /// This is the preferred endpoint for trainer feedback in the current design.
        /// </summary>
        /// <param name="logId">The ID of the specific progress log being commented on.</param>
        /// <param name="feedback">Feedback text provided by the trainer.</param>
        [HttpPost]
        public async Task<IActionResult> AddTrainerFeedback(int logId, string feedback)
        {
            if (!IsTrainer())
                return Unauthorized();

            var log = await _db.ProgressLogs
                .FirstOrDefaultAsync(l => l.ProgressLogId == logId);

            if (log == null)
                return NotFound();

            log.TrainerFeedback = feedback;
            await _db.SaveChangesAsync();

            // Redirect back to that client's progress page
            return RedirectToAction("ProgressView", new { clientId = log.UserId });
        }

        /// <summary>
        /// Allows a client to accept a trainer access request from the trainer side.
        /// This action is invoked when a client clicks accept on the dashboard.
        /// </summary>
        /// <param name="id">The primary key of the <see cref="TrainerClientRequest"/> to accept.</param>
        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int id)
        {
            var request = await _db.TrainerClientRequests
                .FirstOrDefaultAsync(r => r.TrainerClientRequestId == id);

            if (request == null)
                return NotFound();

            request.Status = "Accepted";
            await _db.SaveChangesAsync();

            TempData["Success"] = "Trainer request accepted.";
            return RedirectToAction("Dashboard", "Home");
        }

        /// <summary>
        /// Allows a client to decline a trainer access request.
        /// </summary>
        /// <param name="id">The primary key of the <see cref="TrainerClientRequest"/> to decline.</param>
        [HttpPost]
        public async Task<IActionResult> DeclineRequest(int id)
        {
            var request = await _db.TrainerClientRequests
                .FirstOrDefaultAsync(r => r.TrainerClientRequestId == id);

            if (request == null)
                return NotFound();

            request.Status = "Declined";
            await _db.SaveChangesAsync();

            TempData["Success"] = "Trainer request declined.";
            return RedirectToAction("Dashboard", "Home");
        }

        /// <summary>
        /// Displays a page where a trainer can assign workout templates to any client
        /// that has accepted their access request.
        /// </summary>
        public async Task<IActionResult> Assign()
        {
            if (!IsTrainer())
                return RedirectToAction("Login", "Auth");

            int trainerId = int.Parse(HttpContext.Session.GetString("UserId")!);

            var acceptedClients = await _db.TrainerClientRequests
                .Where(r => r.TrainerId == trainerId && r.Status == "Accepted")
                .Select(r => r.Client!)
                .ToListAsync();

            var templates = await _db.WorkoutTemplates.ToListAsync();

            ViewBag.Clients = acceptedClients;
            ViewBag.Templates = templates;

            return View();
        }

        /// <summary>
        /// Creates an assignment record that links a workout template to a specific client for this trainer.
        /// </summary>
        /// <param name="clientId">The ID of the client receiving the template.</param>
        /// <param name="templateId">The ID of the template being assigned.</param>
        [HttpPost]
        public async Task<IActionResult> AssignTemplate(int clientId, int templateId)
        {
            if (!IsTrainer())
                return Unauthorized();

            int trainerId = int.Parse(HttpContext.Session.GetString("UserId")!);

            var assignment = new TrainerAssignedWorkout
            {
                TrainerId = trainerId,
                ClientId = clientId,
                WorkoutTemplateId = templateId,
                AssignedDate = DateTime.Now
            };

            _db.TrainerAssignedWorkouts.Add(assignment);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Template assigned successfully!";
            return RedirectToAction("Assign");
        }
    }
}
