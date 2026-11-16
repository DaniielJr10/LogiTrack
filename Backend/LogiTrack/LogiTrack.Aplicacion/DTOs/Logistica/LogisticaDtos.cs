using LogiTrack.Dominio.Enumeraciones;

namespace LogiTrack.Aplicacion.DTOs.Logistica;

public sealed class VehiculoDto
{
    public Guid Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public decimal CapacidadKg { get; set; }
    public bool EstaDisponible { get; set; }
    public DateTime FechaRegistro { get; set; }
}

public sealed class CrearVehiculoSolicitudDto
{
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public decimal CapacidadKg { get; set; }
    public bool EstaDisponible { get; set; } = true;
}

public sealed class ActualizarVehiculoSolicitudDto
{
    public string? Placa { get; set; }
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? Tipo { get; set; }
    public int? Anio { get; set; }
    public decimal? CapacidadKg { get; set; }
    public bool? EstaDisponible { get; set; }
}

public sealed class ConductorDto
{
    public Guid Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string? Licencia { get; set; }
    public string? Telefono { get; set; }
    public bool EstaDisponible { get; set; }
    public DateTime FechaRegistro { get; set; }
}

public sealed class CrearConductorSolicitudDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string? Licencia { get; set; }
    public string? Telefono { get; set; }
    public bool EstaDisponible { get; set; } = true;
}

public sealed class ActualizarConductorSolicitudDto
{
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? Licencia { get; set; }
    public string? Telefono { get; set; }
    public bool? EstaDisponible { get; set; }
}

public sealed class RutaDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Origen { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public EstadoRuta Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public sealed class CrearRutaSolicitudDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Origen { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public EstadoRuta Estado { get; set; } = EstadoRuta.Pendiente;
}

public sealed class ActualizarRutaSolicitudDto
{
    public string? Nombre { get; set; }
    public string? Origen { get; set; }
    public string? Destino { get; set; }
    public string? Descripcion { get; set; }
    public EstadoRuta? Estado { get; set; }
}

public sealed class DespachoDto
{
    public Guid Id { get; set; }
    public Guid RutaId { get; set; }
    public string? RutaNombre { get; set; }
    public Guid? VehiculoId { get; set; }
    public string? VehiculoPlaca { get; set; }
    public Guid? ConductorId { get; set; }
    public string? ConductorNombre { get; set; }
    public EstadoDespacho Estado { get; set; }
    public DateTime FechaProgramada { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Observaciones { get; set; }
}

public sealed class CrearDespachoSolicitudDto
{
    public Guid RutaId { get; set; }
    public Guid? VehiculoId { get; set; }
    public Guid? ConductorId { get; set; }
    public EstadoDespacho Estado { get; set; } = EstadoDespacho.Programado;
    public DateTime? FechaProgramada { get; set; }
    public string? Observaciones { get; set; }
}

public sealed class ActualizarDespachoSolicitudDto
{
    public Guid? RutaId { get; set; }
    public Guid? VehiculoId { get; set; }
    public Guid? ConductorId { get; set; }
    public EstadoDespacho? Estado { get; set; }
    public DateTime? FechaProgramada { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Observaciones { get; set; }
}

public sealed class EntregaDto
{
    public Guid Id { get; set; }
    public Guid DespachoId { get; set; }
    public Guid PedidoId { get; set; }
    public string? DireccionEntrega { get; set; }
    public string? CiudadEntrega { get; set; }
    public EstadoEntrega Estado { get; set; }
    public DateTime FechaProgramada { get; set; }
    public DateTime? FechaEntregaReal { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
}

public sealed class CrearEntregaSolicitudDto
{
    public Guid DespachoId { get; set; }
    public Guid PedidoId { get; set; }
    public string? DireccionEntrega { get; set; }
    public string? CiudadEntrega { get; set; }
    public EstadoEntrega Estado { get; set; } = EstadoEntrega.Pendiente;
    public DateTime? FechaProgramada { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
}

public sealed class ActualizarEntregaSolicitudDto
{
    public Guid? DespachoId { get; set; }
    public Guid? PedidoId { get; set; }
    public string? DireccionEntrega { get; set; }
    public string? CiudadEntrega { get; set; }
    public EstadoEntrega? Estado { get; set; }
    public DateTime? FechaProgramada { get; set; }
    public DateTime? FechaEntregaReal { get; set; }
    public decimal? Latitud { get; set; }
    public decimal? Longitud { get; set; }
}

public sealed class HistorialSeguimientoDto
{
    public Guid Id { get; set; }
    public Guid EntregaId { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
}

public sealed class CrearHistorialSeguimientoSolicitudDto
{
    public Guid EntregaId { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}
