using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Models
{
    /// <summary>
    /// Represents a single set performed in a workout, including reps and weight.
    /// </summary>
    public class WorkoutSet
    {
        /// <summary>
        /// Primary key for the workout set.
        /// </summary>
        public int WorkoutSetId { get; set; }

        /// <summary>
        /// Foreign key to the parent workout.
        /// </summary>
        public int WorkoutId { get; set; }

        /// <summary>
        /// Navigation property to the parent workout.
        /// </summary>
        public Workout? Workout { get; set; }

        /// <summary>
        /// Name of the exercise this set belongs to.
        /// </summary>
        [Required]
        public string ExerciseName { get; set; } = string.Empty;

        /// <summary>
        /// Ordinal number of the set for the given exercise (1-based).
        /// </summary>
        public int SetNumber { get; set; }

        /// <summary>
        /// Number of repetitions completed in this set.
        /// </summary>
        public int Reps { get; set; }

        /// <summary>
        /// Weight used for this set.
        /// </summary>
        public double Weight { get; set; }
    }
}
