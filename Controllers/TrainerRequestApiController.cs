using FitnessTracker.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Controllers.Api
{
    /// <summary>
    /// REST API controller that exposes trainer access requests for the logged-in client.
    /// Used by AJAX calls to get pending requests and respond to them without reloading pages.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TrainerRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Creates a new instance of <see cref="TrainerRequestController"/>.
        /// </summary>
        /// <param name="db">Application database context used to query trainer requests.</param>
        /// <param name="httpContextAccessor">Accessor used to read the current HTTP session.</param>
        public TrainerRequestController(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Reads the current logged-in user ID from the session.
        /// Returns 0 if the user cannot be determined.
        /// </summary>
        /// <returns>The numeric user ID or 0 when not logged in.</returns>
        private int GetCurrentUserId()
        {
            var idString = _httpContextAccessor.HttpContext?.Session.GetString("UserId");

            return int.TryParse(idString, out var id) ? id : 0;
        }

        /// <summary>
        /// Returns pending trainer access requests for the currently logged-in client.
        /// </summary>
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized();
            }

            var requests = await _db.TrainerClientRequests
                .Where(r => r.ClientId == userId && r.Status == "Pending")
                .Include(r => r.Trainer)
                .OrderByDescending(r => r.SentDate)
                .Select(r => new
                {
                    r.TrainerClientRequestId,
                    TrainerName = r.Trainer.Name,
                    r.SentDate
                })
                .ToListAsync();

            return Ok(requests);
        }

        /// <summary>
        /// DTO used when the client responds to a trainer request via the API.
        /// </summary>
        public class RespondDto
        {
            /// <summary>
            /// Primary key of the trainer request being responded to.
            /// </summary>
            public int RequestId { get; set; }

            /// <summary>
            /// Decision string ("accept" or "decline").
            /// </summary>
            public string Decision { get; set; } = "";
        }

        /// <summary>
        /// Handles a client's decision on a pending trainer request ("accept" or "decline").
        /// </summary>
        /// <param name="dto">DTO containing the request ID and the decision value.</param>
        [HttpPost("respond")]
        public async Task<IActionResult> Respond([FromForm] RespondDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized();
            }

            var request = await _db.TrainerClientRequests
                .FirstOrDefaultAsync(r => r.TrainerClientRequestId == dto.RequestId && r.ClientId == userId);

            if (request == null)
            {
                return NotFound();
            }

            if (request.Status != "Pending")
            {
                return BadRequest("Request already handled.");
            }

            if (dto.Decision.Equals("accept", StringComparison.OrdinalIgnoreCase))
            {
                request.Status = "Accepted";
                // Optional: create a permanent Trainer↔Client relationship in a separate table here
            }
            else if (dto.Decision.Equals("decline", StringComparison.OrdinalIgnoreCase))
            {
                request.Status = "Declined";
            }
            else
            {
                return BadRequest("Invalid decision.");
            }

            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
