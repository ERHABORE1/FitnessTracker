using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Models
{
    /// <summary>
    /// Represents a completed workout session for a user, including totals
    /// and optional per-set details through <see cref="WorkoutSet"/>.
    /// </summary>
    public class Workout
    {
        /// <summary>
        /// Primary key for the workout.
        /// </summary>
        public int WorkoutId { get; set; }

        /// <summary>
        /// Foreign key to the user who performed the workout.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Navigation property to the owning user.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Date when the workout took place.
        /// </summary>
        [Required]
        public DateTime Date { get; set; } = DateTime.Today;

        /// <summary>
        /// Short description of the workout (e.g. "Leg Day", "Upper Body").
        /// </summary>
        [Required, StringLength(40)]
        public string WorkoutStyle { get; set; } = string.Empty;

        /// <summary>
        /// Optional total duration of the workout in minutes.
        /// </summary>
        public int? DurationMinutes { get; set; }

        /// <summary>
        /// Total number of sets completed in the workout.
        /// </summary>
        public int? TotalSets { get; set; }

        /// <summary>
        /// Total number of repetitions completed in the workout.
        /// </summary>
        public int? TotalReps { get; set; }

        /// <summary>
        /// Optional aggregate weight field, if you choose to track it.
        /// </summary>
        public double? Weight { get; set; }

        /// <summary>
        /// Free-text notes about the workout.
        /// </summary>
        [StringLength(250)]
        public string? Notes { get; set; }

        /// <summary>
        /// Collection of per-set details captured for this workout.
        /// </summary>
        public List<WorkoutSet>? Sets { get; set; }
    }
}
