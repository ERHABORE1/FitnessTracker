using FitnessTracker.Models;
using FitnessTracker.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Controllers
{
    /// <summary>
    /// REST API for workout-related operations.
    /// Supports CRUD for workouts that belong to the currently logged-in user.
    /// This controller is consumed via JavaScript/AJAX on the Workout page.
    /// </summary>
    [Route("api/workout")]
    [ApiController]
    public class WorkoutApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// Creates a new instance of <see cref="WorkoutApiController"/>.
        /// </summary>
        /// <param name="db">Application database context used to query and modify workout data.</param>
        public WorkoutApiController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Reads the current user's ID from the session.
        /// Returns <c>null</c> if the user is not logged in.
        /// </summary>
        /// <returns>The numeric user ID, or <c>null</c> if the session is not set.</returns>
        private int? GetUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            return string.IsNullOrWhiteSpace(userIdString)
                ? (int?)null
                : int.Parse(userIdString);
        }

        /// <summary>
        /// Returns all workouts that belong to the current user,
        /// ordered from most recent to oldest.
        /// This endpoint is used by the Workout page to render the workout cards.
        /// </summary>
        /// <returns>HTTP 401 if not logged in, otherwise a list of workouts.</returns>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Workout>>> GetAll()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var workouts = await _db.Workouts
                .Where(w => w.UserId == userId.Value)
                .OrderByDescending(w => w.Date)
                .ToListAsync();

            return Ok(workouts);
        }

        /// <summary>
        /// Data Transfer Object used for creating and updating workout
        /// records via the API from the Workout page.
        /// </summary>
        public class WorkoutDto
        {
            /// <summary>
            /// Primary key of the workout (used when updating an existing workout).
            /// </summary>
            public int WorkoutId { get; set; }

            /// <summary>
            /// Free-text description of the workout style (e.g. "Leg day", "Cardio").
            /// </summary>
            public string WorkoutStyle { get; set; } = string.Empty;

            /// <summary>
            /// Approximate duration in minutes, if known.
            /// </summary>
            public int? DurationMinutes { get; set; }

            /// <summary>
            /// Total number of sets completed in the workout.
            /// </summary>
            public int? TotalSets { get; set; }

            /// <summary>
            /// Total number of reps completed in the workout.
            /// </summary>
            public int? TotalReps { get; set; }

            /// <summary>
            /// Additional notes or comments about the workout.
            /// </summary>
            public string? Notes { get; set; }
        }

        /// <summary>
        /// Creates a new workout for the current user based on the posted DTO.
        /// The workout date is set to today's date on the server.
        /// This is called when the user submits the "Add Workout" form.
        /// </summary>
        /// <param name="dto">Workout data posted from the client.</param>
        /// <returns>The created workout as JSON, or HTTP 401 if not logged in.</returns>
        [HttpPost("create")]
        public async Task<ActionResult<Workout>> Create([FromForm] WorkoutDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var workout = new Workout
            {
                UserId = userId.Value,
                Date = DateTime.Today,
                WorkoutStyle = dto.WorkoutStyle,
                DurationMinutes = dto.DurationMinutes,
                TotalSets = dto.TotalSets,
                TotalReps = dto.TotalReps,
                Notes = dto.Notes
            };

            _db.Workouts.Add(workout);
            await _db.SaveChangesAsync();

            return Ok(workout);
        }

        /// <summary>
        /// Retrieves a single workout by ID for the current user.
        /// Used by the Workout page when the user clicks "Edit"
        /// on a specific workout card.
        /// </summary>
        /// <param name="id">The ID of the workout to retrieve.</param>
        /// <returns>The workout as JSON, HTTP 401 if not logged in, or 404 if not found.</returns>
        [HttpGet("one/{id}")]
        public async Task<ActionResult<Workout>> GetOne(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var workout = await _db.Workouts
                .FirstOrDefaultAsync(w => w.WorkoutId == id && w.UserId == userId.Value);

            if (workout == null)
                return NotFound();

            return Ok(workout);
        }

        /// <summary>
        /// Updates an existing workout for the current user using the provided DTO.
        /// This endpoint is called when the user edits a workout and submits the form.
        /// </summary>
        /// <param name="dto">
        /// Workout DTO containing updated values. 
        /// The <see cref="WorkoutDto.WorkoutId"/> property identifies the record to update.
        /// </param>
        /// <returns>HTTP 204 on success, 401 if not logged in, or 404 if the workout is not found.</returns>
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] WorkoutDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var workout = await _db.Workouts
                .FirstOrDefaultAsync(w => w.WorkoutId == dto.WorkoutId && w.UserId == userId.Value);

            if (workout == null)
                return NotFound();

            workout.WorkoutStyle = dto.WorkoutStyle;
            workout.DurationMinutes = dto.DurationMinutes;
            workout.TotalSets = dto.TotalSets;
            workout.TotalReps = dto.TotalReps;
            workout.Notes = dto.Notes;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes an existing workout that belongs to the current user.
        /// This endpoint is called when the user clicks "Delete" on a workout card.
        /// </summary>
        /// <param name="id">ID of the workout to delete.</param>
        /// <returns>HTTP 204 on success, 401 if not logged in, or 404 if the workout is not found.</returns>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var workout = await _db.Workouts
                .FirstOrDefaultAsync(w => w.WorkoutId == id && w.UserId == userId.Value);

            if (workout == null)
                return NotFound();

            _db.Workouts.Remove(workout);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
