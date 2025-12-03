using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessTracker.Models
{
    /// <summary>
    /// Represents an application user (client or trainer), including
    /// authentication and role information.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Primary key for the user record.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Display name of the user.
        /// </summary>
        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email address used for login and contact.
        /// </summary>
        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Stored password hash used for authentication.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Plain-text password used during registration and validation only.
        /// Not mapped to the database.
        /// </summary>
        [NotMapped]
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Confirmation password field to ensure the user types the same password twice.
        /// Not mapped to the database.
        /// </summary>
        [NotMapped]
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Role assigned to the user (e.g. "User" or "Trainer").
        /// </summary>
        [StringLength(20)]
        public string Role { get; set; } = "User";

        /// <summary>
        /// Navigation property for workouts owned by this user.
        /// </summary>
        public List<Workout>? Workouts { get; set; }

        /// <summary>
        /// Navigation property for user-defined workout templates.
        /// </summary>
        public List<UserWorkoutTemplate>? SavedTemplates { get; set; }

        /// <summary>
        /// Navigation property for trainer-assigned workouts where this user is the client.
        /// </summary>
        public List<TrainerAssignedWorkout>? AssignedWorkouts { get; set; }
    }
}
