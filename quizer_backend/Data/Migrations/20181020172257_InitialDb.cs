using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class InitialDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuizItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OwnerId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<long>(nullable: false),
                    LastModifiedTime = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestionItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuizId = table.Column<long>(nullable: false),
                    LastModifiedTime = table.Column<long>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    QuizItemId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestionItems_QuizItems_QuizItemId",
                        column: x => x.QuizItemId,
                        principalTable: "QuizItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestionAnswerItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuizQuestionId = table.Column<long>(nullable: false),
                    LastModifiedTime = table.Column<long>(nullable: false),
                    IsCorrect = table.Column<bool>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    QuizQuestionItemId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestionAnswerItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestionAnswerItems_QuizQuestionItems_QuizQuestionItemId",
                        column: x => x.QuizQuestionItemId,
                        principalTable: "QuizQuestionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionAnswerItems_QuizQuestionItemId",
                table: "QuizQuestionAnswerItems",
                column: "QuizQuestionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionItems_QuizItemId",
                table: "QuizQuestionItems",
                column: "QuizItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizQuestionAnswerItems");

            migrationBuilder.DropTable(
                name: "QuizQuestionItems");

            migrationBuilder.DropTable(
                name: "QuizItems");
        }
    }
}
