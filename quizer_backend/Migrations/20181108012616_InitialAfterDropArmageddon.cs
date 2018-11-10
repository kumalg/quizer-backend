using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class InitialAfterDropArmageddon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    OwnerId = table.Column<string>(nullable: true),
                    CreationTime = table.Column<long>(nullable: false),
                    LastModifiedTime = table.Column<long>(nullable: false),
                    QuestionsInSolvingQuiz = table.Column<int>(nullable: true),
                    MinutesInSolvingQuiz = table.Column<int>(nullable: true),
                    IsPublic = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    ReoccurrencesIfBad = table.Column<long>(nullable: false),
                    ReoccurrencesOnStart = table.Column<long>(nullable: false),
                    MaxReoccurrences = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "LearningQuizzes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuizId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    CreationTime = table.Column<long>(nullable: false),
                    IsFinished = table.Column<bool>(nullable: false),
                    FinishedTime = table.Column<long>(nullable: true),
                    LearningTime = table.Column<long>(nullable: false),
                    NumberOfQuestions = table.Column<long>(nullable: false),
                    NumberOfLearnedQuestions = table.Column<long>(nullable: false),
                    NumberOfCorrectAnswers = table.Column<long>(nullable: false),
                    NumberOfBadAnswers = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningQuizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningQuizzes_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuizId = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<long>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletionTime = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizAccessItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuizId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Access = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAccessItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAccessItems_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuestionId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<long>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletionTime = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LearningQuizQuestions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LearningQuizId = table.Column<long>(nullable: false),
                    QuestionId = table.Column<long>(nullable: false),
                    Reoccurrences = table.Column<long>(nullable: false),
                    BadUserAnswers = table.Column<long>(nullable: false),
                    GoodUserAnswers = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningQuizQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningQuizQuestions_LearningQuizzes_LearningQuizId",
                        column: x => x.LearningQuizId,
                        principalTable: "LearningQuizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearningQuizQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionVersions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuizQuestionId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<long>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionVersions_Questions_QuizQuestionId",
                        column: x => x.QuizQuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerVersions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuizQuestionAnswerId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<long>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    IsCorrect = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerVersions_Answers_QuizQuestionAnswerId",
                        column: x => x.QuizQuestionAnswerId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerVersions_QuizQuestionAnswerId",
                table: "AnswerVersions",
                column: "QuizQuestionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningQuizQuestions_LearningQuizId",
                table: "LearningQuizQuestions",
                column: "LearningQuizId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningQuizQuestions_QuestionId",
                table: "LearningQuizQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningQuizzes_QuizId",
                table: "LearningQuizzes",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuizId",
                table: "Questions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionVersions_QuizQuestionId",
                table: "QuestionVersions",
                column: "QuizQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAccessItems_QuizId",
                table: "QuizAccessItems",
                column: "QuizId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerVersions");

            migrationBuilder.DropTable(
                name: "LearningQuizQuestions");

            migrationBuilder.DropTable(
                name: "QuestionVersions");

            migrationBuilder.DropTable(
                name: "QuizAccessItems");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "LearningQuizzes");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Quizzes");
        }
    }
}
