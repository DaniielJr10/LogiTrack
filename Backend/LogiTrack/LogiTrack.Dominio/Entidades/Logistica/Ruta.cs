using LogiTrack.Dominio.Enumeraciones;

namespace LogiTrack.Dominio.Entidades.Logistica;

public class Ruta
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Nombre { get; set; } = string.Empty;

    public string Origen { get; set; } = string.Empty;

    public string Destino { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public EstadoRuta Estado { get; set; } = EstadoRuta.Pendiente;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
