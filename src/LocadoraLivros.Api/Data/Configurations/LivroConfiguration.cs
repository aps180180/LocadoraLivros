using LocadoraLivros.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraLivros.Api.Data.Configurations;

public class LivroConfiguration : IEntityTypeConfiguration<Livro>
{
    public void Configure(EntityTypeBuilder<Livro> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Titulo)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.ISBN)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(l => l.Autor)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(l => l.Editora)
            .HasMaxLength(100);

        builder.Property(l => l.Categoria)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.ValorDiaria)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(l => l.ImagemUrl)
            .HasMaxLength(500);

        builder.Property(l => l.DataCadastro)
            .IsRequired();

        builder.Property(l => l.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        // Índice único no ISBN
        builder.HasIndex(l => l.ISBN)
            .IsUnique()
            .HasDatabaseName("IX_Livros_ISBN");

        // Índice na categoria para pesquisas
        builder.HasIndex(l => l.Categoria)
            .HasDatabaseName("IX_Livros_Categoria");

        // Relacionamento com EmprestimoItens
        builder.HasMany(l => l.EmprestimoItens)
            .WithOne(ei => ei.Livro)
            .HasForeignKey(ei => ei.LivroId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
