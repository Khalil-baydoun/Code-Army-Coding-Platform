using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlMigrations.Migrations
{
    public partial class Changingsubmission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmissionStatistics");

            migrationBuilder.DropTable(
                name: "WaReports");

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProblemId = table.Column<int>(type: "int", nullable: false),
                    Verdict = table.Column<int>(type: "int", nullable: false),
                    SourceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProgrammingLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRetried = table.Column<bool>(type: "bit", nullable: false),
                    TestsPassed = table.Column<int>(type: "int", nullable: false),
                    ActualOutput = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpectedOutput = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WrongTestInput = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RuntimeErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompilerErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submissions_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Submissions_Users_UserEmail",
                        column: x => x.UserEmail,
                        principalTable: "Users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ProblemId",
                table: "Submissions",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_UserEmail",
                table: "Submissions",
                column: "UserEmail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.CreateTable(
                name: "SubmissionStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProblemId = table.Column<int>(type: "int", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsRetried = table.Column<bool>(type: "bit", nullable: false),
                    ProgrammingLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Verdict = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionStatistics_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmissionStatistics_Users_UserEmail",
                        column: x => x.UserEmail,
                        principalTable: "Users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "WaReports",
                columns: table => new
                {
                    SubmissionStatisticsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActualOutput = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpectedOutput = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Input = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaReports", x => x.SubmissionStatisticsId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionStatistics_ProblemId",
                table: "SubmissionStatistics",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionStatistics_UserEmail",
                table: "SubmissionStatistics",
                column: "UserEmail");
        }
    }
}
