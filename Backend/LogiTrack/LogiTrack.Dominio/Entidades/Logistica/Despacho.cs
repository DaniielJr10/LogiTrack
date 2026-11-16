using LogiTrack.Dominio.Enumeraciones;

namespace LogiTrack.Dominio.Entidades.Logistica;

public class Despacho
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid RutaId { get; set; }

    public Ruta? Ruta { get; set; }

    public Guid? VehiculoId { get; set; }

    public Vehiculo? Vehiculo { get; set; }

    public Guid? ConductorId { get; set; }

    public Conductor? Conductor { get; set; }

    public EstadoDespacho Estado { get; set; } = EstadoDespacho.Programado;

    public DateTime FechaProgramada { get; set; } = DateTime.UtcNow;

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public string? Observaciones { get; set; }
}
