using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitnessTracker.Migrations
{
    /// <inheritdoc />
    public partial class SeedExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "IsBodyweight", "MuscleGroup", "Name" },
                values: new object[,]
                {
                    { 1, false, "Chest", "Bench Press" },
                    { 2, false, "Chest", "Incline Bench Press" },
                    { 3, true, "Chest", "Push-Up" },
                    { 4, false, "Chest", "Chest Fly" },
                    { 5, false, "Legs", "Squat" },
                    { 6, false, "Legs", "Leg Press" },
                    { 7, false, "Legs", "Romanian Deadlift" },
                    { 8, true, "Legs", "Lunge" },
                    { 9, false, "Legs", "Calf Raise" },
                    { 10, false, "Legs", "Hip Thrust" },
                    { 11, false, "Back", "Deadlift" },
                    { 12, false, "Back", "Barbell Row" },
                    { 13, false, "Back", "Dumbbell Row" },
                    { 14, true, "Back", "Pull-Up" },
                    { 15, false, "Back", "Lat Pulldown" },
                    { 16, false, "Shoulders", "Overhead Press" },
                    { 17, false, "Shoulders", "Dumbbell Shoulder Press" },
                    { 18, false, "Shoulders", "Lateral Raise" },
                    { 19, false, "Arms", "Bicep Curl" },
                    { 20, true, "Arms", "Tricep Dip" },
                    { 21, true, "Core", "Plank" },
                    { 22, true, "Core", "Sit-Up" }
                });

            // The rows above were inserted with explicit Ids, which doesn't
            // advance the identity sequence backing this column - without
            // this, the next admin-created exercise would try to reuse Id 1
            // and fail on the unique primary key. Wrapped in its own DO
            // block using PERFORM (not SELECT): idempotent migration
            // scripts (see `dotnet ef migrations script --idempotent`)
            // nest each migration's operations inside a PL/pgSQL DO block,
            // and a bare SELECT with no destination isn't valid there.
            migrationBuilder.Sql(
                "DO $$ BEGIN PERFORM setval(pg_get_serial_sequence('\"Exercises\"', 'Id'), (SELECT MAX(\"Id\") FROM \"Exercises\")); END $$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 22);
        }
    }
}
