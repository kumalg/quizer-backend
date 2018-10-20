using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class SomeForeignKeyAttr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestionAnswerItems_QuizQuestionItems_QuizQuestionItemId",
                table: "QuizQuestionAnswerItems");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestionItems_QuizItems_QuizItemId",
                table: "QuizQuestionItems");

            migrationBuilder.DropIndex(
                name: "IX_QuizQuestionItems_QuizItemId",
                table: "QuizQuestionItems");

            migrationBuilder.DropIndex(
                name: "IX_QuizQuestionAnswerItems_QuizQuestionItemId",
                table: "QuizQuestionAnswerItems");

            migrationBuilder.DropColumn(
                name: "QuizItemId",
                table: "QuizQuestionItems");

            migrationBuilder.DropColumn(
                name: "QuizQuestionItemId",
                table: "QuizQuestionAnswerItems");

            migrationBuilder.AddColumn<long>(
                name: "QuizItemForeignKey",
                table: "QuizQuestionItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "QuizQuestionItemForeignKey",
                table: "QuizQuestionAnswerItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionItems_QuizItemForeignKey",
                table: "QuizQuestionItems",
                column: "QuizItemForeignKey");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionAnswerItems_QuizQuestionItemForeignKey",
                table: "QuizQuestionAnswerItems",
                column: "QuizQuestionItemForeignKey");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestionAnswerItems_QuizQuestionItems_QuizQuestionItemForeignKey",
                table: "QuizQuestionAnswerItems");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestionItems_QuizItems_QuizItemForeignKey",
                table: "QuizQuestionItems");

            migrationBuilder.DropIndex(
                name: "IX_QuizQuestionItems_QuizItemForeignKey",
                table: "QuizQuestionItems");

            migrationBuilder.DropIndex(
                name: "IX_QuizQuestionAnswerItems_QuizQuestionItemForeignKey",
                table: "QuizQuestionAnswerItems");

            migrationBuilder.DropColumn(
                name: "QuizItemForeignKey",
                table: "QuizQuestionItems");

            migrationBuilder.DropColumn(
                name: "QuizQuestionItemForeignKey",
                table: "QuizQuestionAnswerItems");

            migrationBuilder.AddColumn<long>(
                name: "QuizItemId",
                table: "QuizQuestionItems",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "QuizQuestionItemId",
                table: "QuizQuestionAnswerItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionItems_QuizItemId",
                table: "QuizQuestionItems",
                column: "QuizItemId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionAnswerItems_QuizQuestionItemId",
                table: "QuizQuestionAnswerItems",
                column: "QuizQuestionItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestionAnswerItems_QuizQuestionItems_QuizQuestionItemId",
                table: "QuizQuestionAnswerItems",
                column: "QuizQuestionItemId",
                principalTable: "QuizQuestionItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestionItems_QuizItems_QuizItemId",
                table: "QuizQuestionItems",
                column: "QuizItemId",
                principalTable: "QuizItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
