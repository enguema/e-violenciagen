using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_violenciagen.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonasAndInstitucionalActor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TiposDocumentoIdentidad",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDocumentoIdentidad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoDocumentoIdentidadId = table.Column<Guid>(type: "uuid", nullable: true),
                    NumeroDocumento = table.Column<string>(type: "text", nullable: true),
                    Nombres = table.Column<string>(type: "text", nullable: false),
                    Apellidos = table.Column<string>(type: "text", nullable: false),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    Sexo = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Direccion = table.Column<string>(type: "text", nullable: true),
                    BarrioId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personas_Barrios_BarrioId",
                        column: x => x.BarrioId,
                        principalTable: "Barrios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Personas_TiposDocumentoIdentidad_TipoDocumentoIdentidadId",
                        column: x => x.TipoDocumentoIdentidadId,
                        principalTable: "TiposDocumentoIdentidad",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PersonasInstituciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonaId = table.Column<Guid>(type: "uuid", nullable: false),
                    InstitucionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnidadOrganizativaId = table.Column<Guid>(type: "uuid", nullable: true),
                    Cargo = table.Column<string>(type: "text", nullable: true),
                    CodigoProfesional = table.Column<string>(type: "text", nullable: true),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonasInstituciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonasInstituciones_Instituciones_InstitucionId",
                        column: x => x.InstitucionId,
                        principalTable: "Instituciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonasInstituciones_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonasInstituciones_UnidadesOrganizativas_UnidadOrganizat~",
                        column: x => x.UnidadOrganizativaId,
                        principalTable: "UnidadesOrganizativas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personas_BarrioId",
                table: "Personas",
                column: "BarrioId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_TipoDocumentoIdentidadId",
                table: "Personas",
                column: "TipoDocumentoIdentidadId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonasInstituciones_InstitucionId",
                table: "PersonasInstituciones",
                column: "InstitucionId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonasInstituciones_PersonaId",
                table: "PersonasInstituciones",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonasInstituciones_UnidadOrganizativaId",
                table: "PersonasInstituciones",
                column: "UnidadOrganizativaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonasInstituciones");

            migrationBuilder.DropTable(
                name: "Personas");

            migrationBuilder.DropTable(
                name: "TiposDocumentoIdentidad");
        }
    }
}
