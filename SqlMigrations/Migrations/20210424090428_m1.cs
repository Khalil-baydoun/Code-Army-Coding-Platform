using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SqlMigrations.Migrations
{
    public partial class m1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Salt = table.Column<string>(nullable: true),
                    Role = table.Column<int>(nullable: false, defaultValue: 0)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Email);
                    table.ForeignKey(
                        name: "FK_users_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    AuthorEmail = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Course_users_AuthorEmail",
                        column: x => x.AuthorEmail,
                        principalTable: "users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "CourseUser",
                columns: table => new
                {
                    UserEmail = table.Column<string>(nullable: false),
                    CourseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseUser", x => new { x.CourseId, x.UserEmail });
                    table.ForeignKey(
                        name: "FK_CourseUser_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseUser_users_UserEmail",
                        column: x => x.UserEmail,
                        principalTable: "users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "ProblemSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Prerequisites = table.Column<string>(nullable: true),
                    AuthorEmail = table.Column<string>(nullable: true),
                    CourseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProblemSet_users_AuthorEmail",
                        column: x => x.AuthorEmail,
                        principalTable: "users",
                        principalColumn: "Email");
                    table.ForeignKey(
                        name: "FK_ProblemSet_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DueDates",
                columns: table => new
                {
                    groupId = table.Column<int>(nullable: false),
                    problemSetId = table.Column<int>(nullable: false),
                    dueDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DueDates", x => new { x.groupId, x.problemSetId });
                    table.ForeignKey(
                        name: "FK_DueDates_Groups_groupId",
                        column: x => x.groupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DueDates_ProblemSet_problemSetId",
                        column: x => x.problemSetId,
                        principalTable: "ProblemSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Problem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AuthorEmail = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    GeneralDescription = table.Column<string>(nullable: true),
                    IDescription = table.Column<string>(nullable: true),
                    ODescription = table.Column<string>(nullable: true),
                    SampleInput = table.Column<string>(nullable: true),
                    SampleOutput = table.Column<string>(nullable: true),
                    TimeLimitInMilliseconds = table.Column<int>(nullable: false),
                    TimeFactor = table.Column<int>(nullable: false),
                    MemoryFactor = table.Column<int>(nullable: false),
                    MemoryLimitInKiloBytes = table.Column<int>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    Hints = table.Column<string>(nullable: true),
                    Difficulty = table.Column<int>(nullable: false),
                    ProblemSetId = table.Column<int>(nullable: false),
                    IsPublic = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Problem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Problem_users_AuthorEmail",
                        column: x => x.AuthorEmail,
                        principalTable: "users",
                        principalColumn: "Email");
                    table.ForeignKey(
                        name: "FK_Problem_ProblemSet_ProblemSetId",
                        column: x => x.ProblemSetId,
                        principalTable: "ProblemSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Body = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    AuthorEmail = table.Column<string>(nullable: true),
                    problemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_users_AuthorEmail",
                        column: x => x.AuthorEmail,
                        principalTable: "users",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Problem_problemId",
                        column: x => x.problemId,
                        principalTable: "Problem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Solution",
                columns: table => new
                {
                    ProblemId = table.Column<int>(nullable: false),
                    SourceCode = table.Column<string>(nullable: true),
                    ProgLanguage = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solution", x => x.ProblemId);
                    table.ForeignKey(
                        name: "FK_Solution_Problem_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserEmail = table.Column<string>(nullable: true),
                    ProblemId = table.Column<int>(nullable: false),
                    Verdict = table.Column<int>(nullable: false),
                    TimeTakenInMilliseconds = table.Column<long>(nullable: false),
                    MemoryTakenInKiloBytes = table.Column<long>(nullable: false),
                    sourceCode = table.Column<string>(nullable: true),
                    SubmittedAt = table.Column<DateTime>(nullable: false),
                    ProgrammingLanguage = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionStatistics_Problem_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmissionStatistics_users_UserEmail",
                        column: x => x.UserEmail,
                        principalTable: "users",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProblemId = table.Column<int>(nullable: false),
                    Input = table.Column<string>(nullable: true),
                    Output = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tests_Problem_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    StaticCodeAnalysis = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_SubmissionStatistics_Id",
                        column: x => x.Id,
                        principalTable: "SubmissionStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaReport",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ActualOutput = table.Column<string>(nullable: true),
                    ExpectedOutput = table.Column<string>(nullable: true),
                    Input = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaReport_Reports_Id",
                        column: x => x.Id,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorEmail",
                table: "Comments",
                column: "AuthorEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_problemId",
                table: "Comments",
                column: "problemId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_AuthorEmail",
                table: "Course",
                column: "AuthorEmail");

            migrationBuilder.CreateIndex(
                name: "IX_CourseUser_UserEmail",
                table: "CourseUser",
                column: "UserEmail");

            migrationBuilder.CreateIndex(
                name: "IX_DueDates_problemSetId",
                table: "DueDates",
                column: "problemSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Problem_AuthorEmail",
                table: "Problem",
                column: "AuthorEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Problem_ProblemSetId",
                table: "Problem",
                column: "ProblemSetId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemSet_AuthorEmail",
                table: "ProblemSet",
                column: "AuthorEmail");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemSet_CourseId",
                table: "ProblemSet",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionStatistics_ProblemId",
                table: "SubmissionStatistics",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionStatistics_UserEmail",
                table: "SubmissionStatistics",
                column: "UserEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_ProblemId",
                table: "Tests",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_users_GroupId",
                table: "users",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "CourseUser");

            migrationBuilder.DropTable(
                name: "DueDates");

            migrationBuilder.DropTable(
                name: "Solution");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "WaReport");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "SubmissionStatistics");

            migrationBuilder.DropTable(
                name: "Problem");

            migrationBuilder.DropTable(
                name: "ProblemSet");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
