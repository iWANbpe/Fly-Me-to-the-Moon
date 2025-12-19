using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fly_Me_to_the_Moon.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "spaceship",
                type: "bigint",
                nullable: false,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "service_log",
                type: "bigint",
                nullable: false,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "robot_model_catalog",
                type: "bigint",
                nullable: false,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "robot",
                type: "bigint",
                nullable: false,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "passenger_flight",
                type: "bigint",
                nullable: false,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "flight",
                type: "bigint",
                nullable: false,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "container_flight",
                type: "bigint",
                nullable: false,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "container",
                type: "bigint",
                nullable: false,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "baggage",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "spaceship");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "service_log");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "robot_model_catalog");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "robot");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "passenger_flight");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "flight");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "container_flight");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "container");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "baggage");
        }
    }
}
