using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peo.GestaoConteudo.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoteUsuarioFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Curso_Usuario_InstrutorId",
                table: "Curso");

            //migrationBuilder.DropTable(
            //    name: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_Curso_InstrutorId",
                table: "Curso");
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

            migrationBuilder.CreateIndex(
                name: "IX_Curso_InstrutorId",
                table: "Curso",
                column: "InstrutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Curso_Usuario_InstrutorId",
                table: "Curso",
                column: "InstrutorId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
