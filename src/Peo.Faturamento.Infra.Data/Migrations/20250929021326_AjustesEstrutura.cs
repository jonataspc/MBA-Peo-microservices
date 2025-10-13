using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peo.Faturamento.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AjustesEstrutura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartaoCreditoData");

            migrationBuilder.CreateTable(
                name: "DadosDoCartaoCredito",
                columns: table => new
                {
                    PagamentoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Hash = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadosDoCartaoCredito", x => x.PagamentoId);
                    table.ForeignKey(
                        name: "FK_DadosDoCartaoCredito_Pagamento_PagamentoId",
                        column: x => x.PagamentoId,
                        principalTable: "Pagamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DadosDoCartaoCredito");

            migrationBuilder.CreateTable(
                name: "CartaoCreditoData",
                columns: table => new
                {
                    PagamentoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Hash = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartaoCreditoData", x => x.PagamentoId);
                    table.ForeignKey(
                        name: "FK_CartaoCreditoData_Pagamento_PagamentoId",
                        column: x => x.PagamentoId,
                        principalTable: "Pagamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
