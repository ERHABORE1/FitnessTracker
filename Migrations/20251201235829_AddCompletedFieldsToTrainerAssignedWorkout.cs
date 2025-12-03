using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletedFieldsToTrainerAssignedWorkout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_Workouts_WorkoutId1",
                table: "WorkoutExercises");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutExercises_WorkoutId1",
                table: "WorkoutExercises");

            migrationBuilder.DropColumn(
                name: "WorkoutId1",
                table: "WorkoutExercises");

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "Workouts",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "TrainerAssignedWorkouts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "TrainerAssignedWorkouts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "TrainerAssignedWorkouts");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "TrainerAssignedWorkouts");

            migrationBuilder.AddColumn<int>(
                name: "WorkoutId1",
                table: "WorkoutExercises",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_WorkoutId1",
                table: "WorkoutExercises",
                column: "WorkoutId1");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_Workouts_WorkoutId1",
                table: "WorkoutExercises",
                column: "WorkoutId1",
                principalTable: "Workouts",
                principalColumn: "WorkoutId");
        }
    }
}
