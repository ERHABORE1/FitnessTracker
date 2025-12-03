using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Models
{
    /// <summary>
    /// Represents a reusable workout template created by a user (usually a trainer).
    /// </summary>
    public class UserWorkoutTemplate
    {
        /// <summary>
        /// Primary key for the user workout template.
        /// </summary>
        public int UserWorkoutTemplateId { get; set; }

        /// <summary>
        /// Foreign key to the user who owns this template.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
            /// Navigation property to the owning user.
            /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Display name of the template (e.g. "Leg Day Power").
        /// </summary>
        [Required, StringLength(50)]
        public string TemplateName { get; set; } = string.Empty;

        /// <summary>
        /// Collection of exercises that belong to this template.
        /// </summary>
        public List<UserWorkoutTemplateExercise>? Exercises { get; set; }
    }
}
