using LogiTrack.Dominio.Enumeraciones;

namespace LogiTrack.Dominio.Entidades.Logistica;

public class Entrega
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid DespachoId { get; set; }

    public Despacho? Despacho { get; set; }

    public Guid PedidoId { get; set; }

    public string? DireccionEntrega { get; set; }

    public string? CiudadEntrega { get; set; }

    public EstadoEntrega Estado { get; set; } = EstadoEntrega.Pendiente;

    public DateTime FechaProgramada { get; set; } = DateTime.UtcNow;

    public DateTime? FechaEntregaReal { get; set; }

    public decimal Latitud { get; set; }

    public decimal Longitud { get; set; }
}
