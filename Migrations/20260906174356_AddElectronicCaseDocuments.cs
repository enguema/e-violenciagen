using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_violenciagen.Migrations
{
    /// <inheritdoc />
    public partial class AddElectronicCaseDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentosExpediente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CasoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActuacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    TipoDocumentoExpedienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    NombreOriginal = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    NombreAlmacenado = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RutaAlmacenamiento = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TipoMime = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Extension = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    TamanoBytes = table.Column<long>(type: "bigint", nullable: false),
                    HashArchivo = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Titulo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FechaDocumento = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaIncorporacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EsConfidencial = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosExpediente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentosExpediente_Actuaciones_ActuacionId",
                        column: x => x.ActuacionId,
                        principalTable: "Actuaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentosExpediente_Casos_CasoId",
                        column: x => x.CasoId,
                        principalTable: "Casos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentosExpediente_TiposDocumentoExpediente_TipoDocumento~",
                        column: x => x.TipoDocumentoExpedienteId,
                        principalTable: "TiposDocumentoExpediente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosExpediente_ActuacionId",
                table: "DocumentosExpediente",
                column: "ActuacionId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosExpediente_CasoId",
                table: "DocumentosExpediente",
                column: "CasoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosExpediente_HashArchivo",
                table: "DocumentosExpediente",
                column: "HashArchivo");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosExpediente_RutaAlmacenamiento",
                table: "DocumentosExpediente",
                column: "RutaAlmacenamiento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosExpediente_TipoDocumentoExpedienteId",
                table: "DocumentosExpediente",
                column: "TipoDocumentoExpedienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentosExpediente");
        }
    }
}
