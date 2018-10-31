using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class renaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningQuizItems_QuizItems_QuizId",
                table: "LearningQuizItems");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAccessItems_QuizItems_QuizId",
                table: "QuizAccessItems");

            migrationBuilder.DropTable(
                name: "LearningQuizQuestionReoccurrencesItems");

            migrationBuilder.DropTable(
                name: "QuizLinks");

            migrationBuilder.DropTable(
                name: "QuizQuestionAnswerVersionItems");

            migrationBuilder.DropTable(
                name: "QuizQuestionVersionItems");

            migrationBuilder.DropTable(
                name: "SolvingQuizFinishedQuestionSelectedAnswerItems");

            migrationBuilder.DropTable(
                name: "SolvingQuizFinishedQuestionItems");

            migrationBuilder.DropTable(
                name: "QuizQuestionAnswerItems");

            migrationBuilder.DropTable(
                name: "SolvingQuizItems");

            migrationBuilder.DropTable(
                name: "QuizQuestionItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizItems",
                table: "QuizItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LearningQuizItems",
                table: "LearningQuizItems");

            migrationBuilder.RenameTable(
                name: "QuizItems",
                newName: "Quizzes");

            migrationBuilder.RenameTable(
                name: "LearningQuizItems",
                newName: "LearningQuizzes");

            migrationBuilder.RenameIndex(
                name: "IX_LearningQuizItems_QuizId",
                table: "LearningQuizzes",
                newName: "IX_LearningQuizzes_QuizId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quizzes",
                table: "Quizzes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LearningQuizzes",
                table: "LearningQuizzes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuizId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<long>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
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
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuestionId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<long>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
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
                    Reoccurrences = table.Column<long>(nullable: false)
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
                    Value = table.Column<string>(nullable: true)
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
                    Value = table.Column<string>(nullable: true),
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
                name: "IX_Questions_QuizId",
                table: "Questions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionVersions_QuizQuestionId",
                table: "QuestionVersions",
                column: "QuizQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningQuizzes_Quizzes_QuizId",
                table: "LearningQuizzes",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAccessItems_Quizzes_QuizId",
                table: "QuizAccessItems",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningQuizzes_Quizzes_QuizId",
                table: "LearningQuizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAccessItems_Quizzes_QuizId",
                table: "QuizAccessItems");

            migrationBuilder.DropTable(
                name: "AnswerVersions");

            migrationBuilder.DropTable(
                name: "LearningQuizQuestions");

            migrationBuilder.DropTable(
                name: "QuestionVersions");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quizzes",
                table: "Quizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LearningQuizzes",
                table: "LearningQuizzes");

            migrationBuilder.RenameTable(
                name: "Quizzes",
                newName: "QuizItems");

            migrationBuilder.RenameTable(
                name: "LearningQuizzes",
                newName: "LearningQuizItems");

            migrationBuilder.RenameIndex(
                name: "IX_LearningQuizzes_QuizId",
                table: "LearningQuizItems",
                newName: "IX_LearningQuizItems_QuizId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizItems",
                table: "QuizItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LearningQuizItems",
                table: "LearningQuizItems",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "QuizLinks",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Access = table.Column<int>(nullable: false),
                    Link = table.Column<string>(nullable: true),
                    QuizId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizLinks_QuizItems_QuizId",
                        column: x => x.QuizId,
                        principalTable: "QuizItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestionItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<long>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    QuizId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestionItems_QuizItems_QuizId",
                        column: x => x.QuizId,
                        principalTable: "QuizItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolvingQuizItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<long>(nullable: false),
                    IsFinished = table.Column<bool>(nullable: false),
                    QuizId = table.Column<long>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolvingQuizItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolvingQuizItems_QuizItems_QuizId",
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

            migrationBuilder.CreateTable(
                name: "QuizQuestionAnswerItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<long>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    QuizQuestionId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestionAnswerItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestionAnswerItems_QuizQuestionItems_QuizQuestionId",
                        column: x => x.QuizQuestionId,
                        principalTable: "QuizQuestionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestionVersionItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<long>(nullable: false),
                    QuizQuestionId = table.Column<long>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestionVersionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestionVersionItems_QuizQuestionItems_QuizQuestionId",
                        column: x => x.QuizQuestionId,
                        principalTable: "QuizQuestionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolvingQuizFinishedQuestionItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CorrectlyAnswered = table.Column<bool>(nullable: false),
                    QuizQuestionId = table.Column<long>(nullable: true),
                    SolvingQuizId = table.Column<long>(nullable: false)
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
                name: "QuizQuestionAnswerVersionItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<long>(nullable: false),
                    IsCorrect = table.Column<bool>(nullable: false),
                    QuizQuestionAnswerId = table.Column<long>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestionAnswerVersionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestionAnswerVersionItems_QuizQuestionAnswerItems_QuizQuestionAnswerId",
                        column: x => x.QuizQuestionAnswerId,
                        principalTable: "QuizQuestionAnswerItems",
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
                name: "IX_LearningQuizQuestionReoccurrencesItems_LearningQuizId",
                table: "LearningQuizQuestionReoccurrencesItems",
                column: "LearningQuizId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningQuizQuestionReoccurrencesItems_QuizQuestionId",
                table: "LearningQuizQuestionReoccurrencesItems",
                column: "QuizQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizLinks_QuizId",
                table: "QuizLinks",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionAnswerItems_QuizQuestionId",
                table: "QuizQuestionAnswerItems",
                column: "QuizQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionAnswerVersionItems_QuizQuestionAnswerId",
                table: "QuizQuestionAnswerVersionItems",
                column: "QuizQuestionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionItems_QuizId",
                table: "QuizQuestionItems",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionVersionItems_QuizQuestionId",
                table: "QuizQuestionVersionItems",
                column: "QuizQuestionId");

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

            migrationBuilder.CreateIndex(
                name: "IX_SolvingQuizItems_QuizId",
                table: "SolvingQuizItems",
                column: "QuizId");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningQuizItems_QuizItems_QuizId",
                table: "LearningQuizItems",
                column: "QuizId",
                principalTable: "QuizItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAccessItems_QuizItems_QuizId",
                table: "QuizAccessItems",
                column: "QuizId",
                principalTable: "QuizItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
