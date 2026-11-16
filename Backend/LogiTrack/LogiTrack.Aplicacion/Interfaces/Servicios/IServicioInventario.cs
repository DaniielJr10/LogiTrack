using LogiTrack.Aplicacion.DTOs.Inventario;

namespace LogiTrack.Aplicacion.Interfaces.Servicios;

public interface IServicioInventario
{
    Task<ResultadoAutenticacion> ObtenerCategoriasAsync(CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ObtenerCategoriaPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> CrearCategoriaAsync(CrearCategoriaSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ActualizarCategoriaAsync(Guid id, ActualizarCategoriaSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> EliminarCategoriaAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> ObtenerProductosAsync(CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ObtenerProductoPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> CrearProductoAsync(CrearProductoSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ActualizarProductoAsync(Guid id, ActualizarProductoSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> EliminarProductoAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> ObtenerExistenciasAsync(CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> RegistrarMovimientoAsync(RegistrarMovimientoInventarioSolicitudDto solicitud, CancellationToken cancellationToken = default);
}
