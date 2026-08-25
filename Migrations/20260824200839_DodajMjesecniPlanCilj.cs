using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UstediPametno.Migrations
{
    /// <inheritdoc />
    public partial class DodajMjesecniPlanCilj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MjesecniPlanCilj",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MjesecniPlanId = table.Column<int>(type: "int", nullable: false),
                    CiljStednjeId = table.Column<int>(type: "int", nullable: false),
                    PlaniraniIznos = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MjesecniPlanCilj", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MjesecniPlanCilj_CiljStednje_CiljStednjeId",
                        column: x => x.CiljStednjeId,
                        principalTable: "CiljStednje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MjesecniPlanCilj_MjesecniPlan_MjesecniPlanId",
                        column: x => x.MjesecniPlanId,
                        principalTable: "MjesecniPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MjesecniPlanCilj_CiljStednjeId",
                table: "MjesecniPlanCilj",
                column: "CiljStednjeId");

            migrationBuilder.CreateIndex(
                name: "IX_MjesecniPlanCilj_MjesecniPlanId",
                table: "MjesecniPlanCilj",
                column: "MjesecniPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MjesecniPlanCilj");
        }
    }
}
