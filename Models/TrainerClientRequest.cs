using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Models
{
    /// <summary>
    /// Represents a request from a trainer to a client asking for access
    /// to view and manage the client's workouts and progress.
    /// </summary>
    public class TrainerClientRequest
    {
        /// <summary>
        /// Primary key for the trainer-client request record.
        /// </summary>
        public int TrainerClientRequestId { get; set; }

        /// <summary>
        /// Foreign key to the trainer who sent the request.
        /// </summary>
        public int TrainerId { get; set; }

        /// <summary>
        /// Navigation property to the trainer user.
        /// </summary>
        public User? Trainer { get; set; }

        /// <summary>
        /// Foreign key to the client who receives the request.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Navigation property to the client user.
        /// </summary>
        public User? Client { get; set; }

        /// <summary>
        /// Current status of the request: "Pending", "Accepted", or "Declined".
        /// </summary>
        [Required]
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Timestamp indicating when the request was sent.
        /// </summary>
        public DateTime SentDate { get; set; } = DateTime.UtcNow;
    }
}
