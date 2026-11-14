using LogiTrack.Dominio.Enumeraciones;

namespace LogiTrack.Dominio.Entidades.Pedidos;

/// <summary>
/// Representa un pedido realizado por un cliente.
/// </summary>
public class Pedido
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string NumeroPedido { get; set; } = string.Empty;

    public Guid ClienteId { get; set; }

    public Cliente? Cliente { get; set; }

    public string DireccionEntrega { get; set; } = string.Empty;

    public string CiudadEntrega { get; set; } = string.Empty;

    public EstadoPedido Estado { get; set; }

    public decimal Subtotal { get; set; }

    public decimal CostoEnvio { get; set; }

    public decimal Total { get; set; }

    public string? Observaciones { get; set; }

    public DateTime FechaPedido { get; set; } = DateTime.UtcNow;

    public DateTime? FechaEntregaEsperada { get; set; }

    public DateTime? FechaEntregaReal { get; set; }
}
