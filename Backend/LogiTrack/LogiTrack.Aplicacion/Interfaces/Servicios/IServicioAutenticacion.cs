using LogiTrack.Aplicacion.DTOs.Autenticacion;
using LogiTrack.Aplicacion.DTOs.Usuarios;

namespace LogiTrack.Aplicacion.Interfaces.Servicios;

public interface IServicioAutenticacion
{
    Task<ResultadoAutenticacion> RegistrarAsync(RegistroUsuarioSolicitudDto solicitud, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> IniciarSesionAsync(LoginUsuarioSolicitudDto solicitud, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> CerrarSesionAsync(CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> ObtenerPerfilAsync(CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> ActualizarPerfilAsync(ActualizarPerfilSolicitudDto solicitud, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> RecuperarContrasenaAsync(RecuperarContrasenaSolicitudDto solicitud, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> RestablecerContrasenaAsync(RestablecerContrasenaSolicitudDto solicitud, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> ObtenerUsuariosAsync(CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> ObtenerUsuarioPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> ActualizarUsuarioAsync(Guid id, ActualizarUsuarioSolicitudDto solicitud, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> CambiarEstadoAsync(Guid id, bool estaActivo, CancellationToken cancellationToken = default);

    Task<ResultadoAutenticacion> ActualizarRolAsync(Guid id, string rol, CancellationToken cancellationToken = default);
}
