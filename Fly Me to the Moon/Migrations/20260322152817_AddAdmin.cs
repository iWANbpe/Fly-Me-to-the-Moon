using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fly_Me_to_the_Moon.Migrations
{
    /// <inheritdoc />
    public partial class AddAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_passenger_full_health_analysis_result_AnalysisId",
                table: "passenger");

            migrationBuilder.DropForeignKey(
                name: "FK_passenger_insurance_InsuranceId",
                table: "passenger");

            migrationBuilder.AlterColumn<int>(
                name: "InsuranceId",
                table: "passenger",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AnalysisId",
                table: "passenger",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "passenger",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<double>(
                name: "MaxWeight",
                table: "baggage",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "admin",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdminName = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin", x => x.AdminId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_passenger_full_health_analysis_result_AnalysisId",
                table: "passenger",
                column: "AnalysisId",
                principalTable: "full_health_analysis_result",
                principalColumn: "AnalysisId");

            migrationBuilder.AddForeignKey(
                name: "FK_passenger_insurance_InsuranceId",
                table: "passenger",
                column: "InsuranceId",
                principalTable: "insurance",
                principalColumn: "InsuranceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_passenger_full_health_analysis_result_AnalysisId",
                table: "passenger");

            migrationBuilder.DropForeignKey(
                name: "FK_passenger_insurance_InsuranceId",
                table: "passenger");

            migrationBuilder.DropTable(
                name: "admin");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "passenger");

            migrationBuilder.AlterColumn<int>(
                name: "InsuranceId",
                table: "passenger",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AnalysisId",
                table: "passenger",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaxWeight",
                table: "baggage",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddForeignKey(
                name: "FK_passenger_full_health_analysis_result_AnalysisId",
                table: "passenger",
                column: "AnalysisId",
                principalTable: "full_health_analysis_result",
                principalColumn: "AnalysisId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_passenger_insurance_InsuranceId",
                table: "passenger",
                column: "InsuranceId",
                principalTable: "insurance",
                principalColumn: "InsuranceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
