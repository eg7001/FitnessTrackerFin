using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitnessTracker.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTheModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sets_WorkoutExercises_WorkoutExerciseId",
                table: "Sets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutExercises",
                table: "WorkoutExercises");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutExercises_WorkoutId",
                table: "WorkoutExercises");

            migrationBuilder.DropIndex(
                name: "IX_Sets_WorkoutExerciseId",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "WorkoutExercises");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "WorkoutExercises",
                newName: "Sets");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "WorkoutExercises",
                newName: "Repetitions");

            migrationBuilder.AlterColumn<int>(
                name: "Repetitions",
                table: "WorkoutExercises",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "WorkoutExercises",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkoutExerciseExerciseId",
                table: "Sets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "WorkoutExerciseWorkoutId",
                table: "Sets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutExercises",
                table: "WorkoutExercises",
                columns: new[] { "WorkoutId", "ExerciseId" });

            migrationBuilder.CreateIndex(
                name: "IX_Sets_WorkoutExerciseWorkoutId_WorkoutExerciseExerciseId",
                table: "Sets",
                columns: new[] { "WorkoutExerciseWorkoutId", "WorkoutExerciseExerciseId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_WorkoutExercises_WorkoutExerciseWorkoutId_WorkoutExerc~",
                table: "Sets",
                columns: new[] { "WorkoutExerciseWorkoutId", "WorkoutExerciseExerciseId" },
                principalTable: "WorkoutExercises",
                principalColumns: new[] { "WorkoutId", "ExerciseId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sets_WorkoutExercises_WorkoutExerciseWorkoutId_WorkoutExerc~",
                table: "Sets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutExercises",
                table: "WorkoutExercises");

            migrationBuilder.DropIndex(
                name: "IX_Sets_WorkoutExerciseWorkoutId_WorkoutExerciseExerciseId",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "WorkoutExercises");

            migrationBuilder.DropColumn(
                name: "WorkoutExerciseExerciseId",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "WorkoutExerciseWorkoutId",
                table: "Sets");

            migrationBuilder.RenameColumn(
                name: "Sets",
                table: "WorkoutExercises",
                newName: "Order");

            migrationBuilder.RenameColumn(
                name: "Repetitions",
                table: "WorkoutExercises",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "WorkoutExercises",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "WorkoutExercises",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutExercises",
                table: "WorkoutExercises",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_WorkoutId",
                table: "WorkoutExercises",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_WorkoutExerciseId",
                table: "Sets",
                column: "WorkoutExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_WorkoutExercises_WorkoutExerciseId",
                table: "Sets",
                column: "WorkoutExerciseId",
                principalTable: "WorkoutExercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
