using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class versions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuizLinks",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuizId = table.Column<long>(nullable: false),
                    Link = table.Column<string>(nullable: true),
                    Access = table.Column<int>(nullable: false)
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
                name: "QuizQuestionAnswerVersionItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuizQuestionAnswerId = table.Column<long>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    IsCorrect = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<long>(nullable: false)
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
                name: "QuizQuestionVersionItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuizQuestionId = table.Column<long>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    CreationTime = table.Column<long>(nullable: false)
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
                name: "QuizVersionItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuizId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CreationTime = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizVersionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizVersionItems_QuizItems_QuizId",
                        column: x => x.QuizId,
                        principalTable: "QuizItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizLinks_QuizId",
                table: "QuizLinks",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionAnswerVersionItems_QuizQuestionAnswerId",
                table: "QuizQuestionAnswerVersionItems",
                column: "QuizQuestionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionVersionItems_QuizQuestionId",
                table: "QuizQuestionVersionItems",
                column: "QuizQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizVersionItems_QuizId",
                table: "QuizVersionItems",
                column: "QuizId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizLinks");

            migrationBuilder.DropTable(
                name: "QuizQuestionAnswerVersionItems");

            migrationBuilder.DropTable(
                name: "QuizQuestionVersionItems");

            migrationBuilder.DropTable(
                name: "QuizVersionItems");
        }
    }
}
