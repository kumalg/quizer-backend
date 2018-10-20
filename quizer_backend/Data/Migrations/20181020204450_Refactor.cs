using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class Refactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestionAnswerItems_QuizQuestionItems_QuizQuestionItemForeignKey",
                table: "QuizQuestionAnswerItems");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestionItems_QuizItems_QuizItemForeignKey",
                table: "QuizQuestionItems");

            migrationBuilder.RenameColumn(
                name: "QuizItemForeignKey",
                table: "QuizQuestionItems",
                newName: "QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizQuestionItems_QuizItemForeignKey",
                table: "QuizQuestionItems",
                newName: "IX_QuizQuestionItems_QuizId");

            migrationBuilder.RenameColumn(
                name: "QuizQuestionItemForeignKey",
                table: "QuizQuestionAnswerItems",
                newName: "QuizQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizQuestionAnswerItems_QuizQuestionItemForeignKey",
                table: "QuizQuestionAnswerItems",
                newName: "IX_QuizQuestionAnswerItems_QuizQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestionAnswerItems_QuizQuestionItems_QuizQuestionId",
                table: "QuizQuestionAnswerItems",
                column: "QuizQuestionId",
                principalTable: "QuizQuestionItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestionItems_QuizItems_QuizId",
                table: "QuizQuestionItems",
                column: "QuizId",
                principalTable: "QuizItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestionAnswerItems_QuizQuestionItems_QuizQuestionId",
                table: "QuizQuestionAnswerItems");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestionItems_QuizItems_QuizId",
                table: "QuizQuestionItems");

            migrationBuilder.RenameColumn(
                name: "QuizId",
                table: "QuizQuestionItems",
                newName: "QuizItemForeignKey");

            migrationBuilder.RenameIndex(
                name: "IX_QuizQuestionItems_QuizId",
                table: "QuizQuestionItems",
                newName: "IX_QuizQuestionItems_QuizItemForeignKey");

            migrationBuilder.RenameColumn(
                name: "QuizQuestionId",
                table: "QuizQuestionAnswerItems",
                newName: "QuizQuestionItemForeignKey");

            migrationBuilder.RenameIndex(
                name: "IX_QuizQuestionAnswerItems_QuizQuestionId",
                table: "QuizQuestionAnswerItems",
                newName: "IX_QuizQuestionAnswerItems_QuizQuestionItemForeignKey");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestionAnswerItems_QuizQuestionItems_QuizQuestionItemForeignKey",
                table: "QuizQuestionAnswerItems",
                column: "QuizQuestionItemForeignKey",
                principalTable: "QuizQuestionItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestionItems_QuizItems_QuizItemForeignKey",
                table: "QuizQuestionItems",
                column: "QuizItemForeignKey",
                principalTable: "QuizItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
