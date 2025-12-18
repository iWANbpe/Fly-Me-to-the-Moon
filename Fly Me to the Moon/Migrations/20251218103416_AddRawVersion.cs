using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fly_Me_to_the_Moon.Migrations
{
    /// <inheritdoc />
    public partial class AddRawVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "passenger",
                type: "bigint",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "insurance",
                type: "bigint",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "full_health_analysis_result",
                type: "bigint",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "1");
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
