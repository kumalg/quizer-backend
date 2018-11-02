using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class newPropsInQuiz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MinutesInSolvingQuiz",
                table: "Quizzes",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "QuestionsInSolvingQuiz",
                table: "Quizzes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinutesInSolvingQuiz",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "QuestionsInSolvingQuiz",
                table: "Quizzes");
        }
    }
}
