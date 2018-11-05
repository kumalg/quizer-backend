using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class deletionTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DeletionTime",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeletionTime",
                table: "Answers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Answers");
        }
    }
}
