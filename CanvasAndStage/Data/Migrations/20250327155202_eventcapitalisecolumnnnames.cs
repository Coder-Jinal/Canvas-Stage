using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanvasAndStage.Data.Migrations
{
    /// <inheritdoc />
    public partial class eventcapitalisecolumnnnames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "location",
                table: "Events",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Events",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "Events",
                newName: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Events",
                newName: "location");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Events",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Events",
                newName: "date");
        }
    }
}
