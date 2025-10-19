using LocadoraLivros.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraLivros.Api.Data.Configurations;

public class EmprestimoItemConfiguration : IEntityTypeConfiguration<EmprestimoItem>
{
    public void Configure(EntityTypeBuilder<EmprestimoItem> builder)
    {
        builder.HasKey(ei => ei.Id);

        builder.Property(ei => ei.DiasEmprestimo)
            .IsRequired();

        builder.Property(ei => ei.ValorDiaria)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(ei => ei.ValorSubtotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(ei => ei.ValorMultaItem)
            .HasColumnType("decimal(18,2)");

        // Relacionamento com Emprestimo
        builder.HasOne(ei => ei.Emprestimo)
            .WithMany(e => e.Itens)
            .HasForeignKey(ei => ei.EmprestimoId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Relacionamento com Livro
        builder.HasOne(ei => ei.Livro)
            .WithMany(l => l.EmprestimoItens)
            .HasForeignKey(ei => ei.LivroId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // Índices para performance
        builder.HasIndex(ei => ei.EmprestimoId)
            .HasDatabaseName("IX_EmprestimoItens_EmprestimoId");

        builder.HasIndex(ei => ei.LivroId)
            .HasDatabaseName("IX_EmprestimoItens_LivroId");

        builder.HasIndex(ei => new { ei.EmprestimoId, ei.LivroId })
            .HasDatabaseName("IX_EmprestimoItens_EmprestimoId_LivroId");
    }
}
