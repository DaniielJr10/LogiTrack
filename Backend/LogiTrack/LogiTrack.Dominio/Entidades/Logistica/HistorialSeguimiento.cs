namespace LogiTrack.Dominio.Entidades.Logistica;

public class HistorialSeguimiento
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid EntregaId { get; set; }

    public Entrega? Entrega { get; set; }

    public decimal Latitud { get; set; }

    public decimal Longitud { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
}
