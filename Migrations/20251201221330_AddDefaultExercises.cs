using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitnessTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "WorkoutTemplates");

            migrationBuilder.UpdateData(
                table: "WorkoutTemplateExercises",
                keyColumn: "WorkoutTemplateExerciseId",
                keyValue: 2001,
                column: "ExerciseName",
                value: "Dumbbell Bicep Curl");

            migrationBuilder.UpdateData(
                table: "WorkoutTemplateExercises",
                keyColumn: "WorkoutTemplateExerciseId",
                keyValue: 2002,
                columns: new[] { "ExerciseName", "Reps" },
                values: new object[] { "Hammer Curl", 12 });

            migrationBuilder.UpdateData(
                table: "WorkoutTemplateExercises",
                keyColumn: "WorkoutTemplateExerciseId",
                keyValue: 2003,
                columns: new[] { "ExerciseName", "Reps", "Sets" },
                values: new object[] { "Tricep Cable Pushdown", 12, 3 });

            migrationBuilder.UpdateData(
                table: "WorkoutTemplateExercises",
                keyColumn: "WorkoutTemplateExerciseId",
                keyValue: 2004,
                columns: new[] { "ExerciseName", "Reps", "Sets" },
                values: new object[] { "Overhead Dumbbell Extension", 10, 3 });

            migrationBuilder.UpdateData(
                table: "WorkoutTemplates",
                keyColumn: "WorkoutTemplateId",
                keyValue: 1001,
                columns: new[] { "Category", "TemplateName" },
                values: new object[] { "Biceps", "Biceps Workout" });

            migrationBuilder.UpdateData(
                table: "WorkoutTemplates",
                keyColumn: "WorkoutTemplateId",
                keyValue: 1002,
                columns: new[] { "Category", "TemplateName" },
                values: new object[] { "Triceps", "Triceps Workout" });

            migrationBuilder.InsertData(
                table: "WorkoutTemplates",
                columns: new[] { "WorkoutTemplateId", "Category", "TemplateName" },
                values: new object[,]
                {
                    { 1003, "Back", "Back Workout" },
                    { 1004, "Legs", "Leg Workout" }
                });

            migrationBuilder.InsertData(
                table: "WorkoutTemplateExercises",
                columns: new[] { "WorkoutTemplateExerciseId", "ExerciseName", "Reps", "Sets", "Weight", "WorkoutTemplateId" },
                values: new object[,]
                {
                    { 2005, "Lat Pulldown", 10, 3, 0.0, 1003 },
                    { 2006, "Seated Cable Row", 12, 3, 0.0, 1003 },
                    { 2007, "Leg Press", 10, 3, 0.0, 1004 },
                    { 2008, "Goblet Squat", 12, 3, 0.0, 1004 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WorkoutTemplateExercises",
                keyColumn: "WorkoutTemplateExerciseId",
                keyValue: 2005);

            migrationBuilder.DeleteData(
                table: "WorkoutTemplateExercises",
                keyColumn: "WorkoutTemplateExerciseId",
                keyValue: 2006);

            migrationBuilder.DeleteData(
                table: "WorkoutTemplateExercises",
                keyColumn: "WorkoutTemplateExerciseId",
                keyValue: 2007);

            migrationBuilder.DeleteData(
                table: "WorkoutTemplateExercises",
                keyColumn: "WorkoutTemplateExerciseId",
                keyValue: 2008);

            migrationBuilder.DeleteData(
                table: "WorkoutTemplates",
                keyColumn: "WorkoutTemplateId",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "WorkoutTemplates",
                keyColumn: "WorkoutTemplateId",
                keyValue: 1004);

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "WorkoutTemplates",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "WorkoutTemplateExercises",
                keyColumn: "WorkoutTemplateExerciseId",
                keyValue: 2001,
                column: "ExerciseName",
                value: "Squat");

            migrationBuilder.UpdateData(
                table: "WorkoutTemplateExercises",
                keyColumn: "WorkoutTemplateExerciseId",
                keyValue: 2002,
                columns: new[] { "ExerciseName", "Reps" },
                values: new object[] { "Bench Press", 10 });

            migrationBuilder.UpdateData(
                table: "WorkoutTemplateExercises",
                keyColumn: "WorkoutTemplateExerciseId",
                keyValue: 2003,
                columns: new[] { "ExerciseName", "Reps", "Sets" },
                values: new object[] { "Push Day (Chest/Shoulders)", 8, 4 });

            migrationBuilder.UpdateData(
                table: "WorkoutTemplateExercises",
                keyColumn: "WorkoutTemplateExerciseId",
                keyValue: 2004,
                columns: new[] { "ExerciseName", "Reps", "Sets" },
                values: new object[] { "Pull Day (Back)", 8, 4 });

            migrationBuilder.UpdateData(
                table: "WorkoutTemplates",
                keyColumn: "WorkoutTemplateId",
                keyValue: 1001,
                columns: new[] { "Category", "CreatorId", "TemplateName" },
                values: new object[] { "Beginner", null, "Full Body Starter" });

            migrationBuilder.UpdateData(
                table: "WorkoutTemplates",
                keyColumn: "WorkoutTemplateId",
                keyValue: 1002,
                columns: new[] { "Category", "CreatorId", "TemplateName" },
                values: new object[] { "Split", null, "Push / Pull / Legs" });
        }
    }
}
