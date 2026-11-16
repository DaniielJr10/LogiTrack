using LogiTrack.Aplicacion;
using LogiTrack.Aplicacion.DTOs.Inventario;
using LogiTrack.Aplicacion.Interfaces.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiTrack.API.Controllers;

[ApiController]
[Route("api/inventario")]
[Authorize]
public class InventarioController : ControllerBase
{
    private readonly IServicioInventario _servicioInventario;

    public InventarioController(IServicioInventario servicioInventario)
    {
        _servicioInventario = servicioInventario;
    }

    [HttpGet("categorias")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerCategoriasAsync(CancellationToken cancellationToken)
    {
        var resultado = await _servicioInventario.ObtenerCategoriasAsync(cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("categorias/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerCategoriaPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioInventario.ObtenerCategoriaPorIdAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : NotFound(resultado);
    }

    [HttpPost("categorias")]
    public async Task<ActionResult<ResultadoAutenticacion>> CrearCategoriaAsync([FromBody] CrearCategoriaSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioInventario.CrearCategoriaAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPut("categorias/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ActualizarCategoriaAsync(Guid id, [FromBody] ActualizarCategoriaSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioInventario.ActualizarCategoriaAsync(id, solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpDelete("categorias/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> EliminarCategoriaAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioInventario.EliminarCategoriaAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("productos")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerProductosAsync(CancellationToken cancellationToken)
    {
        var resultado = await _servicioInventario.ObtenerProductosAsync(cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("productos/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerProductoPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioInventario.ObtenerProductoPorIdAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : NotFound(resultado);
    }

    [HttpPost("productos")]
    public async Task<ActionResult<ResultadoAutenticacion>> CrearProductoAsync([FromBody] CrearProductoSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioInventario.CrearProductoAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPut("productos/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ActualizarProductoAsync(Guid id, [FromBody] ActualizarProductoSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioInventario.ActualizarProductoAsync(id, solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpDelete("productos/{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> EliminarProductoAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioInventario.EliminarProductoAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("existencias")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerExistenciasAsync(CancellationToken cancellationToken)
    {
        var resultado = await _servicioInventario.ObtenerExistenciasAsync(cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPost("movimientos")]
    public async Task<ActionResult<ResultadoAutenticacion>> RegistrarMovimientoAsync([FromBody] RegistrarMovimientoInventarioSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioInventario.RegistrarMovimientoAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }
}
