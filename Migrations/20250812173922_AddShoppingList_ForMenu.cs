using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddShoppingList_ForMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListaDeCompras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MenuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaDeCompras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListaDeCompras_Menu_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ListaDeComprasItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ListaDeComprasId = table.Column<int>(type: "int", nullable: false),
                    IngredienteId = table.Column<int>(type: "int", nullable: false),
                    IngredienteNombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CantidadTotal = table.Column<int>(type: "int", nullable: false),
                    UnidadMedida = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaDeComprasItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListaDeComprasItem_ListaDeCompras_ListaDeComprasId",
                        column: x => x.ListaDeComprasId,
                        principalTable: "ListaDeCompras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ListaDeCompras_MenuId",
                table: "ListaDeCompras",
                column: "MenuId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListaDeComprasItem_ListaDeComprasId_IngredienteId_UnidadMedi~",
                table: "ListaDeComprasItem",
                columns: new[] { "ListaDeComprasId", "IngredienteId", "UnidadMedida" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListaDeComprasItem");

            migrationBuilder.DropTable(
                name: "ListaDeCompras");
        }
    }
}
