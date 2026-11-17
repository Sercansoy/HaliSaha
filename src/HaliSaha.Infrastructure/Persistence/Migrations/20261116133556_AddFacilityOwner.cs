using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HaliSaha.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFacilityOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Facilities",
                type: "int",
                nullable: false,
                defaultValue: 7);

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_OwnerId",
                table: "Facilities",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_Users_OwnerId",
                table: "Facilities",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_Users_OwnerId",
                table: "Facilities");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_OwnerId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Facilities");
        }
    }
}
