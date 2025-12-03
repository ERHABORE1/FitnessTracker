using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Models
{
    /// <summary>
    /// Represents a single progress entry for a user, including weight,
    /// optional body fat %, notes, and trainer feedback.
    /// </summary>
    public class ProgressLog
    {
        /// <summary>
        /// Primary key for the progress log entry.
        /// </summary>
        public int ProgressLogId { get; set; }

        /// <summary>
        /// Foreign key to the user who owns this progress entry.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Navigation property to the owning user.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Date of the progress entry.
        /// </summary>
        [Required]
        [Display(Name = "Date")]
        public DateTime EntryDate { get; set; } = DateTime.Today;

        /// <summary>
        /// User's body weight in pounds at the time of the entry.
        /// </summary>
        [Required]
        [Range(1, 1000)]
        [Display(Name = "Weight (lbs)")]
        public double Weight { get; set; }

        /// <summary>
        /// Optional body fat percentage for this entry.
        /// </summary>
        [Range(1, 100)]
        [Display(Name = "Body Fat %")]
        public double? BodyFatPercent { get; set; }

        /// <summary>
        /// Optional notes written by the user about this entry.
        /// </summary>
        [StringLength(250)]
        public string? Notes { get; set; }

        /// <summary>
        /// Optional feedback left by the trainer for this entry.
        /// </summary>
        [StringLength(250)]
        public string? TrainerFeedback { get; set; }
    }
}
