
namespace LogiTrack.Dominio.Entidades.Inventario;

/// <summary>
/// Representa un almacén o centro de almacenamiento.
/// </summary>
public class Almacen
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Direccion { get; set; }

    public string? Ciudad { get; set; }

    public bool EstaActivo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}