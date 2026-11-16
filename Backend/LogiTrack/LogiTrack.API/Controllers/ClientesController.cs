using LogiTrack.Aplicacion;
using LogiTrack.Aplicacion.DTOs.Pedidos;
using LogiTrack.Aplicacion.Interfaces.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiTrack.API.Controllers;

[ApiController]
[Route("api/clientes")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IServicioPedidos _servicioPedidos;

    public ClientesController(IServicioPedidos servicioPedidos)
    {
        _servicioPedidos = servicioPedidos;
    }

    [HttpGet]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerClientesAsync(CancellationToken cancellationToken)
    {
        var resultado = await _servicioPedidos.ObtenerClientesAsync(cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerClientePorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioPedidos.ObtenerClientePorIdAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : NotFound(resultado);
    }

    [HttpPost]
    public async Task<ActionResult<ResultadoAutenticacion>> CrearClienteAsync([FromBody] CrearClienteSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioPedidos.CrearClienteAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ActualizarClienteAsync(Guid id, [FromBody] ActualizarClienteSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioPedidos.ActualizarClienteAsync(id, solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> EliminarClienteAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioPedidos.EliminarClienteAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }
}
