using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peo.GestaoConteudo.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorrecaoCursoInstrutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstrutorId",
                table: "Curso");

            migrationBuilder.AddColumn<string>(
                name: "InstrutorNome",
                table: "Curso",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstrutorNome",
                table: "Curso");

            migrationBuilder.AddColumn<Guid>(
                name: "InstrutorId",
                table: "Curso",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
