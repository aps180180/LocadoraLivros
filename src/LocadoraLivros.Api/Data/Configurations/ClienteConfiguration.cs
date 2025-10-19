// Data/Configurations/ClienteConfiguration.cs
using LocadoraLivros.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraLivros.Api.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.CPF)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Telefone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Celular)
            .HasMaxLength(20);

        builder.Property(c => c.Endereco)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(c => c.Cidade)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Estado)
            .IsRequired()
            .HasMaxLength(2)
            .IsFixedLength();

        builder.Property(c => c.CEP)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(c => c.DataCadastro)
            .IsRequired();

        builder.Property(c => c.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        // Índice único no CPF
        builder.HasIndex(c => c.CPF)
            .IsUnique()
            .HasDatabaseName("IX_Clientes_CPF");

        // Índice no Email
        builder.HasIndex(c => c.Email)
            .HasDatabaseName("IX_Clientes_Email");

        // Relacionamento com Emprestimos
        builder.HasMany(c => c.Emprestimos)
            .WithOne(e => e.Cliente)
            .HasForeignKey(e => e.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
