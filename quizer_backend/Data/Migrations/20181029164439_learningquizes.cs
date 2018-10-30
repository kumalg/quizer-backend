using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class learningquizes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LearningQuizItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuizId = table.Column<long>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    CreationTime = table.Column<long>(nullable: false),
                    IsFinished = table.Column<bool>(nullable: false),
                    LearningTime = table.Column<long>(nullable: false),
                    NumberOfQuestions = table.Column<long>(nullable: false),
                    NumberOfLearnedQuestions = table.Column<long>(nullable: false),
                    NumberOfCorrectAnswers = table.Column<long>(nullable: false),
                    NumberOfBadAnswers = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningQuizItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningQuizItems_QuizItems_QuizId",
                        column: x => x.QuizId,
                        principalTable: "QuizItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LearningQuizQuestionReoccurrencesItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LearningQuizId = table.Column<long>(nullable: false),
                    QuizQuestionId = table.Column<long>(nullable: false),
                    Reoccurrences = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningQuizQuestionReoccurrencesItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningQuizQuestionReoccurrencesItems_LearningQuizItems_LearningQuizId",
                        column: x => x.LearningQuizId,
                        principalTable: "LearningQuizItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearningQuizQuestionReoccurrencesItems_QuizQuestionItems_QuizQuestionId",
                        column: x => x.QuizQuestionId,
                        principalTable: "QuizQuestionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearningQuizItems_QuizId",
                table: "LearningQuizItems",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningQuizQuestionReoccurrencesItems_LearningQuizId",
                table: "LearningQuizQuestionReoccurrencesItems",
                column: "LearningQuizId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningQuizQuestionReoccurrencesItems_QuizQuestionId",
                table: "LearningQuizQuestionReoccurrencesItems",
                column: "QuizQuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearningQuizQuestionReoccurrencesItems");

            migrationBuilder.DropTable(
                name: "LearningQuizItems");
        }
    }
}
