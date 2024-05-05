using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMIApp.Migrations.HMIAppDBContextArchivizationMigrations
{
    /// <inheritdoc />
    public partial class AddParameterNrOfSchift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NrOfShift",
                table: "ArchivizationsForParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NrOfShift",
                table: "ArchivizationsForParameters");
        }
    }
}
