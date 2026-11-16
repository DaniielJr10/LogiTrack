namespace LogiTrack.Dominio.Entidades.Logistica;

public class Conductor
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string NumeroDocumento { get; set; } = string.Empty;

    public string? Licencia { get; set; }

    public string? Telefono { get; set; }

    public bool EstaDisponible { get; set; } = true;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
