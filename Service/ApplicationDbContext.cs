using FitnessTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Service
{
    /// <summary>
    /// Entity Framework Core database context for the Fitness Tracker application.
    /// Defines the entity sets and configures relationships and seed data.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="ApplicationDbContext"/> using the given options.
        /// </summary>
        /// <param name="options">Options used to configure the DbContext.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ============================
        // ENTITY SETS
        // ============================

        /// <summary>
        /// Users of the application (clients and trainers).
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Completed workout sessions logged by users.
        /// </summary>
        public DbSet<Workout> Workouts { get; set; }

        /// <summary>
        /// Exercise summary records for workouts.
        /// Currently not used by the main workflow which relies on <see cref="WorkoutSet"/>.
        /// </summary>
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }

        /// <summary>
        /// Per-set details logged for each workout.
        /// </summary>
        public DbSet<WorkoutSet> WorkoutSets { get; set; }

        /// <summary>
        /// Global workout templates available for assignment.
        /// </summary>
        public DbSet<WorkoutTemplate> WorkoutTemplates { get; set; }

        /// <summary>
        /// Exercises that belong to global workout templates.
        /// </summary>
        public DbSet<WorkoutTemplateExercise> WorkoutTemplateExercises { get; set; }

        /// <summary>
        /// User-defined workout templates (typically created by trainers).
        /// </summary>
        public DbSet<UserWorkoutTemplate> UserWorkoutTemplates { get; set; }

        /// <summary>
        /// Exercises that belong to user-defined workout templates.
        /// </summary>
        public DbSet<UserWorkoutTemplateExercise> UserWorkoutTemplateExercises { get; set; }

        /// <summary>
        /// Records of workout templates that trainers assign to clients.
        /// </summary>
        public DbSet<TrainerAssignedWorkout> TrainerAssignedWorkouts { get; set; }

        /// <summary>
        /// Requests from trainers asking clients for access to their data.
        /// </summary>
        public DbSet<TrainerClientRequest> TrainerClientRequests { get; set; }

        /// <summary>
        /// Progress logs for users, including weight and optional body fat data.
        /// </summary>
        public DbSet<ProgressLog> ProgressLogs { get; set; } = default!;

        /// <summary>
        /// Configures entity relationships and seeds initial data for templates.
        /// </summary>
        /// <param name="modelBuilder">Model builder used to configure EF Core metadata.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================
            // USER WORKOUT TEMPLATE RELATIONS
            // ============================
            // One user → many user workout templates
            modelBuilder.Entity<UserWorkoutTemplate>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One user workout template → many template exercises
            modelBuilder.Entity<UserWorkoutTemplateExercise>()
                .HasOne(e => e.Template)
                .WithMany(t => t.Exercises)
                .HasForeignKey(e => e.UserWorkoutTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================
            // TRAINER ASSIGNMENT RELATIONS
            // ============================
            // TrainerAssignedWorkout ↔ Trainer (User with Role = Trainer)
            modelBuilder.Entity<TrainerAssignedWorkout>()
                .HasOne(t => t.Trainer)
                .WithMany()
                .HasForeignKey(t => t.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            // TrainerAssignedWorkout ↔ Client (User with Role = User)
            modelBuilder.Entity<TrainerAssignedWorkout>()
                .HasOne(t => t.Client)
                .WithMany()
                .HasForeignKey(t => t.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // TrainerAssignedWorkout ↔ WorkoutTemplate
            modelBuilder.Entity<TrainerAssignedWorkout>()
                .HasOne(t => t.Template)
                .WithMany()
                .HasForeignKey(t => t.WorkoutTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================
            // WORKOUT LOGGING RELATIONS
            // ============================
            // WorkoutExercise → Workout (summary-level exercise records)
            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(e => e.Workout)
                .WithMany()
                .HasForeignKey(e => e.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================
            // TRAINER ↔ CLIENT REQUESTS
            // ============================
            // TrainerClientRequest → Trainer
            modelBuilder.Entity<TrainerClientRequest>()
                .HasOne(r => r.Trainer)
                .WithMany()
                .HasForeignKey(r => r.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            // TrainerClientRequest → Client
            modelBuilder.Entity<TrainerClientRequest>()
                .HasOne(r => r.Client)
                .WithMany()
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ====================================================
            // SEED DEFAULT GLOBAL TEMPLATES
            // ====================================================
            modelBuilder.Entity<WorkoutTemplate>().HasData(
                new WorkoutTemplate { WorkoutTemplateId = 1001, TemplateName = "Biceps Workout",  Category = "Biceps"  },
                new WorkoutTemplate { WorkoutTemplateId = 1002, TemplateName = "Triceps Workout", Category = "Triceps" },
                new WorkoutTemplate { WorkoutTemplateId = 1003, TemplateName = "Back Workout",    Category = "Back"    },
                new WorkoutTemplate { WorkoutTemplateId = 1004, TemplateName = "Leg Workout",     Category = "Legs"    }
            );

            modelBuilder.Entity<WorkoutTemplateExercise>().HasData(
                // -------- BICEPS --------
                new WorkoutTemplateExercise { WorkoutTemplateExerciseId = 2001, WorkoutTemplateId = 1001, ExerciseName = "Dumbbell Bicep Curl",      Sets = 3, Reps = 10, Weight = 0 },
                new WorkoutTemplateExercise { WorkoutTemplateExerciseId = 2002, WorkoutTemplateId = 1001, ExerciseName = "Hammer Curl",             Sets = 3, Reps = 12, Weight = 0 },

                // -------- TRICEPS --------
                new WorkoutTemplateExercise { WorkoutTemplateExerciseId = 2003, WorkoutTemplateId = 1002, ExerciseName = "Tricep Cable Pushdown",   Sets = 3, Reps = 12, Weight = 0 },
                new WorkoutTemplateExercise { WorkoutTemplateExerciseId = 2004, WorkoutTemplateId = 1002, ExerciseName = "Overhead Dumbbell Extension", Sets = 3, Reps = 10, Weight = 0 },

                // -------- BACK --------
                new WorkoutTemplateExercise { WorkoutTemplateExerciseId = 2005, WorkoutTemplateId = 1003, ExerciseName = "Lat Pulldown",            Sets = 3, Reps = 10, Weight = 0 },
                new WorkoutTemplateExercise { WorkoutTemplateExerciseId = 2006, WorkoutTemplateId = 1003, ExerciseName = "Seated Cable Row",        Sets = 3, Reps = 12, Weight = 0 },

                // -------- LEGS --------
                new WorkoutTemplateExercise { WorkoutTemplateExerciseId = 2007, WorkoutTemplateId = 1004, ExerciseName = "Leg Press",               Sets = 3, Reps = 10, Weight = 0 },
                new WorkoutTemplateExercise { WorkoutTemplateExerciseId = 2008, WorkoutTemplateId = 1004, ExerciseName = "Goblet Squat",            Sets = 3, Reps = 12, Weight = 0 }
            );
        }
    }
}
