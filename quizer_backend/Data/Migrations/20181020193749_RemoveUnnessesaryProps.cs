using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class RemoveUnnessesaryProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuizId",
                table: "QuizQuestionItems");

            migrationBuilder.DropColumn(
                name: "QuizQuestionId",
                table: "QuizQuestionAnswerItems");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "QuizItems",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "QuizId",
                table: "QuizQuestionItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "QuizQuestionId",
                table: "QuizQuestionAnswerItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "QuizItems",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
