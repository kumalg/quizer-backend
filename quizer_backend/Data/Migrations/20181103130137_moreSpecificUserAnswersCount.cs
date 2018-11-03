using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class moreSpecificUserAnswersCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BadUserAnswers",
                table: "LearningQuizQuestions",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "GoodUserAnswers",
                table: "LearningQuizQuestions",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BadUserAnswers",
                table: "LearningQuizQuestions");

            migrationBuilder.DropColumn(
                name: "GoodUserAnswers",
                table: "LearningQuizQuestions");
        }
    }
}
