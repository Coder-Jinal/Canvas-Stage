using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanvasAndStage.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixContactNumberType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First create a temporary column
            migrationBuilder.AddColumn<string>(
                name: "ContactNumberTemp",
                table: "Attendees", // Replace with your actual table name
                type: "nvarchar(max)",
                nullable: true);

            // Copy data from int column to string column
            migrationBuilder.Sql(
                "UPDATE Attendees SET ContactNumberTemp = CONVERT(VARCHAR, ContactNumber)");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "Attendees");

            // Add a new column with the original name but string type
            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "Attendees",
                type: "nvarchar(max)",
                nullable: true);

            // Copy data from temp column to the new column
            migrationBuilder.Sql(
                "UPDATE Attendees SET ContactNumber = ContactNumberTemp");

            // Drop the temporary column
            migrationBuilder.DropColumn(
                name: "ContactNumberTemp",
                table: "Attendees");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // If you need to revert, convert back to int
            migrationBuilder.AlterColumn<int>(
                name: "ContactNumber",
                table: "Attendees", // Replace with your actual table name
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
