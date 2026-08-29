using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UstediPametno.Migrations
{
    /// <inheritdoc />
    public partial class DodaniBedzevi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bedz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vrsta = table.Column<int>(type: "int", nullable: false),
                    Prag = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ikona = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bedz", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KorisnikBedz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BedzId = table.Column<int>(type: "int", nullable: false),
                    DatumOsvajanja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KorisnikBedz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KorisnikBedz_Bedz_BedzId",
                        column: x => x.BedzId,
                        principalTable: "Bedz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KorisnikBedz_Korisnik_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Bedz",
                columns: new[] { "Id", "Ikona", "Naziv", "Opis", "Prag", "Vrsta" },
                values: new object[,]
                {
                    { 1, "🌱", "Prvi korak", "Ušteđeno prvih 100 KM", 100m, 0 },
                    { 2, "🪵", "Drveni štediša", "Ušteđeno 500 KM", 500m, 0 },
                    { 3, "🥉", "Bronzani štediša", "Ušteđeno 1.000 KM", 1000m, 0 },
                    { 4, "🥈", "Srebreni štediša", "Ušteđeno 2.500 KM", 2500m, 0 },
                    { 5, "🥇", "Zlatni štediša", "Ušteđeno 5.000 KM", 5000m, 0 },
                    { 6, "💎", "Dijamantni štediša", "Ušteđeno 10.000 KM", 10000m, 0 },
                    { 7, "🌱", "Mjesec dana s nama", "Član aplikacije najmanje 1 mjesec", 1m, 1 },
                    { 8, "🤝", "Vjerni član", "Član aplikacije najmanje 3 mjeseca", 3m, 1 },
                    { 9, "⭐", "Pola godine s nama", "Član aplikacije najmanje 6 mjeseci", 6m, 1 },
                    { 10, "🎂", "Godinu dana s nama", "Član aplikacije najmanje 12 mjeseci", 12m, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_KorisnikBedz_BedzId",
                table: "KorisnikBedz",
                column: "BedzId");

            migrationBuilder.CreateIndex(
                name: "IX_KorisnikBedz_KorisnikId_BedzId",
                table: "KorisnikBedz",
                columns: new[] { "KorisnikId", "BedzId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KorisnikBedz");

            migrationBuilder.DropTable(
                name: "Bedz");
        }
    }
}
