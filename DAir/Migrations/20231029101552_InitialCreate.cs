using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAir.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeID);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    FlightCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartureAirport = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArrivalAirport = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduledDepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AircraftID = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.FlightCode);
                });

            migrationBuilder.CreateTable(
                name: "CabinMembers",
                columns: table => new
                {
                    CabinMemberID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    GeoLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Certification = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CabinMembers", x => x.CabinMemberID);
                    table.ForeignKey(
                        name: "FK_CabinMembers_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Crews",
                columns: table => new
                {
                    CrewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pilot = table.Column<int>(type: "int", nullable: false),
                    CoPilot = table.Column<int>(type: "int", nullable: false),
                    Pursuer = table.Column<int>(type: "int", nullable: false),
                    FlightAttendant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crews", x => x.CrewID);
                    table.ForeignKey(
                        name: "FK_Crews_Employees_CoPilot",
                        column: x => x.CoPilot,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Crews_Employees_FlightAttendant",
                        column: x => x.FlightAttendant,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Crews_Employees_Pilot",
                        column: x => x.Pilot,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Crews_Employees_Pursuer",
                        column: x => x.Pursuer,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pilots",
                columns: table => new
                {
                    PilotID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    GeoLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Certification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rank = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pilots", x => x.PilotID);
                    table.ForeignKey(
                        name: "FK_Pilots_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlightSchedules",
                columns: table => new
                {
                    FlightCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightSchedules", x => new { x.FlightCode, x.EmployeeID });
                    table.ForeignKey(
                        name: "FK_FlightSchedules_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightSchedules_Flights_FlightCode",
                        column: x => x.FlightCode,
                        principalTable: "Flights",
                        principalColumn: "FlightCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CabinMemberID = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Languages_CabinMembers_CabinMemberID",
                        column: x => x.CabinMemberID,
                        principalTable: "CabinMembers",
                        principalColumn: "CabinMemberID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    RaterID = table.Column<int>(type: "int", nullable: false),
                    RateeID = table.Column<int>(type: "int", nullable: false),
                    RatingValue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => new { x.RaterID, x.RateeID });
                    table.ForeignKey(
                        name: "FK_Ratings_Employees_RaterID",
                        column: x => x.RaterID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ratings_Pilots_RateeID",
                        column: x => x.RateeID,
                        principalTable: "Pilots",
                        principalColumn: "PilotID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CabinMembers_EmployeeID",
                table: "CabinMembers",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Crews_CoPilot",
                table: "Crews",
                column: "CoPilot");

            migrationBuilder.CreateIndex(
                name: "IX_Crews_FlightAttendant",
                table: "Crews",
                column: "FlightAttendant");

            migrationBuilder.CreateIndex(
                name: "IX_Crews_Pilot",
                table: "Crews",
                column: "Pilot");

            migrationBuilder.CreateIndex(
                name: "IX_Crews_Pursuer",
                table: "Crews",
                column: "Pursuer");

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_EmployeeID",
                table: "FlightSchedules",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CabinMemberID",
                table: "Languages",
                column: "CabinMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_Pilots_EmployeeID",
                table: "Pilots",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_RateeID",
                table: "Ratings",
                column: "RateeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Crews");

            migrationBuilder.DropTable(
                name: "FlightSchedules");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "CabinMembers");

            migrationBuilder.DropTable(
                name: "Pilots");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
