using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_violenciagen.Migrations
{
    /// <inheritdoc />
    public partial class AddRutaFotoToPersona : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RutaFoto",
                table: "Personas",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personas_Apellidos",
                table: "Personas",
                column: "Apellidos");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_Nombres",
                table: "Personas",
                column: "Nombres");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_NumeroDocumento",
                table: "Personas",
                column: "NumeroDocumento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Personas_Apellidos",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_Nombres",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_NumeroDocumento",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "RutaFoto",
                table: "Personas");
        }
    }
}
