using LocadoraLivros.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraLivros.Api.Data.Configurations;

public class ConfiguracaoEmprestimoConfiguration : IEntityTypeConfiguration<ConfiguracaoEmprestimo>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoEmprestimo> builder)
    {
        builder.ToTable("ConfiguracoesEmprestimo");

        builder.HasKey(c => c.Id);

        // ========== MULTAS ==========
        builder.Property(c => c.PercentualMultaDiaria)
            .HasColumnType("decimal(5,2)")
            .IsRequired()
            .HasDefaultValue(0.5m)
            .HasComment("Percentual de multa por dia de atraso (ex: 0.5 = 50%)");

        builder.Property(c => c.MultaMaxima)
            .HasColumnType("decimal(10,2)")
            .IsRequired()
            .HasDefaultValue(1000.00m)
            .HasComment("Valor máximo de multa (teto)");

        // ========== LIMITES ==========
        builder.Property(c => c.MaximoLivrosPorEmprestimo)
            .IsRequired()
            .HasDefaultValue(5)
            .HasComment("Quantidade máxima de livros por empréstimo");

        builder.Property(c => c.MaximoEmprestimosAtivosCliente)
            .IsRequired()
            .HasDefaultValue(3)
            .HasComment("Quantidade máxima de empréstimos ativos por cliente");

        builder.Property(c => c.DiasMaximoEmprestimo)
            .IsRequired()
            .HasDefaultValue(30)
            .HasComment("Prazo máximo de empréstimo em dias");

        builder.Property(c => c.DiasMinimosEmprestimo)
            .IsRequired()
            .HasDefaultValue(1)
            .HasComment("Prazo mínimo de empréstimo em dias");

        // ========== RENOVAÇÕES ==========
        builder.Property(c => c.PermiteRenovacao)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Permite renovação de empréstimos");

        builder.Property(c => c.MaximoRenovacoes)
            .IsRequired()
            .HasDefaultValue(2)
            .HasComment("Quantidade máxima de renovações por empréstimo");

        builder.Property(c => c.DiasRenovacao)
            .IsRequired()
            .HasDefaultValue(7)
            .HasComment("Dias adicionados a cada renovação");

        // ========== RESERVAS ==========
        builder.Property(c => c.PermiteReserva)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Permite reserva de livros");

        builder.Property(c => c.DiasValidadeReserva)
            .IsRequired()
            .HasDefaultValue(3)
            .HasComment("Dias de validade da reserva");

        // ========== POLÍTICAS POR TIPO DE CLIENTE ==========
        builder.Property(c => c.DiasAdicionaisClienteVip)
            .IsRequired()
            .HasDefaultValue(7)
            .HasComment("Dias adicionais para clientes VIP");

        builder.Property(c => c.DescontoClienteVip)
            .HasColumnType("decimal(5,2)")
            .IsRequired()
            .HasDefaultValue(0.1m)
            .HasComment("Desconto para clientes VIP (ex: 0.1 = 10%)");

        // ========== AVISOS ==========
        builder.Property(c => c.DiasAvisoVencimento)
            .IsRequired()
            .HasDefaultValue(2)
            .HasComment("Dias antes do vencimento para enviar aviso");

        // ========== BLOQUEIOS ==========
        builder.Property(c => c.DiasAtrasoParaBloqueio)
            .IsRequired()
            .HasDefaultValue(30)
            .HasComment("Dias de atraso para bloquear cliente");

        builder.Property(c => c.ValorDebitoParaBloqueio)
            .HasColumnType("decimal(10,2)")
            .IsRequired()
            .HasDefaultValue(500.00m)
            .HasComment("Valor de débito para bloquear cliente");

        // ========== AUDITORIA ==========
        builder.Property(c => c.DataCriacao)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("Data de criação da configuração");

        builder.Property(c => c.DataAtualizacao)
            .HasComment("Data da última atualização");

        builder.Property(c => c.UsuarioAtualizacao)
            .HasMaxLength(450)
            .HasComment("ID do usuário que fez a última atualização");

        // ========== SEED DATA ==========
        builder.HasData(new ConfiguracaoEmprestimo
        {
            Id = 1,
            PercentualMultaDiaria = 0.5m,
            MultaMaxima = 1000.00m,
            MaximoLivrosPorEmprestimo = 5,
            MaximoEmprestimosAtivosCliente = 3,
            DiasMaximoEmprestimo = 30,
            DiasMinimosEmprestimo = 1,
            PermiteRenovacao = true,
            MaximoRenovacoes = 2,
            DiasRenovacao = 7,
            PermiteReserva = true,
            DiasValidadeReserva = 3,
            DiasAdicionaisClienteVip = 7,
            DescontoClienteVip = 0.1m,
            DiasAvisoVencimento = 2,
            DiasAtrasoParaBloqueio = 30,
            ValorDebitoParaBloqueio = 500.00m,
            DataCriacao = new DateTime(2025, 11, 8, 22, 0, 0, DateTimeKind.Utc)
        });
    }
}
