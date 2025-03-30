using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanvasAndStage.Data.Migrations
{
    /// <inheritdoc />
    public partial class attendeexpurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttendeeId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_AttendeeId",
                table: "Purchases",
                column: "AttendeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Attendees_AttendeeId",
                table: "Purchases",
                column: "AttendeeId",
                principalTable: "Attendees",
                principalColumn: "AttendeeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Attendees_AttendeeId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_AttendeeId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "AttendeeId",
                table: "Purchases");
        }
    }
}
