using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peo.Faturamento.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pagamento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatriculaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(12,2)", precision: 18, scale: 2, nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Pendente"),
                    IdTransacao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Detalhes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DadosDoCartaoCredito",
                columns: table => new
                {
                    PagamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_Pagamento_MatriculaId",
                table: "Pagamento",
                column: "MatriculaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DadosDoCartaoCredito");

            migrationBuilder.DropTable(
                name: "Pagamento");
        }
    }
}
