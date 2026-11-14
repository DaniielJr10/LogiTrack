using LogiTrack.Aplicacion;
using LogiTrack.Aplicacion.DTOs.Usuarios;
using LogiTrack.Aplicacion.Interfaces.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiTrack.API.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IServicioAutenticacion _servicioAutenticacion;

    public UsuariosController(IServicioAutenticacion servicioAutenticacion)
    {
        _servicioAutenticacion = servicioAutenticacion;
    }

    [HttpGet]
    [Authorize(Policy = "RequireAdministrador")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerUsuariosAsync(CancellationToken cancellationToken)
    {
        var resultado = await _servicioAutenticacion.ObtenerUsuariosAsync(cancellationToken);
        return resultado.Exito ? Ok(resultado) : Forbid();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerUsuarioAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioAutenticacion.ObtenerUsuarioPorIdAsync(id, cancellationToken);
        if (!resultado.Exito)
        {
            return resultado.Mensaje.Contains("permiso", StringComparison.OrdinalIgnoreCase) ? Forbid() : NotFound(resultado);
        }

        return Ok(resultado);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ActualizarUsuarioAsync(Guid id, [FromBody] ActualizarUsuarioSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioAutenticacion.ActualizarUsuarioAsync(id, solicitud, cancellationToken);
        if (!resultado.Exito)
        {
            return resultado.Mensaje.Contains("permiso", StringComparison.OrdinalIgnoreCase) ? Forbid() : BadRequest(resultado);
        }

        return Ok(resultado);
    }

    [HttpPatch("{id:guid}/estado")]
    [Authorize(Policy = "RequireAdministrador")]
    public async Task<ActionResult<ResultadoAutenticacion>> CambiarEstadoAsync(Guid id, [FromBody] EstadoUsuarioDto estado, CancellationToken cancellationToken)
    {
        var resultado = await _servicioAutenticacion.CambiarEstadoAsync(id, estado.EstaActivo, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPut("{id:guid}/rol")]
    [Authorize(Policy = "RequireAdministrador")]
    public async Task<ActionResult<ResultadoAutenticacion>> ActualizarRolAsync(Guid id, [FromBody] CambioRolDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioAutenticacion.ActualizarRolAsync(id, solicitud.Rol, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }
}

public class EstadoUsuarioDto
{
    public bool EstaActivo { get; set; }
}

public class CambioRolDto
{
    public string Rol { get; set; } = string.Empty;
}
