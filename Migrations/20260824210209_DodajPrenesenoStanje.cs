using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UstediPametno.Migrations
{
    /// <inheritdoc />
    public partial class DodajPrenesenoStanje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrenesenoIzPrethodnogMjeseca",
                table: "MjesecniPlan",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrenesenoIzPrethodnogMjeseca",
                table: "MjesecniPlan");
        }
    }
}
