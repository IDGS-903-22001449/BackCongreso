using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app_congreso.Migrations
{
    /// <inheritdoc />
    public partial class TablaUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Participantes",
                table: "Participantes");

            migrationBuilder.RenameTable(
                name: "Participantes",
                newName: "usuarios");

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuarios",
                table: "usuarios",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_usuarios",
                table: "usuarios");

            migrationBuilder.RenameTable(
                name: "usuarios",
                newName: "Participantes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Participantes",
                table: "Participantes",
                column: "Id");
        }
    }
}
