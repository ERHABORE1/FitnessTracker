using FitnessTracker.Models;
using FitnessTracker.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Controllers
{
    /// <summary>
    /// MVC controller responsible for the workout pages: workout list, edit/details pages,
    /// trainer-assigned workouts, and saving per-set data.
    /// </summary>
    public class WorkoutController : Controller
    {
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// Creates a new instance of <see cref="WorkoutController"/>.
        /// </summary>
        /// <param name="db">Application database context used to query and modify workouts.</param>
        public WorkoutController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Reads the current user's ID from the session. Returns null if not logged in.
        /// </summary>
        /// <returns>The numeric user ID, or null if the session is missing or invalid.</returns>
        private int? GetUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            return string.IsNullOrWhiteSpace(userIdString)
                ? (int?)null
                : int.Parse(userIdString);
        }

        /// <summary>
        /// Displays the main workout page for a normal "User" role.
        /// Also shows any pending trainer requests so the user sees them in context of workouts.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Auth");

            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var pendingRequests = await _db.TrainerClientRequests
                .Include(r => r.Trainer)
                .Where(r => r.ClientId == userId.Value && r.Status == "Pending")
                .ToListAsync();

            ViewBag.PendingTrainerRequests = pendingRequests;

            return View();
        }

        /// <summary>
        /// Displays a simple edit page for a single workout that belongs to the current user.
        /// </summary>
        /// <param name="id">The ID of the workout to edit.</param>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var workout = await _db.Workouts
                .FirstOrDefaultAsync(w => w.WorkoutId == id && w.UserId == userId.Value);

            if (workout == null)
                return NotFound();

            return View(workout);
        }

        /// <summary>
        /// Displays a read-only details page for a workout that belongs to the current user.
        /// </summary>
        /// <param name="id">The ID of the workout to display.</param>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var workout = await _db.Workouts
                .FirstOrDefaultAsync(w => w.WorkoutId == id && w.UserId == userId.Value);

            if (workout == null)
                return NotFound();

            return View(workout);
        }

        /// <summary>
        /// Lists all trainer-assigned workouts for the current user.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Assigned()
        {
            if (HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Auth");

            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var assigned = await _db.TrainerAssignedWorkouts
                .Include(a => a.Trainer)
                .Include(a => a.Template)
                .Where(a => a.ClientId == userId.Value)
                .OrderByDescending(a => a.AssignedDate)
                .ToListAsync();

            return View(assigned);
        }

        /// <summary>
        /// Displays a per-set logging UI for a specific trainer-assigned workout.
        /// The view uses the template exercises to create input fields for each set.
        /// </summary>
        /// <param name="assignedId">ID of the trainer assignment to log.</param>
        [HttpGet]
        public async Task<IActionResult> AssignedEntry(int assignedId)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var assigned = await _db.TrainerAssignedWorkouts
                .Include(a => a.Template)
                    .ThenInclude(t => t.Exercises)
                .Include(a => a.Trainer)
                .FirstOrDefaultAsync(a =>
                    a.TrainerAssignedWorkoutId == assignedId &&
                    a.ClientId == userId.Value);

            if (assigned == null)
                return NotFound();

            return View(assigned);
        }

        /// <summary>
        /// Saves per-set data entered for a trainer-assigned workout into a normal Workout record
        /// plus multiple WorkoutSet records. Also marks the assignment as completed.
        /// </summary>
        /// <param name="AssignedId">The ID of the trainer assignment being completed.</param>
        /// <param name="Notes">Optional free-text notes from the user about the workout.</param>
        [HttpPost]
        public IActionResult SaveAssigned(int AssignedId, string Notes)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var assigned = _db.TrainerAssignedWorkouts
                .Include(a => a.Template)
                    .ThenInclude(t => t.Exercises)
                .FirstOrDefault(a =>
                    a.TrainerAssignedWorkoutId == AssignedId &&
                    a.ClientId == userId.Value);

            if (assigned == null)
                return BadRequest("Assigned workout not found.");

            int totalSets = 0;
            int totalReps = 0;

            // Create the main Workout record
            var workout = new Workout
            {
                UserId = userId.Value,
                Date = DateTime.Today,
                WorkoutStyle = assigned.Template?.TemplateName ?? "Trainer Workout",
                TotalSets = 0,  // will be updated after calculating
                TotalReps = 0,
                Notes = Notes
            };

            _db.Workouts.Add(workout);
            _db.SaveChanges(); // generate WorkoutId

            // Loop through template exercises and read per-set form values from the request
            if (assigned.Template?.Exercises != null)
            {
                foreach (var ex in assigned.Template.Exercises)
                {
                    for (int i = 1; i <= ex.Sets; i++)
                    {
                        string repField = $"SetReps_{ex.ExerciseName}_{i}";
                        string weightField = $"SetWeight_{ex.ExerciseName}_{i}";

                        int reps = 0;
                        double weight = 0;

                        if (Request.Form.ContainsKey(repField))
                            int.TryParse(Request.Form[repField], out reps);

                        if (Request.Form.ContainsKey(weightField))
                            double.TryParse(Request.Form[weightField], out weight);

                        totalSets++;
                        totalReps += reps;

                        _db.WorkoutSets.Add(new WorkoutSet
                        {
                            WorkoutId = workout.WorkoutId,
                            ExerciseName = ex.ExerciseName,
                            SetNumber = i,
                            Reps = reps,
                            Weight = weight
                        });
                    }
                }
            }

            // Update totals on the workout
            workout.TotalSets = totalSets;
            workout.TotalReps = totalReps;

            // Mark assignment as completed so the trainer knows it was finished
            assigned.IsCompleted = true;
            assigned.CompletedDate = DateTime.Now;

            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// (Optional helper) Loads an assigned trainer template and pre-fills the regular Create view
        /// so the user can save it as a normal workout instead of using the per-set UI.
        /// </summary>
        /// <param name="id">The ID of the trainer assignment whose template should be used.</param>
        [HttpGet]
        public async Task<IActionResult> UseTemplate(int id)
        {
            if (HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Auth");

            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var assigned = await _db.TrainerAssignedWorkouts
                .Include(a => a.Template)
                    .ThenInclude(t => t.Exercises)
                .FirstOrDefaultAsync(a =>
                    a.TrainerAssignedWorkoutId == id &&
                    a.ClientId == userId.Value);

            if (assigned == null)
                return NotFound();

            var workoutStyle = assigned.Template?.TemplateName ?? "Trainer Workout";
            var totalSets = assigned.Template?.Exercises?.Sum(e => e.Sets) ?? 0;
            var totalReps = assigned.Template?.Exercises?.Sum(e => e.Sets * e.Reps) ?? 0;

            var prefill = new Workout
            {
                WorkoutStyle = workoutStyle,
                TotalSets = totalSets,
                TotalReps = totalReps,
                Notes = $"Trainer template used: {assigned.Template?.TemplateName}"
            };

            ViewBag.FromTemplate = true;
            ViewBag.TemplateName = assigned.Template?.TemplateName;

            return View("Create", prefill);
        }
    }
}
