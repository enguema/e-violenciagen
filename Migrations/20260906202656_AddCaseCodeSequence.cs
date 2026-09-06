using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_violenciagen.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseCodeSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "CasoCodigoSequence");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "CasoCodigoSequence");
        }
    }
}
