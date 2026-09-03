using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_violenciagen.Migrations
{
    /// <inheritdoc />
    public partial class AddViolenciaCases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Barrios_BarrioId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_TiposDocumentoIdentidad_TipoDocumentoIdentidadId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonasInstituciones_Instituciones_InstitucionId",
                table: "PersonasInstituciones");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonasInstituciones_Personas_PersonaId",
                table: "PersonasInstituciones");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonasInstituciones_UnidadesOrganizativas_UnidadOrganizat~",
                table: "PersonasInstituciones");

            migrationBuilder.DropIndex(
                name: "IX_Personas_TipoDocumentoIdentidadId",
                table: "Personas");

            migrationBuilder.AlterColumn<string>(
                name: "CodigoProfesional",
                table: "PersonasInstituciones",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cargo",
                table: "PersonasInstituciones",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstitucionId1",
                table: "PersonasInstituciones",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnidadOrganizativaId1",
                table: "PersonasInstituciones",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombres",
                table: "Personas",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Apellidos",
                table: "Personas",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "Casos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoCaso = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaHecho = table.Column<DateOnly>(type: "date", nullable: true),
                    EstadoCasoId = table.Column<Guid>(type: "uuid", nullable: false),
                    BarrioHechoId = table.Column<Guid>(type: "uuid", nullable: true),
                    LugarDescripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Resumen = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RelatoInicial = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Casos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Casos_Barrios_BarrioHechoId",
                        column: x => x.BarrioHechoId,
                        principalTable: "Barrios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Casos_EstadosCaso_EstadoCasoId",
                        column: x => x.EstadoCasoId,
                        principalTable: "EstadosCaso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CasosTiposViolencia",
                columns: table => new
                {
                    CasoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoViolenciaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasosTiposViolencia", x => new { x.CasoId, x.TipoViolenciaId });
                    table.ForeignKey(
                        name: "FK_CasosTiposViolencia_Casos_CasoId",
                        column: x => x.CasoId,
                        principalTable: "Casos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CasosTiposViolencia_TiposViolencia_TipoViolenciaId",
                        column: x => x.TipoViolenciaId,
                        principalTable: "TiposViolencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonasInstituciones_InstitucionId1",
                table: "PersonasInstituciones",
                column: "InstitucionId1");

            migrationBuilder.CreateIndex(
                name: "IX_PersonasInstituciones_UnidadOrganizativaId1",
                table: "PersonasInstituciones",
                column: "UnidadOrganizativaId1");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_TipoDocumentoIdentidadId_NumeroDocumento",
                table: "Personas",
                columns: new[] { "TipoDocumentoIdentidadId", "NumeroDocumento" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Casos_BarrioHechoId",
                table: "Casos",
                column: "BarrioHechoId");

            migrationBuilder.CreateIndex(
                name: "IX_Casos_CodigoCaso",
                table: "Casos",
                column: "CodigoCaso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Casos_EstadoCasoId",
                table: "Casos",
                column: "EstadoCasoId");

            migrationBuilder.CreateIndex(
                name: "IX_Casos_FechaRegistro",
                table: "Casos",
                column: "FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_CasosTiposViolencia_TipoViolenciaId",
                table: "CasosTiposViolencia",
                column: "TipoViolenciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Barrios_BarrioId",
                table: "Personas",
                column: "BarrioId",
                principalTable: "Barrios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_TiposDocumentoIdentidad_TipoDocumentoIdentidadId",
                table: "Personas",
                column: "TipoDocumentoIdentidadId",
                principalTable: "TiposDocumentoIdentidad",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonasInstituciones_Instituciones_InstitucionId",
                table: "PersonasInstituciones",
                column: "InstitucionId",
                principalTable: "Instituciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonasInstituciones_Instituciones_InstitucionId1",
                table: "PersonasInstituciones",
                column: "InstitucionId1",
                principalTable: "Instituciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonasInstituciones_Personas_PersonaId",
                table: "PersonasInstituciones",
                column: "PersonaId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonasInstituciones_UnidadesOrganizativas_UnidadOrganizat~",
                table: "PersonasInstituciones",
                column: "UnidadOrganizativaId",
                principalTable: "UnidadesOrganizativas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonasInstituciones_UnidadesOrganizativas_UnidadOrganiza~1",
                table: "PersonasInstituciones",
                column: "UnidadOrganizativaId1",
                principalTable: "UnidadesOrganizativas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Barrios_BarrioId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_TiposDocumentoIdentidad_TipoDocumentoIdentidadId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonasInstituciones_Instituciones_InstitucionId",
                table: "PersonasInstituciones");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonasInstituciones_Instituciones_InstitucionId1",
                table: "PersonasInstituciones");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonasInstituciones_Personas_PersonaId",
                table: "PersonasInstituciones");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonasInstituciones_UnidadesOrganizativas_UnidadOrganizat~",
                table: "PersonasInstituciones");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonasInstituciones_UnidadesOrganizativas_UnidadOrganiza~1",
                table: "PersonasInstituciones");

            migrationBuilder.DropTable(
                name: "CasosTiposViolencia");

            migrationBuilder.DropTable(
                name: "Casos");

            migrationBuilder.DropIndex(
                name: "IX_PersonasInstituciones_InstitucionId1",
                table: "PersonasInstituciones");

            migrationBuilder.DropIndex(
                name: "IX_PersonasInstituciones_UnidadOrganizativaId1",
                table: "PersonasInstituciones");

            migrationBuilder.DropIndex(
                name: "IX_Personas_TipoDocumentoIdentidadId_NumeroDocumento",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "InstitucionId1",
                table: "PersonasInstituciones");

            migrationBuilder.DropColumn(
                name: "UnidadOrganizativaId1",
                table: "PersonasInstituciones");

            migrationBuilder.AlterColumn<string>(
                name: "CodigoProfesional",
                table: "PersonasInstituciones",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cargo",
                table: "PersonasInstituciones",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombres",
                table: "Personas",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Apellidos",
                table: "Personas",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personas_TipoDocumentoIdentidadId",
                table: "Personas",
                column: "TipoDocumentoIdentidadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Barrios_BarrioId",
                table: "Personas",
                column: "BarrioId",
                principalTable: "Barrios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_TiposDocumentoIdentidad_TipoDocumentoIdentidadId",
                table: "Personas",
                column: "TipoDocumentoIdentidadId",
                principalTable: "TiposDocumentoIdentidad",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonasInstituciones_Instituciones_InstitucionId",
                table: "PersonasInstituciones",
                column: "InstitucionId",
                principalTable: "Instituciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonasInstituciones_Personas_PersonaId",
                table: "PersonasInstituciones",
                column: "PersonaId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonasInstituciones_UnidadesOrganizativas_UnidadOrganizat~",
                table: "PersonasInstituciones",
                column: "UnidadOrganizativaId",
                principalTable: "UnidadesOrganizativas",
                principalColumn: "Id");
        }
    }
}
