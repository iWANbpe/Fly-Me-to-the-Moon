using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fly_Me_to_the_Moon.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Container",
                columns: table => new
                {
                    ContainerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaxWeight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Container", x => x.ContainerId);
                });

            migrationBuilder.CreateTable(
                name: "FullHealthAnalysisResult",
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
                    table.PrimaryKey("PK_FullHealthAnalysisResult", x => x.AnalysisId);
                });

            migrationBuilder.CreateTable(
                name: "Insurance",
                columns: table => new
                {
                    InsuranceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExpireBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompanyGrantedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insurance", x => x.InsuranceId);
                });

            migrationBuilder.CreateTable(
                name: "RobotModelCatalog",
                columns: table => new
                {
                    RobotModel = table.Column<string>(type: "text", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RobotModelCatalog", x => x.RobotModel);
                });

            migrationBuilder.CreateTable(
                name: "Spaceship",
                columns: table => new
                {
                    SpaceshipName = table.Column<string>(type: "text", nullable: false),
                    DateOfManufacture = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PassengerCapacity = table.Column<int>(type: "integer", nullable: false),
                    ContainersCapacity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spaceship", x => x.SpaceshipName);
                });

            migrationBuilder.CreateTable(
                name: "Passenger",
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
                    table.PrimaryKey("PK_Passenger", x => x.PassengerId);
                    table.ForeignKey(
                        name: "FK_Passenger_FullHealthAnalysisResult_AnalysisId",
                        column: x => x.AnalysisId,
                        principalTable: "FullHealthAnalysisResult",
                        principalColumn: "AnalysisId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Passenger_Insurance_InsuranceId",
                        column: x => x.InsuranceId,
                        principalTable: "Insurance",
                        principalColumn: "InsuranceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Robot",
                columns: table => new
                {
                    RobotId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RobotModel = table.Column<string>(type: "text", nullable: false),
                    ContainerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Robot", x => x.RobotId);
                    table.ForeignKey(
                        name: "FK_Robot_Container_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Container",
                        principalColumn: "ContainerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Robot_RobotModelCatalog_RobotModel",
                        column: x => x.RobotModel,
                        principalTable: "RobotModelCatalog",
                        principalColumn: "RobotModel",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flight",
                columns: table => new
                {
                    FlightId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeparturePoint = table.Column<string>(type: "text", nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PlaceOfArrival = table.Column<string>(type: "text", nullable: false),
                    ArrivalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SpaceshipName = table.Column<string>(type: "text", nullable: true),
                    SpaceshipName1 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flight", x => x.FlightId);
                    table.ForeignKey(
                        name: "FK_Flight_Spaceship_SpaceshipName1",
                        column: x => x.SpaceshipName1,
                        principalTable: "Spaceship",
                        principalColumn: "SpaceshipName");
                });

            migrationBuilder.CreateTable(
                name: "ServiceLog",
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
                    table.PrimaryKey("PK_ServiceLog", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_ServiceLog_Spaceship_SpaceshipName",
                        column: x => x.SpaceshipName,
                        principalTable: "Spaceship",
                        principalColumn: "SpaceshipName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Baggage",
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
                    table.PrimaryKey("PK_Baggage", x => x.BaggageId);
                    table.ForeignKey(
                        name: "FK_Baggage_Container_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Container",
                        principalColumn: "ContainerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Baggage_Passenger_PassengerId",
                        column: x => x.PassengerId,
                        principalTable: "Passenger",
                        principalColumn: "PassengerId");
                });

            migrationBuilder.CreateTable(
                name: "ContainerFlight",
                columns: table => new
                {
                    ContainerId = table.Column<int>(type: "integer", nullable: false),
                    FlightId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerFlight", x => new { x.ContainerId, x.FlightId });
                    table.ForeignKey(
                        name: "FK_ContainerFlight_Container_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Container",
                        principalColumn: "ContainerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContainerFlight_Flight_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flight",
                        principalColumn: "FlightId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PassengerFlight",
                columns: table => new
                {
                    PassengerId = table.Column<int>(type: "integer", nullable: false),
                    FlightId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassengerFlight", x => new { x.PassengerId, x.FlightId });
                    table.ForeignKey(
                        name: "FK_PassengerFlight_Flight_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flight",
                        principalColumn: "FlightId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PassengerFlight_Passenger_PassengerId",
                        column: x => x.PassengerId,
                        principalTable: "Passenger",
                        principalColumn: "PassengerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Baggage_ContainerId",
                table: "Baggage",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Baggage_PassengerId",
                table: "Baggage",
                column: "PassengerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContainerFlight_FlightId",
                table: "ContainerFlight",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_SpaceshipName1",
                table: "Flight",
                column: "SpaceshipName1");

            migrationBuilder.CreateIndex(
                name: "IX_Passenger_AnalysisId",
                table: "Passenger",
                column: "AnalysisId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Passenger_InsuranceId",
                table: "Passenger",
                column: "InsuranceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PassengerFlight_FlightId",
                table: "PassengerFlight",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Robot_ContainerId",
                table: "Robot",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Robot_RobotModel",
                table: "Robot",
                column: "RobotModel");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLog_SpaceshipName",
                table: "ServiceLog",
                column: "SpaceshipName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Baggage");

            migrationBuilder.DropTable(
                name: "ContainerFlight");

            migrationBuilder.DropTable(
                name: "PassengerFlight");

            migrationBuilder.DropTable(
                name: "Robot");

            migrationBuilder.DropTable(
                name: "ServiceLog");

            migrationBuilder.DropTable(
                name: "Flight");

            migrationBuilder.DropTable(
                name: "Passenger");

            migrationBuilder.DropTable(
                name: "Container");

            migrationBuilder.DropTable(
                name: "RobotModelCatalog");

            migrationBuilder.DropTable(
                name: "Spaceship");

            migrationBuilder.DropTable(
                name: "FullHealthAnalysisResult");

            migrationBuilder.DropTable(
                name: "Insurance");
        }
    }
}
