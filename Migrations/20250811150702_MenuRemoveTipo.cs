using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class MenuRemoveTipo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Menu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "Menu",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
