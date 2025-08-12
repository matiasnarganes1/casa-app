using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOneShotToPlato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OneShot",
                table: "Platos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OneShot",
                table: "Platos");
        }
    }
}
