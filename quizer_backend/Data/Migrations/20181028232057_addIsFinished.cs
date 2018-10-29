using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class addIsFinished : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolvingQuizItems_QuizItems_QuizId1",
                table: "SolvingQuizItems");

            migrationBuilder.DropIndex(
                name: "IX_SolvingQuizItems_QuizId1",
                table: "SolvingQuizItems");

            migrationBuilder.DropColumn(
                name: "QuizId1",
                table: "SolvingQuizItems");

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "SolvingQuizItems",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "SolvingQuizItems");

            migrationBuilder.AddColumn<long>(
                name: "QuizId1",
                table: "SolvingQuizItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolvingQuizItems_QuizId1",
                table: "SolvingQuizItems",
                column: "QuizId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SolvingQuizItems_QuizItems_QuizId1",
                table: "SolvingQuizItems",
                column: "QuizId1",
                principalTable: "QuizItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
