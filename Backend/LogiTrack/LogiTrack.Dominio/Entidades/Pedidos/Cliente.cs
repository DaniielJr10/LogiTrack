
namespace LogiTrack.Dominio.Entidades.Pedidos;

/// <summary>
/// Representa un cliente registrado en LogiTrack.
/// </summary>
public class Cliente
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string NombreCompleto { get; set; } = string.Empty;

    public string NumeroDocumento { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public string? CorreoElectronico { get; set; }

    public string? Direccion { get; set; }

    public string? Ciudad { get; set; }

    public bool EstaActivo { get; set; } = true;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}