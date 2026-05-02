using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharpLine.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBarberAvilibality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Barbers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Barbers");
        }
    }
}
