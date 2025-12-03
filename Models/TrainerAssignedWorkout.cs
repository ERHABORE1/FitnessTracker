namespace FitnessTracker.Models
{
    /// <summary>
    /// Represents a workout template that a trainer has assigned to a client
    /// for a specific date, with completion tracking.
    /// </summary>
    public class TrainerAssignedWorkout
    {
        /// <summary>
        /// Primary key for the trainer assignment record.
        /// </summary>
        public int TrainerAssignedWorkoutId { get; set; }

        /// <summary>
        /// Foreign key to the trainer who assigned the workout.
        /// </summary>
        public int TrainerId { get; set; }

        /// <summary>
        /// Navigation property to the trainer user.
        /// </summary>
        public User? Trainer { get; set; }

        /// <summary>
        /// Foreign key to the client who receives the workout.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Navigation property to the client user.
        /// </summary>
        public User? Client { get; set; }

        /// <summary>
        /// Date when the workout was assigned.
        /// </summary>
        public DateTime AssignedDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Foreign key to the workout template being assigned.
        /// </summary>
        public int WorkoutTemplateId { get; set; }

        /// <summary>
        /// Navigation property to the assigned workout template.
        /// </summary>
        public WorkoutTemplate? Template { get; set; }

        /// <summary>
        /// Indicates whether the client has completed this assignment.
        /// </summary>
        public bool IsCompleted { get; set; } = false;

        /// <summary>
        /// Date when the assignment was completed, if applicable.
        /// </summary>
        public DateTime? CompletedDate { get; set; }
    }
}
