using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddShoppingList_ForMenu3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IngredienteNombre",
                table: "ListaDeComprasItem");

            migrationBuilder.CreateIndex(
                name: "IX_ListaDeComprasItem_IngredienteId",
                table: "ListaDeComprasItem",
                column: "IngredienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ListaDeComprasItem_Ingredientes_IngredienteId",
                table: "ListaDeComprasItem",
                column: "IngredienteId",
                principalTable: "Ingredientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListaDeComprasItem_Ingredientes_IngredienteId",
                table: "ListaDeComprasItem");

            migrationBuilder.DropIndex(
                name: "IX_ListaDeComprasItem_IngredienteId",
                table: "ListaDeComprasItem");

            migrationBuilder.AddColumn<string>(
                name: "IngredienteNombre",
                table: "ListaDeComprasItem",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
