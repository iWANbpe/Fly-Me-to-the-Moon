using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fly_Me_to_the_Moon.Migrations
{
    /// <inheritdoc />
    public partial class BaggageCanBeCreatedWithoutContainer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_baggage_container_ContainerId",
                table: "baggage");

            migrationBuilder.AlterColumn<int>(
                name: "ContainerId",
                table: "baggage",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_baggage_container_ContainerId",
                table: "baggage",
                column: "ContainerId",
                principalTable: "container",
                principalColumn: "ContainerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_baggage_container_ContainerId",
                table: "baggage");

            migrationBuilder.AlterColumn<int>(
                name: "ContainerId",
                table: "baggage",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_baggage_container_ContainerId",
                table: "baggage",
                column: "ContainerId",
                principalTable: "container",
                principalColumn: "ContainerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
