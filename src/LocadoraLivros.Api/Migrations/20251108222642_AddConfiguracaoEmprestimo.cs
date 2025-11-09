using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocadoraLivros.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracaoEmprestimo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracoesEmprestimo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PercentualMultaDiaria = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0.5m, comment: "Percentual de multa por dia de atraso (ex: 0.5 = 50%)"),
                    MultaMaxima = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 1000.00m, comment: "Valor máximo de multa (teto)"),
                    MaximoLivrosPorEmprestimo = table.Column<int>(type: "int", nullable: false, defaultValue: 5, comment: "Quantidade máxima de livros por empréstimo"),
                    MaximoEmprestimosAtivosCliente = table.Column<int>(type: "int", nullable: false, defaultValue: 3, comment: "Quantidade máxima de empréstimos ativos por cliente"),
                    DiasMaximoEmprestimo = table.Column<int>(type: "int", nullable: false, defaultValue: 30, comment: "Prazo máximo de empréstimo em dias"),
                    DiasMinimosEmprestimo = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "Prazo mínimo de empréstimo em dias"),
                    PermiteRenovacao = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Permite renovação de empréstimos"),
                    MaximoRenovacoes = table.Column<int>(type: "int", nullable: false, defaultValue: 2, comment: "Quantidade máxima de renovações por empréstimo"),
                    DiasRenovacao = table.Column<int>(type: "int", nullable: false, defaultValue: 7, comment: "Dias adicionados a cada renovação"),
                    PermiteReserva = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Permite reserva de livros"),
                    DiasValidadeReserva = table.Column<int>(type: "int", nullable: false, defaultValue: 3, comment: "Dias de validade da reserva"),
                    DiasAdicionaisClienteVip = table.Column<int>(type: "int", nullable: false, defaultValue: 7, comment: "Dias adicionais para clientes VIP"),
                    DescontoClienteVip = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0.1m, comment: "Desconto para clientes VIP (ex: 0.1 = 10%)"),
                    DiasAvisoVencimento = table.Column<int>(type: "int", nullable: false, defaultValue: 2, comment: "Dias antes do vencimento para enviar aviso"),
                    DiasAtrasoParaBloqueio = table.Column<int>(type: "int", nullable: false, defaultValue: 30, comment: "Dias de atraso para bloquear cliente"),
                    ValorDebitoParaBloqueio = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 500.00m, comment: "Valor de débito para bloquear cliente"),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Data de criação da configuração"),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Data da última atualização"),
                    UsuarioAtualizacao = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true, comment: "ID do usuário que fez a última atualização")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracoesEmprestimo", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ConfiguracoesEmprestimo",
                columns: new[] { "Id", "DataAtualizacao", "DataCriacao", "DescontoClienteVip", "DiasAdicionaisClienteVip", "DiasAtrasoParaBloqueio", "DiasAvisoVencimento", "DiasMaximoEmprestimo", "DiasMinimosEmprestimo", "DiasRenovacao", "DiasValidadeReserva", "MaximoEmprestimosAtivosCliente", "MaximoLivrosPorEmprestimo", "MaximoRenovacoes", "MultaMaxima", "PercentualMultaDiaria", "PermiteRenovacao", "PermiteReserva", "UsuarioAtualizacao", "ValorDebitoParaBloqueio" },
                values: new object[] { 1, null, new DateTime(2025, 11, 8, 22, 0, 0, 0, DateTimeKind.Utc), 0.1m, 7, 30, 2, 30, 1, 7, 3, 3, 5, 2, 1000.00m, 0.5m, true, true, null, 500.00m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracoesEmprestimo");
        }
    }
}
