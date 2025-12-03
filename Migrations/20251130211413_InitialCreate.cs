using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitnessTracker.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutTemplates",
                columns: table => new
                {
                    WorkoutTemplateId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TemplateName = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    CreatorId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutTemplates", x => x.WorkoutTemplateId);
                });

            migrationBuilder.CreateTable(
                name: "TrainerClientRequests",
                columns: table => new
                {
                    TrainerClientRequestId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TrainerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    SentDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerClientRequests", x => x.TrainerClientRequestId);
                    table.ForeignKey(
                        name: "FK_TrainerClientRequests_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainerClientRequests_Users_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserWorkoutTemplates",
                columns: table => new
                {
                    UserWorkoutTemplateId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    TemplateName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UserId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWorkoutTemplates", x => x.UserWorkoutTemplateId);
                    table.ForeignKey(
                        name: "FK_UserWorkoutTemplates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWorkoutTemplates_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Workouts",
                columns: table => new
                {
                    WorkoutId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WorkoutStyle = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: true),
                    TotalSets = table.Column<int>(type: "INTEGER", nullable: true),
                    TotalReps = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workouts", x => x.WorkoutId);
                    table.ForeignKey(
                        name: "FK_Workouts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainerAssignedWorkouts",
                columns: table => new
                {
                    TrainerAssignedWorkoutId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TrainerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WorkoutTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerAssignedWorkouts", x => x.TrainerAssignedWorkoutId);
                    table.ForeignKey(
                        name: "FK_TrainerAssignedWorkouts_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainerAssignedWorkouts_Users_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainerAssignedWorkouts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_TrainerAssignedWorkouts_WorkoutTemplates_WorkoutTemplateId",
                        column: x => x.WorkoutTemplateId,
                        principalTable: "WorkoutTemplates",
                        principalColumn: "WorkoutTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutTemplateExercises",
                columns: table => new
                {
                    WorkoutTemplateExerciseId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkoutTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExerciseName = table.Column<string>(type: "TEXT", nullable: false),
                    Sets = table.Column<int>(type: "INTEGER", nullable: false),
                    Reps = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutTemplateExercises", x => x.WorkoutTemplateExerciseId);
                    table.ForeignKey(
                        name: "FK_WorkoutTemplateExercises_WorkoutTemplates_WorkoutTemplateId",
                        column: x => x.WorkoutTemplateId,
                        principalTable: "WorkoutTemplates",
                        principalColumn: "WorkoutTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWorkoutTemplateExercises",
                columns: table => new
                {
                    UserWorkoutTemplateExerciseId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserWorkoutTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExerciseName = table.Column<string>(type: "TEXT", nullable: false),
                    Sets = table.Column<int>(type: "INTEGER", nullable: false),
                    Reps = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWorkoutTemplateExercises", x => x.UserWorkoutTemplateExerciseId);
                    table.ForeignKey(
                        name: "FK_UserWorkoutTemplateExercises_UserWorkoutTemplates_UserWorkoutTemplateId",
                        column: x => x.UserWorkoutTemplateId,
                        principalTable: "UserWorkoutTemplates",
                        principalColumn: "UserWorkoutTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutExercises",
                columns: table => new
                {
                    WorkoutExerciseId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkoutId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExerciseName = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Sets = table.Column<int>(type: "INTEGER", nullable: false),
                    Reps = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<double>(type: "REAL", nullable: false),
                    WorkoutId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutExercises", x => x.WorkoutExerciseId);
                    table.ForeignKey(
                        name: "FK_WorkoutExercises_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "WorkoutId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutExercises_Workouts_WorkoutId1",
                        column: x => x.WorkoutId1,
                        principalTable: "Workouts",
                        principalColumn: "WorkoutId");
                });

            migrationBuilder.InsertData(
                table: "WorkoutTemplates",
                columns: new[] { "WorkoutTemplateId", "Category", "CreatorId", "TemplateName" },
                values: new object[,]
                {
                    { 1001, "Beginner", null, "Full Body Starter" },
                    { 1002, "Split", null, "Push / Pull / Legs" }
                });

            migrationBuilder.InsertData(
                table: "WorkoutTemplateExercises",
                columns: new[] { "WorkoutTemplateExerciseId", "ExerciseName", "Reps", "Sets", "Weight", "WorkoutTemplateId" },
                values: new object[,]
                {
                    { 2001, "Squat", 10, 3, 0.0, 1001 },
                    { 2002, "Bench Press", 10, 3, 0.0, 1001 },
                    { 2003, "Push Day (Chest/Shoulders)", 8, 4, 0.0, 1002 },
                    { 2004, "Pull Day (Back)", 8, 4, 0.0, 1002 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainerAssignedWorkouts_ClientId",
                table: "TrainerAssignedWorkouts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerAssignedWorkouts_TrainerId",
                table: "TrainerAssignedWorkouts",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerAssignedWorkouts_UserId",
                table: "TrainerAssignedWorkouts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerAssignedWorkouts_WorkoutTemplateId",
                table: "TrainerAssignedWorkouts",
                column: "WorkoutTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerClientRequests_ClientId",
                table: "TrainerClientRequests",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerClientRequests_TrainerId",
                table: "TrainerClientRequests",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkoutTemplateExercises_UserWorkoutTemplateId",
                table: "UserWorkoutTemplateExercises",
                column: "UserWorkoutTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkoutTemplates_UserId",
                table: "UserWorkoutTemplates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkoutTemplates_UserId1",
                table: "UserWorkoutTemplates",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_WorkoutId",
                table: "WorkoutExercises",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_WorkoutId1",
                table: "WorkoutExercises",
                column: "WorkoutId1");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_UserId",
                table: "Workouts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutTemplateExercises_WorkoutTemplateId",
                table: "WorkoutTemplateExercises",
                column: "WorkoutTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainerAssignedWorkouts");

            migrationBuilder.DropTable(
                name: "TrainerClientRequests");

            migrationBuilder.DropTable(
                name: "UserWorkoutTemplateExercises");

            migrationBuilder.DropTable(
                name: "WorkoutExercises");

            migrationBuilder.DropTable(
                name: "WorkoutTemplateExercises");

            migrationBuilder.DropTable(
                name: "UserWorkoutTemplates");

            migrationBuilder.DropTable(
                name: "Workouts");

            migrationBuilder.DropTable(
                name: "WorkoutTemplates");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
