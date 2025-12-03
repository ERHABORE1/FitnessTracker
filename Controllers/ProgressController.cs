using FitnessTracker.Models;
using FitnessTracker.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Controllers
{
    /// <summary>
    /// Handles client progress logging (weight/body fat), editing, deletion,
    /// and provides a basic hook for trainer feedback on client progress.
    /// </summary>
    public class ProgressController : Controller
    {
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// Creates a new instance of <see cref="ProgressController"/>.
        /// </summary>
        /// <param name="db">Application database context used to manage progress logs.</param>
        public ProgressController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Reads the current user's ID from the session. Returns null if the user is not logged in.
        /// </summary>
        /// <returns>The numeric user ID, or null if the session does not contain a valid ID.</returns>
        private int? GetUserId()
        {
            var idStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrWhiteSpace(idStr)) return null;
            return int.Parse(idStr);
        }

        /// <summary>
        /// Displays the main "Your Progress" page for the current user.
        /// Shows a form to log new entries, a chart, and a history table.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var logs = await _db.ProgressLogs
                .Where(p => p.UserId == userId.Value)
                .OrderBy(p => p.EntryDate)
                .ToListAsync();

            return View(logs);
        }

       

        /// <summary>
        /// Handles the creation of a new progress log entry from the "Your Progress" page.
        /// Validates the model, associates the log with the current user, and saves it.
        /// </summary>
        /// <param name="log">ProgressLog data posted from the form (date, weight, body fat, notes).</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("EntryDate,Weight,BodyFatPercent,Notes")] ProgressLog log)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                var logs = await _db.ProgressLogs
                    .Where(p => p.UserId == userId.Value)
                    .OrderBy(p => p.EntryDate)
                    .ToListAsync();

                // If validation fails, redisplay the Index view with existing logs.
                return View("Index", logs);
            }

            log.UserId = userId.Value;

            _db.ProgressLogs.Add(log);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the edit page for a single progress log that belongs to the current user.
        /// </summary>
        /// <param name="id">The primary key of the <see cref="ProgressLog"/> to edit.</param>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var log = await _db.ProgressLogs
                .FirstOrDefaultAsync(p => p.ProgressLogId == id
                                          && p.UserId == userId.Value);

            if (log == null) return NotFound();

            return View(log);
        }

        /// <summary>
        /// Handles POST of edits to an existing progress log.
        /// Ensures that the log belongs to the current user before saving.
        /// </summary>
        /// <param name="id">ID of the log being edited.</param>
        /// <param name="log">Updated progress log values posted from the form.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ProgressLogId,EntryDate,Weight,BodyFatPercent,Notes")]
            ProgressLog log)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            if (id != log.ProgressLogId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(log);

            var existing = await _db.ProgressLogs
                .FirstOrDefaultAsync(p => p.ProgressLogId == id
                                          && p.UserId == userId.Value);

            if (existing == null) return NotFound();

            // Update only allowed fields
            existing.EntryDate = log.EntryDate;
            existing.Weight = log.Weight;
            existing.BodyFatPercent = log.BodyFatPercent;
            existing.Notes = log.Notes;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Deletes a progress log that belongs to the current user.
        /// </summary>
        /// <param name="id">ID of the log to delete.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var log = await _db.ProgressLogs
                .FirstOrDefaultAsync(p => p.ProgressLogId == id
                                          && p.UserId == userId.Value);

            if (log == null) return NotFound();

            _db.ProgressLogs.Remove(log);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
