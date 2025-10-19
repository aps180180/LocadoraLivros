using LocadoraLivros.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraLivros.Api.Data.Configurations;

public class EmprestimoConfiguration : IEntityTypeConfiguration<Emprestimo>
{
    public void Configure(EntityTypeBuilder<Emprestimo> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.DataEmprestimo)
            .IsRequired();

        builder.Property(e => e.DataPrevisaoDevolucao)
            .IsRequired();

        builder.Property(e => e.ValorTotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(e => e.ValorMulta)
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Observacoes)
            .HasMaxLength(500);

        // Relacionamento com Cliente
        builder.HasOne(e => e.Cliente)
            .WithMany(c => c.Emprestimos)
            .HasForeignKey(e => e.ClienteId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // Relacionamento com Itens
        builder.HasMany(e => e.Itens)
            .WithOne(i => i.Emprestimo)
            .HasForeignKey(i => i.EmprestimoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices para performance
        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_Emprestimos_Status");

        builder.HasIndex(e => e.DataEmprestimo)
            .HasDatabaseName("IX_Emprestimos_DataEmprestimo");

        builder.HasIndex(e => e.DataPrevisaoDevolucao)
            .HasDatabaseName("IX_Emprestimos_DataPrevisaoDevolucao");

        builder.HasIndex(e => new { e.ClienteId, e.Status })
            .HasDatabaseName("IX_Emprestimos_ClienteId_Status");
    }
}
