using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Models
{
    /// <summary>
    /// Represents an exercise associated with a workout, at a summary level.
    /// Currently not used in the controller and view code you shared.
    /// </summary>
    public class WorkoutExercise
    {
        public int WorkoutExerciseId { get; set; }

        public int WorkoutId { get; set; }
        public Workout? Workout { get; set; }

        [Required, StringLength(40)]
        public string ExerciseName { get; set; } = string.Empty;

        public int Sets { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }
    }
}
