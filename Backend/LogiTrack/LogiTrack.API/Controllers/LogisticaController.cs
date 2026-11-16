using LogiTrack.Aplicacion;
using LogiTrack.Aplicacion.DTOs.Logistica;
using LogiTrack.Aplicacion.Interfaces.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiTrack.API.Controllers;

[ApiController]
[Route("api/logistica")]
[Authorize]
public class LogisticaController : ControllerBase
{
    private readonly IServicioLogistica _servicioLogistica;

    public LogisticaController(IServicioLogistica servicioLogistica)
    {
        _servicioLogistica = servicioLogistica;
    }

    [HttpGet("vehiculos")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerVehiculosAsync(CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ObtenerVehiculosAsync(cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("vehiculos/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerVehiculoPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ObtenerVehiculoPorIdAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : NotFound(resultado);
    }

    [HttpPost("vehiculos")]
    public async Task<ActionResult<ResultadoAutenticacion>> CrearVehiculoAsync([FromBody] CrearVehiculoSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.CrearVehiculoAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPut("vehiculos/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ActualizarVehiculoAsync(Guid id, [FromBody] ActualizarVehiculoSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ActualizarVehiculoAsync(id, solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpDelete("vehiculos/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> EliminarVehiculoAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.EliminarVehiculoAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("conductores")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerConductoresAsync(CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ObtenerConductoresAsync(cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("conductores/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerConductorPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ObtenerConductorPorIdAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : NotFound(resultado);
    }

    [HttpPost("conductores")]
    public async Task<ActionResult<ResultadoAutenticacion>> CrearConductorAsync([FromBody] CrearConductorSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.CrearConductorAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPut("conductores/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ActualizarConductorAsync(Guid id, [FromBody] ActualizarConductorSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ActualizarConductorAsync(id, solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpDelete("conductores/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> EliminarConductorAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.EliminarConductorAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("rutas")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerRutasAsync(CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ObtenerRutasAsync(cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("rutas/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerRutaPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ObtenerRutaPorIdAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : NotFound(resultado);
    }

    [HttpPost("rutas")]
    public async Task<ActionResult<ResultadoAutenticacion>> CrearRutaAsync([FromBody] CrearRutaSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.CrearRutaAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPut("rutas/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ActualizarRutaAsync(Guid id, [FromBody] ActualizarRutaSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ActualizarRutaAsync(id, solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpDelete("rutas/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> EliminarRutaAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.EliminarRutaAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("despachos")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerDespachosAsync(CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ObtenerDespachosAsync(cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("despachos/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerDespachoPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ObtenerDespachoPorIdAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : NotFound(resultado);
    }

    [HttpPost("despachos")]
    public async Task<ActionResult<ResultadoAutenticacion>> CrearDespachoAsync([FromBody] CrearDespachoSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.CrearDespachoAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPut("despachos/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ActualizarDespachoAsync(Guid id, [FromBody] ActualizarDespachoSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ActualizarDespachoAsync(id, solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpDelete("despachos/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> EliminarDespachoAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.EliminarDespachoAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("entregas")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerEntregasAsync(CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ObtenerEntregasAsync(cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("entregas/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerEntregaPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ObtenerEntregaPorIdAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : NotFound(resultado);
    }

    [HttpPost("entregas")]
    public async Task<ActionResult<ResultadoAutenticacion>> CrearEntregaAsync([FromBody] CrearEntregaSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.CrearEntregaAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPut("entregas/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ActualizarEntregaAsync(Guid id, [FromBody] ActualizarEntregaSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ActualizarEntregaAsync(id, solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpDelete("entregas/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> EliminarEntregaAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.EliminarEntregaAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("entregas/{entregaId:guid}/seguimiento")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerSeguimientoEntregaAsync(Guid entregaId, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.ObtenerSeguimientoEntregaAsync(entregaId, cancellationToken);
        return resultado.Exito ? Ok(resultado) : NotFound(resultado);
    }

    [HttpPost("seguimiento")]
    public async Task<ActionResult<ResultadoAutenticacion>> RegistrarSeguimientoAsync([FromBody] CrearHistorialSeguimientoSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioLogistica.RegistrarSeguimientoAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }
}
