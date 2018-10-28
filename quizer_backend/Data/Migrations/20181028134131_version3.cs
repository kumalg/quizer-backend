using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class version3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastModifiedTime",
                table: "QuizQuestionItems");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "QuizQuestionItems");

            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "QuizQuestionAnswerItems");

            migrationBuilder.DropColumn(
                name: "LastModifiedTime",
                table: "QuizQuestionAnswerItems");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "QuizQuestionAnswerItems");

            migrationBuilder.RenameColumn(
                name: "CreatedTime",
                table: "QuizItems",
                newName: "CreationTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "QuizItems",
                newName: "CreatedTime");

            migrationBuilder.AddColumn<long>(
                name: "LastModifiedTime",
                table: "QuizQuestionItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "QuizQuestionItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "QuizQuestionAnswerItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "LastModifiedTime",
                table: "QuizQuestionAnswerItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "QuizQuestionAnswerItems",
                nullable: true);
        }
    }
}
