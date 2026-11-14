using LogiTrack.Aplicacion;
using LogiTrack.Aplicacion.DTOs.Autenticacion;
using LogiTrack.Aplicacion.Interfaces.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiTrack.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IServicioAutenticacion _servicioAutenticacion;

    public AuthController(IServicioAutenticacion servicioAutenticacion)
    {
        _servicioAutenticacion = servicioAutenticacion;
    }

    [HttpPost("registro")]
    [AllowAnonymous]
    public async Task<ActionResult<ResultadoAutenticacion>> RegistrarAsync([FromBody] RegistroUsuarioSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioAutenticacion.RegistrarAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ResultadoAutenticacion>> LoginAsync([FromBody] LoginUsuarioSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioAutenticacion.IniciarSesionAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : Unauthorized(resultado);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ResultadoAutenticacion>> LogoutAsync(CancellationToken cancellationToken)
    {
        var resultado = await _servicioAutenticacion.CerrarSesionAsync(cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("perfil")]
    [Authorize]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerPerfilAsync(CancellationToken cancellationToken)
    {
        var resultado = await _servicioAutenticacion.ObtenerPerfilAsync(cancellationToken);
        return resultado.Exito ? Ok(resultado) : NotFound(resultado);
    }

    [HttpPut("perfil")]
    [Authorize]
    public async Task<ActionResult<ResultadoAutenticacion>> ActualizarPerfilAsync([FromBody] ActualizarPerfilSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioAutenticacion.ActualizarPerfilAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPost("recuperar-contrasena")]
    [AllowAnonymous]
    public async Task<ActionResult<ResultadoAutenticacion>> RecuperarContrasenaAsync([FromBody] RecuperarContrasenaSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioAutenticacion.RecuperarContrasenaAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPost("restablecer-contrasena")]
    [AllowAnonymous]
    public async Task<ActionResult<ResultadoAutenticacion>> RestablecerContrasenaAsync([FromBody] RestablecerContrasenaSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioAutenticacion.RestablecerContrasenaAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }
}
