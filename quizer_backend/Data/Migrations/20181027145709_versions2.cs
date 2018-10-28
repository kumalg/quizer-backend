using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace quizer_backend.Migrations
{
    public partial class versions2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizVersionItems");

            migrationBuilder.CreateTable(
                name: "SolvingQuizItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QuizId = table.Column<long>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    CreationTime = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolvingQuizItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolvingQuizItems_QuizItems_QuizId",
                        column: x => x.QuizId,
                        principalTable: "QuizItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolvingQuizItems_QuizId",
                table: "SolvingQuizItems",
                column: "QuizId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolvingQuizItems");

            migrationBuilder.CreateTable(
                name: "QuizVersionItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    QuizId = table.Column<long>(nullable: false)
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
                name: "IX_QuizVersionItems_QuizId",
                table: "QuizVersionItems",
                column: "QuizId");
        }
    }
}
