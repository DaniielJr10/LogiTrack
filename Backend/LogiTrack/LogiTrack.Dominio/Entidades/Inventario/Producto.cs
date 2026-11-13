
namespace LogiTrack.Dominio.Entidades.Inventario;

/// <summary>
/// Representa un producto administrado en el inventario.
/// </summary>
public class Producto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public string UnidadMedida { get; set; } = "Unidad";

    public decimal PrecioUnitario { get; set; }

    public int StockActual { get; set; }

    public int StockMinimo { get; set; }

    public bool EstaActivo { get; set; } = true;

    public Guid CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaActualizacion { get; set; }
}
