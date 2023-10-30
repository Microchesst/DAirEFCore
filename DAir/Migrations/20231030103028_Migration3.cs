using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAir.Migrations
{
    /// <inheritdoc />
    public partial class Migration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conflicts",
                columns: table => new
                {
                    ConflictID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PilotID = table.Column<int>(type: "int", nullable: false),
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conflict", x => new { x.PilotID, x.EmployeeID });
                    table.ForeignKey(
                        name: "FK_Conflicts_Pilots_PilotsID",
                        column: x => x.PilotID,
                        principalTable: "Pilots",
                        principalColumn: "PilotID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                         name: "FK_Conflicts_Employees_EmployeeID",
                         column: x => x.EmployeeID,
                         principalTable: "Employees",
                         principalColumn: "EmployeeID",
                         onDelete: ReferentialAction.Restrict);

                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Conflicts");
        }
    }
}
