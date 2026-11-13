
namespace LogiTrack.Dominio.Entidades.Inventario;

/// <summary>
/// Representa la cantidad disponible de un producto en un almacén.
/// </summary>
public class ExistenciaInventario
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductoId { get; set; }

    public Producto? Producto { get; set; }

    public Guid AlmacenId { get; set; }

    public Almacen? Almacen { get; set; }

    public int CantidadDisponible { get; set; }

    public int CantidadReservada { get; set; }

    public int StockDisponible => CantidadDisponible - CantidadReservada;

    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
}