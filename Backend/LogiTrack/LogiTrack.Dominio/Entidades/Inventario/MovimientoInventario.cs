
namespace LogiTrack.Dominio.Entidades.Inventario;

/// <summary>
/// Registra las entradas, salidas y ajustes de inventario.
/// </summary>
public class MovimientoInventario
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductoId { get; set; }

    public Producto? Producto { get; set; }

    public Guid AlmacenId { get; set; }

    public Almacen? Almacen { get; set; }

    public TipoMovimiento Tipo { get; set; }

    public int Cantidad { get; set; }

    public int StockAnterior { get; set; }

    public int StockResultante { get; set; }

    public string? Observacion { get; set; }

    public DateTime FechaMovimiento { get; set; } = DateTime.UtcNow;
}