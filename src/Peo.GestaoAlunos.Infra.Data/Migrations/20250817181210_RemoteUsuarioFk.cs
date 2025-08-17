using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peo.GestaoAlunos.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoteUsuarioFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estudante_Usuario_UsuarioId",
                table: "Estudante");

            //migrationBuilder.DropTable(
            //    name: "Usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Usuario",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "TEXT", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
            //        Email = table.Column<string>(type: "TEXT", nullable: false),
            //        ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
            //        NomeCompleto = table.Column<string>(type: "TEXT", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Usuario", x => x.Id);
            //    });

            migrationBuilder.AddForeignKey(
                name: "FK_Estudante_Usuario_UsuarioId",
                table: "Estudante",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
