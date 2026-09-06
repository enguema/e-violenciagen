using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_violenciagen.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposOlvodados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "UnidadesOrganizativas",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "TiposViolencia",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "TiposInstitucion",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "TiposDocumentoIdentidad",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "TiposDocumentoExpediente",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "TiposActuacion",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Provincias",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "PersonasInstituciones",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Personas",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Instituciones",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "EstadosCaso",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "DocumentosExpediente",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Distritos",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Casos",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Barrios",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Actuaciones",
                newName: "Activo");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "UnidadesOrganizativas",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "UnidadesOrganizativas");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "UnidadesOrganizativas",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "TiposViolencia",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "TiposInstitucion",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "TiposDocumentoIdentidad",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "TiposDocumentoExpediente",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "TiposActuacion",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "Provincias",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "PersonasInstituciones",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "Personas",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "Instituciones",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "EstadosCaso",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "DocumentosExpediente",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "Distritos",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "Casos",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "Barrios",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "Actuaciones",
                newName: "IsActive");
        }
    }
}
