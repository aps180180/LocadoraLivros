namespace LocadoraLivros.Api.Models;

/// <summary>
/// Configurações globais do sistema de empréstimos
/// </summary>
public class ConfiguracaoEmprestimo
{
    public int Id { get; set; }

    // ========== MULTAS ==========

    /// <summary>
    /// Percentual de multa por dia de atraso (ex: 0.5 = 50%)
    /// </summary>
    public decimal PercentualMultaDiaria { get; set; } = 0.5m;

    /// <summary>
    /// Valor máximo de multa (teto)
    /// </summary>
    public decimal MultaMaxima { get; set; } = 1000.00m;

    // ========== LIMITES ==========

    /// <summary>
    /// Quantidade máxima de livros por empréstimo
    /// </summary>
    public int MaximoLivrosPorEmprestimo { get; set; } = 5;

    /// <summary>
    /// Quantidade máxima de empréstimos ativos por cliente
    /// </summary>
    public int MaximoEmprestimosAtivosCliente { get; set; } = 3;

    /// <summary>
    /// Prazo máximo de empréstimo em dias
    /// </summary>
    public int DiasMaximoEmprestimo { get; set; } = 30;

    /// <summary>
    /// Prazo mínimo de empréstimo em dias
    /// </summary>
    public int DiasMinimosEmprestimo { get; set; } = 1;

    // ========== RENOVAÇÕES ==========

    /// <summary>
    /// Permite renovação de empréstimos
    /// </summary>
    public bool PermiteRenovacao { get; set; } = true;

    /// <summary>
    /// Quantidade máxima de renovações por empréstimo
    /// </summary>
    public int MaximoRenovacoes { get; set; } = 2;

    /// <summary>
    /// Dias adicionados a cada renovação
    /// </summary>
    public int DiasRenovacao { get; set; } = 7;

    // ========== RESERVAS ==========

    /// <summary>
    /// Permite reserva de livros
    /// </summary>
    public bool PermiteReserva { get; set; } = true;

    /// <summary>
    /// Dias de validade da reserva
    /// </summary>
    public int DiasValidadeReserva { get; set; } = 3;

    // ========== POLÍTICAS POR TIPO DE CLIENTE ==========

    /// <summary>
    /// Dias adicionais para clientes VIP
    /// </summary>
    public int DiasAdicionaisClienteVip { get; set; } = 7;

    /// <summary>
    /// Desconto para clientes VIP (ex: 0.1 = 10%)
    /// </summary>
    public decimal DescontoClienteVip { get; set; } = 0.1m;

    // ========== AVISOS ==========

    /// <summary>
    /// Dias antes do vencimento para enviar aviso
    /// </summary>
    public int DiasAvisoVencimento { get; set; } = 2;

    // ========== BLOQUEIOS ==========

    /// <summary>
    /// Dias de atraso para bloquear cliente
    /// </summary>
    public int DiasAtrasoParaBloqueio { get; set; } = 30;

    /// <summary>
    /// Valor de débito para bloquear cliente
    /// </summary>
    public decimal ValorDebitoParaBloqueio { get; set; } = 500.00m;

    // ========== AUDITORIA ==========

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataAtualizacao { get; set; }
    public string? UsuarioAtualizacao { get; set; }
}
