using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlatoIngredienteRestrictDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlatoIngredientes_Ingredientes_IngredienteId",
                table: "PlatoIngredientes");

            migrationBuilder.AddForeignKey(
                name: "FK_PlatoIngredientes_Ingredientes_IngredienteId",
                table: "PlatoIngredientes",
                column: "IngredienteId",
                principalTable: "Ingredientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlatoIngredientes_Ingredientes_IngredienteId",
                table: "PlatoIngredientes");

            migrationBuilder.AddForeignKey(
                name: "FK_PlatoIngredientes_Ingredientes_IngredienteId",
                table: "PlatoIngredientes",
                column: "IngredienteId",
                principalTable: "Ingredientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
