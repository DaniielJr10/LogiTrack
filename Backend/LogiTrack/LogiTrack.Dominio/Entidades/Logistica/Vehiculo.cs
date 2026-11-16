namespace LogiTrack.Dominio.Entidades.Logistica;

public class Vehiculo
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Placa { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;

    public string Modelo { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public int Anio { get; set; }

    public decimal CapacidadKg { get; set; }

    public bool EstaDisponible { get; set; } = true;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
