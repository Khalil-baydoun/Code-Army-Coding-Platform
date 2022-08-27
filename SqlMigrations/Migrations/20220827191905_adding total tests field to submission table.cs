using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlMigrations.Migrations
{
    public partial class addingtotaltestsfieldtosubmissiontable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalTests",
                table: "Submissions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalTests",
                table: "Submissions");
        }
    }
}
