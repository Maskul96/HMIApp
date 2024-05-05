using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMIApp.Migrations.HMIAppDBContextArchivizationMigrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchivizationsForParameters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateAndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NrOfCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameOfClient = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrzeciskanieP1 = table.Column<bool>(type: "bit", nullable: false),
                    MontazOslonkiP2 = table.Column<bool>(type: "bit", nullable: false),
                    OetickerP3 = table.Column<bool>(type: "bit", nullable: false),
                    DyszaWahliwaP4 = table.Column<bool>(type: "bit", nullable: false),
                    SmarTulipP5 = table.Column<bool>(type: "bit", nullable: false),
                    SmarPrzegubP6 = table.Column<bool>(type: "bit", nullable: false),
                    TraceUpP7 = table.Column<bool>(type: "bit", nullable: false),
                    TraceUpZapisP8 = table.Column<bool>(type: "bit", nullable: false),
                    RFIDGlowicaGornaP9 = table.Column<bool>(type: "bit", nullable: false),
                    RFIDPlytaSmarujacaP10 = table.Column<bool>(type: "bit", nullable: false),
                    RFIDSzczekiOslonkiP11 = table.Column<bool>(type: "bit", nullable: false),
                    RFIDGniazdoPrzegubuP12 = table.Column<bool>(type: "bit", nullable: false),
                    PozwyjeciaOsi = table.Column<float>(type: "real", nullable: false),
                    PozOetickera = table.Column<float>(type: "real", nullable: false),
                    Przeciskanie_PozStartowa = table.Column<float>(type: "real", nullable: false),
                    Przeciskanie_DojazdWolny = table.Column<float>(type: "real", nullable: false),
                    Przeciskanie_PoczCzytSily = table.Column<float>(type: "real", nullable: false),
                    Przeciskanie_KoniecCzytSily = table.Column<float>(type: "real", nullable: false),
                    Przeciskanie_SilaMin = table.Column<int>(type: "int", nullable: false),
                    Przeciskanie_SilaMax = table.Column<int>(type: "int", nullable: false),
                    Oslonka_PozStartowa = table.Column<float>(type: "real", nullable: false),
                    Oslonka_PozSmarowania = table.Column<float>(type: "real", nullable: false),
                    Oslonka_PozNakladania = table.Column<float>(type: "real", nullable: false),
                    Oslonka_PozPowrotu = table.Column<float>(type: "real", nullable: false),
                    DyszaWahliwa_PozPionowa = table.Column<float>(type: "real", nullable: false),
                    DyszaWahliwa_PozPozioma = table.Column<float>(type: "real", nullable: false),
                    DyszaWahliwa_PozdyszywOslonce = table.Column<float>(type: "real", nullable: false),
                    DyszaWahliwa_PozZjazduOslonkiSmarowanie = table.Column<float>(type: "real", nullable: false),
                    Smarowanie_DawkaPrzegub = table.Column<float>(type: "real", nullable: false),
                    Smarowanie_TolDawkiPrzegub = table.Column<float>(type: "real", nullable: false),
                    Smarowanie_RodzajSmaruPrzegub = table.Column<int>(type: "int", nullable: false),
                    Smarowanie_DawkaTulip = table.Column<float>(type: "real", nullable: false),
                    Smarowanie_TolDawkiTulip = table.Column<float>(type: "real", nullable: false),
                    Smarowanie_RodzajSmaruTulip = table.Column<int>(type: "int", nullable: false),
                    TagRFID_GlowicaGorna = table.Column<int>(type: "int", nullable: false),
                    TagRFID_PlytaSmar = table.Column<int>(type: "int", nullable: false),
                    TagRFID_SzczekiOslonki = table.Column<int>(type: "int", nullable: false),
                    TagRFID_Przegub = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivizationsForParameters", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivizationsForParameters");
        }
    }
}
