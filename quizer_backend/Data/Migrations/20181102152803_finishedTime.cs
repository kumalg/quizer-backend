using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class finishedTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FinishedTime",
                table: "LearningQuizzes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishedTime",
                table: "LearningQuizzes");
        }
    }
}
