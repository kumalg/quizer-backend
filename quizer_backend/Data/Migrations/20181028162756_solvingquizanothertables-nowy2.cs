using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class solvingquizanothertablesnowy2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolvingQuizItems_QuizItems_QuizId",
                table: "SolvingQuizItems");

            migrationBuilder.AlterColumn<long>(
                name: "QuizId",
                table: "SolvingQuizItems",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "QuizId1",
                table: "SolvingQuizItems",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SolvingQuizFinishedQuestionItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SolvingQuizId = table.Column<long>(nullable: false),
                    QuizQuestionId = table.Column<long>(nullable: true),
                    CorrectlyAnswered = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolvingQuizFinishedQuestionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolvingQuizFinishedQuestionItems_QuizQuestionItems_QuizQuestionId",
                        column: x => x.QuizQuestionId,
                        principalTable: "QuizQuestionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SolvingQuizFinishedQuestionItems_SolvingQuizItems_SolvingQuizId",
                        column: x => x.SolvingQuizId,
                        principalTable: "SolvingQuizItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolvingQuizFinishedQuestionSelectedAnswerItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FinishedQuestionId = table.Column<long>(nullable: false),
                    QuizQuestionAnswerId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolvingQuizFinishedQuestionSelectedAnswerItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolvingQuizFinishedQuestionSelectedAnswerItems_SolvingQuizFinishedQuestionItems_FinishedQuestionId",
                        column: x => x.FinishedQuestionId,
                        principalTable: "SolvingQuizFinishedQuestionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolvingQuizFinishedQuestionSelectedAnswerItems_QuizQuestionAnswerItems_QuizQuestionAnswerId",
                        column: x => x.QuizQuestionAnswerId,
                        principalTable: "QuizQuestionAnswerItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolvingQuizItems_QuizId1",
                table: "SolvingQuizItems",
                column: "QuizId1");

            migrationBuilder.CreateIndex(
                name: "IX_SolvingQuizFinishedQuestionItems_QuizQuestionId",
                table: "SolvingQuizFinishedQuestionItems",
                column: "QuizQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SolvingQuizFinishedQuestionItems_SolvingQuizId",
                table: "SolvingQuizFinishedQuestionItems",
                column: "SolvingQuizId");

            migrationBuilder.CreateIndex(
                name: "IX_SolvingQuizFinishedQuestionSelectedAnswerItems_FinishedQuestionId",
                table: "SolvingQuizFinishedQuestionSelectedAnswerItems",
                column: "FinishedQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SolvingQuizFinishedQuestionSelectedAnswerItems_QuizQuestionAnswerId",
                table: "SolvingQuizFinishedQuestionSelectedAnswerItems",
                column: "QuizQuestionAnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolvingQuizItems_QuizItems_QuizId",
                table: "SolvingQuizItems",
                column: "QuizId",
                principalTable: "QuizItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SolvingQuizItems_QuizItems_QuizId1",
                table: "SolvingQuizItems",
                column: "QuizId1",
                principalTable: "QuizItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolvingQuizItems_QuizItems_QuizId",
                table: "SolvingQuizItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SolvingQuizItems_QuizItems_QuizId1",
                table: "SolvingQuizItems");

            migrationBuilder.DropTable(
                name: "SolvingQuizFinishedQuestionSelectedAnswerItems");

            migrationBuilder.DropTable(
                name: "SolvingQuizFinishedQuestionItems");

            migrationBuilder.DropIndex(
                name: "IX_SolvingQuizItems_QuizId1",
                table: "SolvingQuizItems");

            migrationBuilder.DropColumn(
                name: "QuizId1",
                table: "SolvingQuizItems");

            migrationBuilder.AlterColumn<long>(
                name: "QuizId",
                table: "SolvingQuizItems",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SolvingQuizItems_QuizItems_QuizId",
                table: "SolvingQuizItems",
                column: "QuizId",
                principalTable: "QuizItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
