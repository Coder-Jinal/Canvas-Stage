using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanvasAndStage.Data.Migrations
{
    /// <inheritdoc />
    public partial class eventxpurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_EventId",
                table: "Purchases",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Events_EventId",
                table: "Purchases",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Events_EventId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_EventId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Purchases");
        }
    }
}
