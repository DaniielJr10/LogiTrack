using LogiTrack.Dominio.Enumeraciones;

namespace LogiTrack.Aplicacion.DTOs.Inventario;

public sealed class CategoriaDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool EstaActiva { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public sealed class CrearCategoriaSolicitudDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public sealed class ActualizarCategoriaSolicitudDto
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public bool? EstaActiva { get; set; }
}

public sealed class ProductoDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public bool EstaActivo { get; set; }
    public Guid CategoriaId { get; set; }
    public string? CategoriaNombre { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}

public sealed class CrearProductoSolicitudDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string UnidadMedida { get; set; } = "Unidad";
    public decimal PrecioUnitario { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public bool EstaActivo { get; set; } = true;
    public Guid CategoriaId { get; set; }
}

public sealed class ActualizarProductoSolicitudDto
{
    public string? Codigo { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string? UnidadMedida { get; set; }
    public decimal? PrecioUnitario { get; set; }
    public int? StockActual { get; set; }
    public int? StockMinimo { get; set; }
    public bool? EstaActivo { get; set; }
    public Guid? CategoriaId { get; set; }
}

public sealed class ExistenciaInventarioDto
{
    public Guid Id { get; set; }
    public Guid ProductoId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public Guid AlmacenId { get; set; }
    public string AlmacenNombre { get; set; } = string.Empty;
    public int CantidadDisponible { get; set; }
    public int CantidadReservada { get; set; }
    public int StockDisponible { get; set; }
    public DateTime FechaActualizacion { get; set; }
}

public sealed class MovimientoInventarioDto
{
    public Guid Id { get; set; }
    public Guid ProductoId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public Guid AlmacenId { get; set; }
    public string AlmacenNombre { get; set; } = string.Empty;
    public TipoMovimiento Tipo { get; set; }
    public int Cantidad { get; set; }
    public int StockAnterior { get; set; }
    public int StockResultante { get; set; }
    public string? Observacion { get; set; }
    public DateTime FechaMovimiento { get; set; }
}

public sealed class RegistrarMovimientoInventarioSolicitudDto
{
    public Guid ProductoId { get; set; }
    public Guid AlmacenId { get; set; }
    public TipoMovimiento Tipo { get; set; }
    public int Cantidad { get; set; }
    public string? Observacion { get; set; }
}
