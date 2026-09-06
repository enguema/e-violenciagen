using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_violenciagen.Migrations
{
    /// <inheritdoc />
    public partial class AddInstitutionalActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actuaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CasoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoActuacionId = table.Column<Guid>(type: "uuid", nullable: false),
                    InstitucionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnidadOrganizativaId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaActuacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Resultado = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actuaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actuaciones_Casos_CasoId",
                        column: x => x.CasoId,
                        principalTable: "Casos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Actuaciones_Instituciones_InstitucionId",
                        column: x => x.InstitucionId,
                        principalTable: "Instituciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Actuaciones_TiposActuacion_TipoActuacionId",
                        column: x => x.TipoActuacionId,
                        principalTable: "TiposActuacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Actuaciones_UnidadesOrganizativas_UnidadOrganizativaId",
                        column: x => x.UnidadOrganizativaId,
                        principalTable: "UnidadesOrganizativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActuacionesParticipantes",
                columns: table => new
                {
                    ActuacionId = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonaInstitucionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RolEnActuacion = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    EsResponsable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActuacionesParticipantes", x => new { x.ActuacionId, x.PersonaInstitucionId });
                    table.ForeignKey(
                        name: "FK_ActuacionesParticipantes_Actuaciones_ActuacionId",
                        column: x => x.ActuacionId,
                        principalTable: "Actuaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActuacionesParticipantes_PersonasInstituciones_PersonaInsti~",
                        column: x => x.PersonaInstitucionId,
                        principalTable: "PersonasInstituciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actuaciones_CasoId_FechaActuacion",
                table: "Actuaciones",
                columns: new[] { "CasoId", "FechaActuacion" });

            migrationBuilder.CreateIndex(
                name: "IX_Actuaciones_InstitucionId",
                table: "Actuaciones",
                column: "InstitucionId");

            migrationBuilder.CreateIndex(
                name: "IX_Actuaciones_TipoActuacionId",
                table: "Actuaciones",
                column: "TipoActuacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Actuaciones_UnidadOrganizativaId",
                table: "Actuaciones",
                column: "UnidadOrganizativaId");

            migrationBuilder.CreateIndex(
                name: "IX_ActuacionesParticipantes_PersonaInstitucionId",
                table: "ActuacionesParticipantes",
                column: "PersonaInstitucionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActuacionesParticipantes");

            migrationBuilder.DropTable(
                name: "Actuaciones");
        }
    }
}
