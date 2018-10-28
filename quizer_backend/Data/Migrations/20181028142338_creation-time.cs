using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class creationtime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreationTime",
                table: "QuizQuestionItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CreationTime",
                table: "QuizQuestionAnswerItems",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "QuizQuestionItems");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "QuizQuestionAnswerItems");
        }
    }
}
