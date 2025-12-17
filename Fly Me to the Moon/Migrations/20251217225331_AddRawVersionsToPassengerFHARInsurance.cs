using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fly_Me_to_the_Moon.Migrations
{
    /// <inheritdoc />
    public partial class AddRawVersionsToPassengerFHARInsurance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "passenger",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "insurance",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "full_health_analysis_result",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "passenger");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "insurance");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "full_health_analysis_result");
        }
    }
}
