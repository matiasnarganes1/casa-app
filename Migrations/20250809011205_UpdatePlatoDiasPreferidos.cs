using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlatoDiasPreferidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlatosDiasPreferidos",
                columns: table => new
                {
                    PlatoId = table.Column<int>(type: "int", nullable: false),
                    Dia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatosDiasPreferidos", x => new { x.PlatoId, x.Dia });
                    table.ForeignKey(
                        name: "FK_PlatosDiasPreferidos_Platos_PlatoId",
                        column: x => x.PlatoId,
                        principalTable: "Platos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlatosDiasPreferidos");
        }
    }
}
