using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fly_Me_to_the_Moon.Migrations
{
    /// <inheritdoc />
    public partial class SchemaSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "container",
                columns: table => new
                {
                    ContainerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaxWeight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_container", x => x.ContainerId);
                });

            migrationBuilder.CreateTable(
                name: "full_health_analysis_result",
                columns: table => new
                {
                    AnalysisId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExpireBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AllowedToFly = table.Column<bool>(type: "boolean", nullable: false),
                    GrantedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_full_health_analysis_result", x => x.AnalysisId);
                });

            migrationBuilder.CreateTable(
                name: "insurance",
                columns: table => new
                {
                    InsuranceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExpireBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompanyGrantedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_insurance", x => x.InsuranceId);
                });

            migrationBuilder.CreateTable(
                name: "robot_model_catalog",
                columns: table => new
                {
                    RobotModel = table.Column<string>(type: "text", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_robot_model_catalog", x => x.RobotModel);
                });

            migrationBuilder.CreateTable(
                name: "spaceship",
                columns: table => new
                {
                    SpaceshipName = table.Column<string>(type: "text", nullable: false),
                    DateOfManufacture = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PassengerCapacity = table.Column<int>(type: "integer", nullable: false),
                    ContainersCapacity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_spaceship", x => x.SpaceshipName);
                });

            migrationBuilder.CreateTable(
                name: "passenger",
                columns: table => new
                {
                    PassengerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    AnalysisId = table.Column<int>(type: "integer", nullable: false),
                    InsuranceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passenger", x => x.PassengerId);
                    table.ForeignKey(
                        name: "FK_passenger_full_health_analysis_result_AnalysisId",
                        column: x => x.AnalysisId,
                        principalTable: "full_health_analysis_result",
                        principalColumn: "AnalysisId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_passenger_insurance_InsuranceId",
                        column: x => x.InsuranceId,
                        principalTable: "insurance",
                        principalColumn: "InsuranceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "robot",
                columns: table => new
                {
                    RobotId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RobotModel = table.Column<string>(type: "text", nullable: false),
                    ContainerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_robot", x => x.RobotId);
                    table.ForeignKey(
                        name: "FK_robot_container_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "container",
                        principalColumn: "ContainerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_robot_robot_model_catalog_RobotModel",
                        column: x => x.RobotModel,
                        principalTable: "robot_model_catalog",
                        principalColumn: "RobotModel",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "flight",
                columns: table => new
                {
                    FlightId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeparturePoint = table.Column<string>(type: "text", nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PlaceOfArrival = table.Column<string>(type: "text", nullable: false),
                    ArrivalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SpaceshipName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_flight", x => x.FlightId);
                    table.ForeignKey(
                        name: "FK_flight_spaceship_SpaceshipName",
                        column: x => x.SpaceshipName,
                        principalTable: "spaceship",
                        principalColumn: "SpaceshipName");
                });

            migrationBuilder.CreateTable(
                name: "service_log",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SpaceshipName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_log", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_service_log_spaceship_SpaceshipName",
                        column: x => x.SpaceshipName,
                        principalTable: "spaceship",
                        principalColumn: "SpaceshipName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "baggage",
                columns: table => new
                {
                    BaggageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaxWeight = table.Column<int>(type: "integer", nullable: false),
                    PassengerId = table.Column<int>(type: "integer", nullable: true),
                    ContainerId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_baggage", x => x.BaggageId);
                    table.ForeignKey(
                        name: "FK_baggage_container_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "container",
                        principalColumn: "ContainerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_baggage_passenger_PassengerId",
                        column: x => x.PassengerId,
                        principalTable: "passenger",
                        principalColumn: "PassengerId");
                });

            migrationBuilder.CreateTable(
                name: "container_flight",
                columns: table => new
                {
                    ContainerId = table.Column<int>(type: "integer", nullable: false),
                    FlightId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_container_flight", x => new { x.ContainerId, x.FlightId });
                    table.ForeignKey(
                        name: "FK_container_flight_container_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "container",
                        principalColumn: "ContainerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_container_flight_flight_FlightId",
                        column: x => x.FlightId,
                        principalTable: "flight",
                        principalColumn: "FlightId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "passenger_flight",
                columns: table => new
                {
                    PassengerId = table.Column<int>(type: "integer", nullable: false),
                    FlightId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passenger_flight", x => new { x.PassengerId, x.FlightId });
                    table.ForeignKey(
                        name: "FK_passenger_flight_flight_FlightId",
                        column: x => x.FlightId,
                        principalTable: "flight",
                        principalColumn: "FlightId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_passenger_flight_passenger_PassengerId",
                        column: x => x.PassengerId,
                        principalTable: "passenger",
                        principalColumn: "PassengerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_baggage_ContainerId",
                table: "baggage",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_baggage_PassengerId",
                table: "baggage",
                column: "PassengerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_container_flight_FlightId",
                table: "container_flight",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_flight_SpaceshipName",
                table: "flight",
                column: "SpaceshipName");

            migrationBuilder.CreateIndex(
                name: "IX_passenger_AnalysisId",
                table: "passenger",
                column: "AnalysisId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_passenger_InsuranceId",
                table: "passenger",
                column: "InsuranceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_passenger_flight_FlightId",
                table: "passenger_flight",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_robot_ContainerId",
                table: "robot",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_robot_RobotModel",
                table: "robot",
                column: "RobotModel");

            migrationBuilder.CreateIndex(
                name: "IX_service_log_SpaceshipName",
                table: "service_log",
                column: "SpaceshipName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "baggage");

            migrationBuilder.DropTable(
                name: "container_flight");

            migrationBuilder.DropTable(
                name: "passenger_flight");

            migrationBuilder.DropTable(
                name: "robot");

            migrationBuilder.DropTable(
                name: "service_log");

            migrationBuilder.DropTable(
                name: "flight");

            migrationBuilder.DropTable(
                name: "passenger");

            migrationBuilder.DropTable(
                name: "container");

            migrationBuilder.DropTable(
                name: "robot_model_catalog");

            migrationBuilder.DropTable(
                name: "spaceship");

            migrationBuilder.DropTable(
                name: "full_health_analysis_result");

            migrationBuilder.DropTable(
                name: "insurance");
        }
    }
}
