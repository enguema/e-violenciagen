using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_violenciagen.Migrations
{
    /// <inheritdoc />
    public partial class AddDistritoProcedenciaToPersona : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DistritoProcedenciaId",
                table: "Personas",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personas_DistritoProcedenciaId",
                table: "Personas",
                column: "DistritoProcedenciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Distritos_DistritoProcedenciaId",
                table: "Personas",
                column: "DistritoProcedenciaId",
                principalTable: "Distritos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Distritos_DistritoProcedenciaId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_DistritoProcedenciaId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "DistritoProcedenciaId",
                table: "Personas");
        }
    }
}
