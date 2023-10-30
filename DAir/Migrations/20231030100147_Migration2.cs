using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAir.Migrations
{
    /// <inheritdoc />
    public partial class Migration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Certifications",
                columns: table => new
                {
                    CertificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PilotID = table.Column<int>(type: "int", nullable: false),
                    CertificationName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    LicenseNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certification", x => new { x.PilotID, x.CertificationName, x.LicenseNumber });
                    table.ForeignKey(
                        name: "FK_Certification_Pilots_PilotsID",
                        column: x => x.PilotID,
                        principalTable: "Pilots",
                        principalColumn: "PilotID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certification");
        }
    }
}
