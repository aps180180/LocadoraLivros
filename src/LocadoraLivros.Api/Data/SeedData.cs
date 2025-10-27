using LocadoraLivros.Api.Models;
using LocadoraLivros.Api.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LocadoraLivros.Api.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Aplicar migrations pendentes
        await context.Database.MigrateAsync();

        // Seed apenas se banco estiver vazio
        if (context.Livros.Any())
        {
            Console.WriteLine("ℹ️  Banco já possui dados. Seed ignorado.");
            return;
        }

        Console.WriteLine("🌱 Iniciando seed de dados...");

        // === LIVROS ===
        var livros = new List<Livro>
        {
            new Livro
            {
                Titulo = "O Senhor dos Anéis: A Sociedade do Anel",
                ISBN = "978-85-336-2767-8",
                Autor = "J.R.R. Tolkien",
                Editora = "Martins Fontes",
                AnoPublicacao = 2019,
                Categoria = "Fantasia",
                QuantidadeDisponivel = 3,
                QuantidadeTotal = 5,
                ValorDiaria = 2.50m,
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            },
            new Livro
            {
                Titulo = "Harry Potter e a Pedra Filosofal",
                ISBN = "978-85-325-1101-9",
                Autor = "J.K. Rowling",
                Editora = "Rocco",
                AnoPublicacao = 2017,
                Categoria = "Fantasia",
                QuantidadeDisponivel = 4,
                QuantidadeTotal = 6,
                ValorDiaria = 2.00m,
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            },
            new Livro
            {
                Titulo = "1984",
                ISBN = "978-85-359-0277-8",
                Autor = "George Orwell",
                Editora = "Companhia das Letras",
                AnoPublicacao = 2009,
                Categoria = "Ficção Científica",
                QuantidadeDisponivel = 2,
                QuantidadeTotal = 3,
                ValorDiaria = 3.00m,
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            },
            new Livro
            {
                Titulo = "O Hobbit",
                ISBN = "978-85-336-2772-2",
                Autor = "J.R.R. Tolkien",
                Editora = "Martins Fontes",
                AnoPublicacao = 2019,
                Categoria = "Fantasia",
                QuantidadeDisponivel = 5,
                QuantidadeTotal = 5,
                ValorDiaria = 2.50m,
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            },
            new Livro
            {
                Titulo = "Clean Code",
                ISBN = "978-0-13-235088-4",
                Autor = "Robert C. Martin",
                Editora = "Prentice Hall",
                AnoPublicacao = 2008,
                Categoria = "Tecnologia",
                QuantidadeDisponivel = 2,
                QuantidadeTotal = 4,
                ValorDiaria = 4.00m,
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            },
            new Livro
            {
                Titulo = "Domain-Driven Design",
                ISBN = "978-0-321-12521-7",
                Autor = "Eric Evans",
                Editora = "Addison-Wesley",
                AnoPublicacao = 2003,
                Categoria = "Tecnologia",
                QuantidadeDisponivel = 1,
                QuantidadeTotal = 2,
                ValorDiaria = 5.00m,
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            },
            new Livro
            {
                Titulo = "A Revolução dos Bichos",
                ISBN = "978-85-359-0675-2",
                Autor = "George Orwell",
                Editora = "Companhia das Letras",
                AnoPublicacao = 2007,
                Categoria = "Ficção",
                QuantidadeDisponivel = 3,
                QuantidadeTotal = 3,
                ValorDiaria = 2.50m,
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            },
            new Livro
            {
                Titulo = "O Código Da Vinci",
                ISBN = "978-85-7542-145-9",
                Autor = "Dan Brown",
                Editora = "Sextante",
                AnoPublicacao = 2004,
                Categoria = "Suspense",
                QuantidadeDisponivel = 2,
                QuantidadeTotal = 4,
                ValorDiaria = 3.00m,
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            }
        };

        context.Livros.AddRange(livros);
        await context.SaveChangesAsync();
        Console.WriteLine($"✅ {livros.Count} livros criados");

        // === CLIENTES ===
        var clientes = new List<Cliente>
        {
            new Cliente
            {
                Nome = "Maria Santos Silva",
                CPF = "12345678901",
                Email = "maria.santos@email.com",
                Telefone = "(11) 3456-7890",
                Celular = "(11) 98765-4321",
                Endereco = "Rua das Flores, 123, Apto 45",
                Cidade = "São Paulo",
                Estado = "SP",
                CEP = "01234-567",
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            },
            new Cliente
            {
                Nome = "João Silva Santos",
                CPF = "98765432100",
                Email = "joao.silva@email.com",
                Telefone = "(21) 2345-6789",
                Celular = "(21) 99876-5432",
                Endereco = "Avenida Brasil, 456",
                Cidade = "Rio de Janeiro",
                Estado = "RJ",
                CEP = "98765-432",
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            },
            new Cliente
            {
                Nome = "Ana Paula Oliveira",
                CPF = "11122233344",
                Email = "ana.oliveira@email.com",
                Telefone = "(31) 3333-4444",
                Celular = "(31) 98888-7777",
                Endereco = "Rua Minas Gerais, 789",
                Cidade = "Belo Horizonte",
                Estado = "MG",
                CEP = "30123-456",
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            },
            new Cliente
            {
                Nome = "Carlos Eduardo Costa",
                CPF = "55566677788",
                Email = "carlos.costa@email.com",
                Telefone = "(41) 3222-1111",
                Celular = "(41) 99999-8888",
                Endereco = "Avenida das Araucárias, 321",
                Cidade = "Curitiba",
                Estado = "PR",
                CEP = "80000-000",
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            },
            new Cliente
            {
                Nome = "Fernanda Lima Souza",
                CPF = "99988877766",
                Email = "fernanda.lima@email.com",
                Telefone = "(85) 3111-2222",
                Celular = "(85) 98777-6666",
                Endereco = "Rua do Sol, 555",
                Cidade = "Fortaleza",
                Estado = "CE",
                CEP = "60000-000",
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            }
        };

        context.Clientes.AddRange(clientes);
        await context.SaveChangesAsync();
        Console.WriteLine($"✅ {clientes.Count} clientes criados");

        // === EMPRÉSTIMOS DE EXEMPLO ===
        // Empréstimo 1 - Ativo (ainda não devolvido)
        var emprestimo1 = new Emprestimo
        {
            ClienteId = clientes[0].Id,
            DataEmprestimo = DateTime.UtcNow.AddDays(-5),
            DataPrevisaoDevolucao = DateTime.UtcNow.AddDays(9), // 14 dias no total
            Status = EmprestimoStatus.Ativo,
            Observacoes = "Primeiro empréstimo da cliente",
            ValorTotal = 0
        };

        var item1_1 = new EmprestimoItem
        {
            LivroId = livros[0].Id, // Senhor dos Anéis
            DiasEmprestimo = 14,
            ValorDiaria = livros[0].ValorDiaria,
            ValorSubtotal = livros[0].ValorDiaria * 14
        };

        var item1_2 = new EmprestimoItem
        {
            LivroId = livros[1].Id, // Harry Potter
            DiasEmprestimo = 14,
            ValorDiaria = livros[1].ValorDiaria,
            ValorSubtotal = livros[1].ValorDiaria * 14
        };

        emprestimo1.Itens.Add(item1_1);
        emprestimo1.Itens.Add(item1_2);
        emprestimo1.ValorTotal = item1_1.ValorSubtotal + item1_2.ValorSubtotal;

        // Decrementar estoque
        livros[0].QuantidadeDisponivel--;
        livros[1].QuantidadeDisponivel--;

        context.Emprestimos.Add(emprestimo1);
        await context.SaveChangesAsync();

        // Empréstimo 2 - Ativo (próximo de vencer)
        var emprestimo2 = new Emprestimo
        {
            ClienteId = clientes[1].Id,
            DataEmprestimo = DateTime.UtcNow.AddDays(-6),
            DataPrevisaoDevolucao = DateTime.UtcNow.AddDays(1), // Vence amanhã
            Status = EmprestimoStatus.Ativo,
            ValorTotal = 0
        };

        var item2_1 = new EmprestimoItem
        {
            LivroId = livros[2].Id, // 1984
            DiasEmprestimo = 7,
            ValorDiaria = livros[2].ValorDiaria,
            ValorSubtotal = livros[2].ValorDiaria * 7
        };

        emprestimo2.Itens.Add(item2_1);
        emprestimo2.ValorTotal = item2_1.ValorSubtotal;

        livros[2].QuantidadeDisponivel--;

        context.Emprestimos.Add(emprestimo2);
        await context.SaveChangesAsync();

        // Empréstimo 3 - Devolvido sem atraso
        var emprestimo3 = new Emprestimo
        {
            ClienteId = clientes[2].Id,
            DataEmprestimo = DateTime.UtcNow.AddDays(-20),
            DataPrevisaoDevolucao = DateTime.UtcNow.AddDays(-6),
            DataDevolucao = DateTime.UtcNow.AddDays(-7), // Devolveu 1 dia antes
            Status = EmprestimoStatus.Devolvido,
            ValorMulta = 0,
            ValorTotal = 0
        };

        var item3_1 = new EmprestimoItem
        {
            LivroId = livros[3].Id, // O Hobbit
            DiasEmprestimo = 14,
            ValorDiaria = livros[3].ValorDiaria,
            ValorSubtotal = livros[3].ValorDiaria * 14,
            DataDevolucaoItem = DateTime.UtcNow.AddDays(-7)
        };

        emprestimo3.Itens.Add(item3_1);
        emprestimo3.ValorTotal = item3_1.ValorSubtotal;

        context.Emprestimos.Add(emprestimo3);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ 3 empréstimos de exemplo criados");

        // === USUÁRIOS DE TESTE ===
        // User comum
        var userComum = await userManager.FindByEmailAsync("user@locadora.com");
        if (userComum == null)
        {
            userComum = new ApplicationUser
            {
                UserName = "user",
                Email = "user@locadora.com",
                NomeCompleto = "Usuário Comum",
                DataCadastro = DateTime.UtcNow,
                Ativo = true,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(userComum, "User@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(userComum, "User");
                Console.WriteLine("✅ Usuário 'user@locadora.com' criado (Role: User)");
            }
        }

        // Manager
        var manager = await userManager.FindByEmailAsync("manager@locadora.com");
        if (manager == null)
        {
            manager = new ApplicationUser
            {
                UserName = "manager",
                Email = "manager@locadora.com",
                NomeCompleto = "Gerente da Locadora",
                DataCadastro = DateTime.UtcNow,
                Ativo = true,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(manager, "Manager@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(manager, "User");
                await userManager.AddToRoleAsync(manager, "Manager");
                Console.WriteLine("✅ Usuário 'manager@locadora.com' criado (Roles: User, Manager)");
            }
        }

        Console.WriteLine("\n🎉 Seed de dados concluído com sucesso!");
        Console.WriteLine("\n📋 Credenciais de acesso:");
        Console.WriteLine("   Admin:   admin@locadora.com / Admin@123");
        Console.WriteLine("   Manager: manager@locadora.com / Manager@123");
        Console.WriteLine("   User:    user@locadora.com / User@123\n");
    }
}
