namespace LogiTrack.Dominio.Entidades.Inventario;

/// <summary>
/// Representa una categoría de productos del inventario.
/// </summary>
public class Categoria
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool EstaActiva { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}