using LogiTrack.Aplicacion.DTOs.Pedidos;

namespace LogiTrack.Aplicacion.Interfaces.Servicios;

public interface IServicioPedidos
{
    Task<ResultadoAutenticacion> ObtenerClientesAsync(CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ObtenerClientePorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> CrearClienteAsync(CrearClienteSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ActualizarClienteAsync(Guid id, ActualizarClienteSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> EliminarClienteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> ObtenerPedidosAsync(CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ObtenerPedidoPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> CrearPedidoAsync(CrearPedidoSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> ActualizarPedidoAsync(Guid id, ActualizarPedidoSolicitudDto solicitud, CancellationToken cancellationToken = default);
    Task<ResultadoAutenticacion> EliminarPedidoAsync(Guid id, CancellationToken cancellationToken = default);
}
