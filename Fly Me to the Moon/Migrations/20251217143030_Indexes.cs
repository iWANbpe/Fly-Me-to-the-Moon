using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fly_Me_to_the_Moon.Migrations
{
    /// <inheritdoc />
    public partial class Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_service_log_SpaceshipName",
                table: "service_log",
                newName: "IX_ServiceLog_SpaceshipName");

            migrationBuilder.RenameIndex(
                name: "IX_robot_RobotModel",
                table: "robot",
                newName: "IX_Robot_RobotModel");

            migrationBuilder.RenameIndex(
                name: "IX_flight_SpaceshipName",
                table: "flight",
                newName: "IX_Flight_SpaceshipName");

            migrationBuilder.CreateIndex(
                name: "IX_Spaceship_Name",
                table: "spaceship",
                column: "SpaceshipName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Passenger_Name",
                table: "passenger",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Passenger_PhoneNumber",
                table: "passenger",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FullHealthAnalysisResult_AllowedToFly",
                table: "full_health_analysis_result",
                column: "AllowedToFly");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_ArrivalDate",
                table: "flight",
                column: "ArrivalDate");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_DepartureDate",
                table: "flight",
                column: "DepartureDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Spaceship_Name",
                table: "spaceship");

            migrationBuilder.DropIndex(
                name: "IX_Passenger_Name",
                table: "passenger");

            migrationBuilder.DropIndex(
                name: "IX_Passenger_PhoneNumber",
                table: "passenger");

            migrationBuilder.DropIndex(
                name: "IX_FullHealthAnalysisResult_AllowedToFly",
                table: "full_health_analysis_result");

            migrationBuilder.DropIndex(
                name: "IX_Flight_ArrivalDate",
                table: "flight");

            migrationBuilder.DropIndex(
                name: "IX_Flight_DepartureDate",
                table: "flight");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLog_SpaceshipName",
                table: "service_log",
                newName: "IX_service_log_SpaceshipName");

            migrationBuilder.RenameIndex(
                name: "IX_Robot_RobotModel",
                table: "robot",
                newName: "IX_robot_RobotModel");

            migrationBuilder.RenameIndex(
                name: "IX_Flight_SpaceshipName",
                table: "flight",
                newName: "IX_flight_SpaceshipName");
        }
    }
}
