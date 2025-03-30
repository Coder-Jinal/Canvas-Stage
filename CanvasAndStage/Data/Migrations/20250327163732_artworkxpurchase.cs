using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanvasAndStage.Data.Migrations
{
    /// <inheritdoc />
    public partial class artworkxpurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArtworkId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_ArtworkId",
                table: "Purchases",
                column: "ArtworkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Artworks_ArtworkId",
                table: "Purchases",
                column: "ArtworkId",
                principalTable: "Artworks",
                principalColumn: "ArtworkId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Artworks_ArtworkId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_ArtworkId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "ArtworkId",
                table: "Purchases");
        }
    }
}
