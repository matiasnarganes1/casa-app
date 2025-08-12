using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIngrediente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "Ingredientes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Ingredientes");
        }
    }
}
