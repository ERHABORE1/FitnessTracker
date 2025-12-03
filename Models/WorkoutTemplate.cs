using System.Collections.Generic;

namespace FitnessTracker.Models
{
    /// <summary>
    /// Represents a global workout template that can be assigned by trainers to clients.
    /// </summary>
    public class WorkoutTemplate
    {
        /// <summary>
        /// Primary key for the workout template.
        /// </summary>
        public int WorkoutTemplateId { get; set; }

        /// <summary>
        /// Display name of the template.
        /// </summary>
        public string TemplateName { get; set; } = string.Empty;

        /// <summary>
        /// Optional category label (e.g. "Legs", "Cardio", "Strength").
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Collection of exercises that belong to this template.
        /// </summary>
        public List<WorkoutTemplateExercise>? Exercises { get; set; }
    }
}
