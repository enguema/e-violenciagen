using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_violenciagen.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseVictimsAndAllegedAggressors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CasosPresuntosAgresores",
                columns: table => new
                {
                    CasoId = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonaId = table.Column<Guid>(type: "uuid", nullable: false),
                    RelacionConVictima = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FechaVinculacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasosPresuntosAgresores", x => new { x.CasoId, x.PersonaId });
                    table.ForeignKey(
                        name: "FK_CasosPresuntosAgresores_Casos_CasoId",
                        column: x => x.CasoId,
                        principalTable: "Casos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CasosPresuntosAgresores_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CasosVictimas",
                columns: table => new
                {
                    CasoId = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonaId = table.Column<Guid>(type: "uuid", nullable: false),
                    EsVictimaPrincipal = table.Column<bool>(type: "boolean", nullable: false),
                    RequiereProteccion = table.Column<bool>(type: "boolean", nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FechaVinculacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasosVictimas", x => new { x.CasoId, x.PersonaId });
                    table.ForeignKey(
                        name: "FK_CasosVictimas_Casos_CasoId",
                        column: x => x.CasoId,
                        principalTable: "Casos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CasosVictimas_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CasosPresuntosAgresores_PersonaId",
                table: "CasosPresuntosAgresores",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_CasosVictimas_PersonaId",
                table: "CasosVictimas",
                column: "PersonaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CasosPresuntosAgresores");

            migrationBuilder.DropTable(
                name: "CasosVictimas");
        }
    }
}
