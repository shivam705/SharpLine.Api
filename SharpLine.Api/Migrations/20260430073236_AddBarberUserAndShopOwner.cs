using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharpLine.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBarberUserAndShopOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Shops",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Barbers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shops_OwnerId",
                table: "Shops",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Barbers_UserId",
                table: "Barbers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Barbers_AspNetUsers_UserId",
                table: "Barbers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_AspNetUsers_OwnerId",
                table: "Shops",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Barbers_AspNetUsers_UserId",
                table: "Barbers");

            migrationBuilder.DropForeignKey(
                name: "FK_Shops_AspNetUsers_OwnerId",
                table: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_Shops_OwnerId",
                table: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_Barbers_UserId",
                table: "Barbers");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Barbers");
        }
    }
}
