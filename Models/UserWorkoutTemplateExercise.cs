namespace FitnessTracker.Models
{
    /// <summary>
    /// Represents a single exercise inside a user workout template.
    /// </summary>
    public class UserWorkoutTemplateExercise
    {
        /// <summary>
        /// Primary key for the template exercise.
        /// </summary>
        public int UserWorkoutTemplateExerciseId { get; set; }

        /// <summary>
        /// Foreign key to the parent user workout template.
        /// </summary>
        public int UserWorkoutTemplateId { get; set; }

        /// <summary>
        /// Navigation property to the parent template.
        /// </summary>
        public UserWorkoutTemplate? Template { get; set; }

        /// <summary>
        /// Name of the exercise (e.g. "Bench Press").
        /// </summary>
        public string ExerciseName { get; set; } = string.Empty;

        /// <summary>
        /// Number of sets for this exercise in the template.
        /// </summary>
        public int Sets { get; set; }

        /// <summary>
        /// Number of reps per set for this exercise.
        /// </summary>
        public int Reps { get; set; }

        /// <summary>
        /// Suggested weight for this exercise in the template.
        /// </summary>
        public double Weight { get; set; }
    }
}
