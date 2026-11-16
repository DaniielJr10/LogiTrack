using LogiTrack.Aplicacion;
using LogiTrack.Aplicacion.DTOs.Pedidos;
using LogiTrack.Aplicacion.Interfaces.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiTrack.API.Controllers;

[ApiController]
[Route("api/pedidos")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly IServicioPedidos _servicioPedidos;

    public PedidosController(IServicioPedidos servicioPedidos)
    {
        _servicioPedidos = servicioPedidos;
    }

    [HttpGet]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerPedidosAsync(CancellationToken cancellationToken)
    {
        var resultado = await _servicioPedidos.ObtenerPedidosAsync(cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ObtenerPedidoPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioPedidos.ObtenerPedidoPorIdAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : NotFound(resultado);
    }

    [HttpPost]
    public async Task<ActionResult<ResultadoAutenticacion>> CrearPedidoAsync([FromBody] CrearPedidoSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioPedidos.CrearPedidoAsync(solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> ActualizarPedidoAsync(Guid id, [FromBody] ActualizarPedidoSolicitudDto solicitud, CancellationToken cancellationToken)
    {
        var resultado = await _servicioPedidos.ActualizarPedidoAsync(id, solicitud, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ResultadoAutenticacion>> EliminarPedidoAsync(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioPedidos.EliminarPedidoAsync(id, cancellationToken);
        return resultado.Exito ? Ok(resultado) : BadRequest(resultado);
    }
}
