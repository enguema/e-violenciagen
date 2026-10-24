using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_violenciagen.Migrations
{
    /// <inheritdoc />
    public partial class cambioNombreCampoEnActuacionParticipante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActuacionesParticipantes_PersonasInstituciones_PersonaInsti~",
                table: "ActuacionesParticipantes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActuacionesParticipantes",
                table: "ActuacionesParticipantes");

            migrationBuilder.RenameColumn(
                name: "PersonaInstitucionId",
                table: "ActuacionesParticipantes",
                newName: "UsuarioId1");

            migrationBuilder.RenameIndex(
                name: "IX_ActuacionesParticipantes_PersonaInstitucionId",
                table: "ActuacionesParticipantes",
                newName: "IX_ActuacionesParticipantes_UsuarioId1");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "ActuacionesParticipantes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActuacionesParticipantes",
                table: "ActuacionesParticipantes",
                columns: new[] { "ActuacionId", "UsuarioId" });

            migrationBuilder.CreateIndex(
                name: "IX_ActuacionesParticipantes_UsuarioId",
                table: "ActuacionesParticipantes",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActuacionesParticipantes_AspNetUsers_UsuarioId1",
                table: "ActuacionesParticipantes",
                column: "UsuarioId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActuacionesParticipantes_PersonasInstituciones_UsuarioId",
                table: "ActuacionesParticipantes",
                column: "UsuarioId",
                principalTable: "PersonasInstituciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActuacionesParticipantes_AspNetUsers_UsuarioId1",
                table: "ActuacionesParticipantes");

            migrationBuilder.DropForeignKey(
                name: "FK_ActuacionesParticipantes_PersonasInstituciones_UsuarioId",
                table: "ActuacionesParticipantes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActuacionesParticipantes",
                table: "ActuacionesParticipantes");

            migrationBuilder.DropIndex(
                name: "IX_ActuacionesParticipantes_UsuarioId",
                table: "ActuacionesParticipantes");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "ActuacionesParticipantes");

            migrationBuilder.RenameColumn(
                name: "UsuarioId1",
                table: "ActuacionesParticipantes",
                newName: "PersonaInstitucionId");

            migrationBuilder.RenameIndex(
                name: "IX_ActuacionesParticipantes_UsuarioId1",
                table: "ActuacionesParticipantes",
                newName: "IX_ActuacionesParticipantes_PersonaInstitucionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActuacionesParticipantes",
                table: "ActuacionesParticipantes",
                columns: new[] { "ActuacionId", "PersonaInstitucionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ActuacionesParticipantes_PersonasInstituciones_PersonaInsti~",
                table: "ActuacionesParticipantes",
                column: "PersonaInstitucionId",
                principalTable: "PersonasInstituciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
