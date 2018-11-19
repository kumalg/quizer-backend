using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class someStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuizSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    NumberOfSolveSessions = table.Column<long>(nullable: false),
                    NumberOfLearnSessions = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizSessions_Quizzes_Id",
                        column: x => x.Id,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolvedQuizzes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuizId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    CreationTime = table.Column<long>(nullable: false),
                    FinishTime = table.Column<long>(nullable: false),
                    SolveTime = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolvedQuizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolvedQuizzes_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolvedQuestions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SolvedQuizId = table.Column<long>(nullable: false),
                    QuestionId = table.Column<long>(nullable: true),
                    AnsweredCorrectly = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolvedQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolvedQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolvedQuestions_SolvedQuizzes_SolvedQuizId",
                        column: x => x.SolvedQuizId,
                        principalTable: "SolvedQuizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolvedAnswers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SolvedQuestionId = table.Column<long>(nullable: false),
                    AnswerId = table.Column<long>(nullable: true),
                    IsSelected = table.Column<bool>(nullable: false),
                    IsCorrect = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolvedAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolvedAnswers_Answers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolvedAnswers_SolvedQuestions_SolvedQuestionId",
                        column: x => x.SolvedQuestionId,
                        principalTable: "SolvedQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolvedAnswers_AnswerId",
                table: "SolvedAnswers",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_SolvedAnswers_SolvedQuestionId",
                table: "SolvedAnswers",
                column: "SolvedQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SolvedQuestions_QuestionId",
                table: "SolvedQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SolvedQuestions_SolvedQuizId",
                table: "SolvedQuestions",
                column: "SolvedQuizId");

            migrationBuilder.CreateIndex(
                name: "IX_SolvedQuizzes_QuizId",
                table: "SolvedQuizzes",
                column: "QuizId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizSessions");

            migrationBuilder.DropTable(
                name: "SolvedAnswers");

            migrationBuilder.DropTable(
                name: "SolvedQuestions");

            migrationBuilder.DropTable(
                name: "SolvedQuizzes");
        }
    }
}
