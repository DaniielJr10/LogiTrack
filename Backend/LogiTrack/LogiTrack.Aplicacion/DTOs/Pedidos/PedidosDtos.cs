using LogiTrack.Dominio.Enumeraciones;

namespace LogiTrack.Aplicacion.DTOs.Pedidos;

public sealed class ClienteDto
{
    public Guid Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? CorreoElectronico { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public bool EstaActivo { get; set; }
    public DateTime FechaRegistro { get; set; }
}

public sealed class CrearClienteSolicitudDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? CorreoElectronico { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
}

public sealed class ActualizarClienteSolicitudDto
{
    public string? NombreCompleto { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? Telefono { get; set; }
    public string? CorreoElectronico { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public bool? EstaActivo { get; set; }
}

public sealed class DetallePedidoDto
{
    public Guid Id { get; set; }
    public Guid ProductoId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

public sealed class PedidoDto
{
    public Guid Id { get; set; }
    public string NumeroPedido { get; set; } = string.Empty;
    public Guid ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public string DireccionEntrega { get; set; } = string.Empty;
    public string CiudadEntrega { get; set; } = string.Empty;
    public EstadoPedido Estado { get; set; }
    public decimal Subtotal { get; set; }
    public decimal CostoEnvio { get; set; }
    public decimal Total { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaPedido { get; set; }
    public DateTime? FechaEntregaEsperada { get; set; }
    public DateTime? FechaEntregaReal { get; set; }
    public List<DetallePedidoDto> Detalles { get; set; } = [];
}

public sealed class DetallePedidoItemSolicitudDto
{
    public Guid ProductoId { get; set; }
    public int Cantidad { get; set; }
}

public sealed class CrearPedidoSolicitudDto
{
    public Guid ClienteId { get; set; }
    public string DireccionEntrega { get; set; } = string.Empty;
    public string CiudadEntrega { get; set; } = string.Empty;
    public decimal CostoEnvio { get; set; }
    public string? Observaciones { get; set; }
    public List<DetallePedidoItemSolicitudDto> Detalles { get; set; } = [];
}

public sealed class ActualizarPedidoSolicitudDto
{
    public string? DireccionEntrega { get; set; }
    public string? CiudadEntrega { get; set; }
    public EstadoPedido? Estado { get; set; }
    public decimal? CostoEnvio { get; set; }
    public string? Observaciones { get; set; }
    public DateTime? FechaEntregaEsperada { get; set; }
    public DateTime? FechaEntregaReal { get; set; }
}
