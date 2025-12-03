namespace FitnessTracker.Models
{
    /// <summary>
    /// Represents a single exercise inside a global workout template.
    /// </summary>
    public class WorkoutTemplateExercise
    {
        /// <summary>
        /// Primary key for the template exercise.
        /// </summary>
        public int WorkoutTemplateExerciseId { get; set; }

        /// <summary>
        /// Foreign key to the parent workout template.
        /// </summary>
        public int WorkoutTemplateId { get; set; }

        /// <summary>
        /// Navigation property to the parent workout template.
        /// </summary>
        public WorkoutTemplate? Template { get; set; }

        /// <summary>
        /// Name of the exercise (e.g. "Squat", "Bench Press").
        /// </summary>
        public string ExerciseName { get; set; } = string.Empty;

        /// <summary>
        /// Number of sets defined for this exercise in the template.
        /// </summary>
        public int Sets { get; set; }

        /// <summary>
        /// Number of reps per set defined for this exercise.
        /// </summary>
        public int Reps { get; set; }

        /// <summary>
        /// Suggested weight for this exercise.
        /// </summary>
        public double Weight { get; set; }
    }
}
