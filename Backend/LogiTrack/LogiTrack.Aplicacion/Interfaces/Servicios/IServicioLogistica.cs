using LogiTrack.Aplicacion.DTOs.Logistica;

namespace LogiTrack.Aplicacion.Interfaces.Servicios;

public interface IServicioLogistica
{
    Task<ResultadoAutenticacion> ObtenerVehiculosAsync(CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ObtenerVehiculoPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> CrearVehiculoAsync(CrearVehiculoSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ActualizarVehiculoAsync(Guid id, ActualizarVehiculoSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> EliminarVehiculoAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> ObtenerConductoresAsync(CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ObtenerConductorPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> CrearConductorAsync(CrearConductorSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ActualizarConductorAsync(Guid id, ActualizarConductorSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> EliminarConductorAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> ObtenerRutasAsync(CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ObtenerRutaPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> CrearRutaAsync(CrearRutaSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ActualizarRutaAsync(Guid id, ActualizarRutaSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> EliminarRutaAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> ObtenerDespachosAsync(CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ObtenerDespachoPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> CrearDespachoAsync(CrearDespachoSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ActualizarDespachoAsync(Guid id, ActualizarDespachoSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> EliminarDespachoAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> ObtenerEntregasAsync(CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ObtenerEntregaPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> CrearEntregaAsync(CrearEntregaSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ActualizarEntregaAsync(Guid id, ActualizarEntregaSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> EliminarEntregaAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> ObtenerSeguimientoEntregaAsync(Guid entregaId, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> RegistrarSeguimientoAsync(CrearHistorialSeguimientoSolicitudDto solicitud, CancellationToken cancellationToken = default);
}
