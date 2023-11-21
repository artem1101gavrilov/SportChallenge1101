using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportChallenge.Migrations
{
    /// <inheritdoc />
    public partial class AddRecordsOneTime2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SportsExercise",
                table: "SportsExercise");

            migrationBuilder.RenameTable(
                name: "SportsExercise",
                newName: "SportsExercises");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SportsExercises",
                table: "SportsExercises",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RecordsOneTime",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SportType = table.Column<int>(type: "INTEGER", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordsOneTime", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecordsOneTime");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SportsExercises",
                table: "SportsExercises");

            migrationBuilder.RenameTable(
                name: "SportsExercises",
                newName: "SportsExercise");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SportsExercise",
                table: "SportsExercise",
                column: "Id");
        }
    }
}
