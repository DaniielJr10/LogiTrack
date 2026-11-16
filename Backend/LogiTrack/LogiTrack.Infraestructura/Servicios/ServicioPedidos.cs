using LogiTrack.Aplicacion;
using LogiTrack.Aplicacion.DTOs.Pedidos;
using LogiTrack.Aplicacion.Interfaces.Seguridad;
using LogiTrack.Aplicacion.Interfaces.Servicios;
using LogiTrack.Dominio.Entidades.Inventario;
using LogiTrack.Dominio.Entidades.Pedidos;
using LogiTrack.Dominio.Enumeraciones;
using LogiTrack.Infraestructura.BaseDatos.Contexto;
using LogiTrack.Infraestructura.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Infraestructura.Servicios;

public class ServicioPedidos : IServicioPedidos
{
    private readonly LogiTrackDbContext _context;
    private readonly IUsuarioActualServicio _usuarioActualServicio;

    public ServicioPedidos(LogiTrackDbContext context, IUsuarioActualServicio usuarioActualServicio)
    {
        _context = context;
        _usuarioActualServicio = usuarioActualServicio;
    }

    public async Task<ResultadoAutenticacion> ObtenerClientesAsync(CancellationToken cancellationToken = default)
    {
        var clientes = await _context.Clientes
            .AsNoTracking()
            .OrderBy(c => c.NombreCompleto)
            .Select(c => new ClienteDto
            {
                Id = c.Id,
                NombreCompleto = c.NombreCompleto,
                NumeroDocumento = c.NumeroDocumento,
                Telefono = c.Telefono,
                CorreoElectronico = c.CorreoElectronico,
                Direccion = c.Direccion,
                Ciudad = c.Ciudad,
                EstaActivo = c.EstaActivo,
                FechaRegistro = c.FechaRegistro
            })
            .ToListAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Clientes obtenidos correctamente.", clientes);
    }

    public async Task<ResultadoAutenticacion> ObtenerClientePorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cliente = await _context.Clientes
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (cliente is null)
        {
            return ResultadoAutenticacion.Fallido("Cliente no encontrado.");
        }

        return ResultadoAutenticacion.Correcto("Cliente encontrado.", MapearCliente(cliente));
    }

    public async Task<ResultadoAutenticacion> CrearClienteAsync(CrearClienteSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarPedidos())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para crear clientes.");
        }

        if (string.IsNullOrWhiteSpace(solicitud.NombreCompleto) || string.IsNullOrWhiteSpace(solicitud.NumeroDocumento))
        {
            return ResultadoAutenticacion.Fallido("Nombre completo y documento del cliente son obligatorios.");
        }

        var existeCliente = await _context.Clientes.AnyAsync(c => c.NumeroDocumento == solicitud.NumeroDocumento.Trim(), cancellationToken);
        if (existeCliente)
        {
            return ResultadoAutenticacion.Fallido("Ya existe un cliente con ese número de documento.");
        }

        var cliente = new Cliente
        {
            NombreCompleto = solicitud.NombreCompleto.Trim(),
            NumeroDocumento = solicitud.NumeroDocumento.Trim(),
            Telefono = string.IsNullOrWhiteSpace(solicitud.Telefono) ? null : solicitud.Telefono.Trim(),
            CorreoElectronico = string.IsNullOrWhiteSpace(solicitud.CorreoElectronico) ? null : solicitud.CorreoElectronico.Trim(),
            Direccion = string.IsNullOrWhiteSpace(solicitud.Direccion) ? null : solicitud.Direccion.Trim(),
            Ciudad = string.IsNullOrWhiteSpace(solicitud.Ciudad) ? null : solicitud.Ciudad.Trim(),
            EstaActivo = true,
            FechaRegistro = DateTime.UtcNow
        };

        await _context.Clientes.AddAsync(cliente, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Cliente creado correctamente.", MapearCliente(cliente));
    }

    public async Task<ResultadoAutenticacion> ActualizarClienteAsync(Guid id, ActualizarClienteSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarPedidos())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para actualizar clientes.");
        }

        var cliente = await _context.Clientes.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (cliente is null)
        {
            return ResultadoAutenticacion.Fallido("Cliente no encontrado.");
        }

        if (!string.IsNullOrWhiteSpace(solicitud.NombreCompleto))
        {
            cliente.NombreCompleto = solicitud.NombreCompleto.Trim();
        }

        if (!string.IsNullOrWhiteSpace(solicitud.NumeroDocumento))
        {
            var documentoDuplicado = await _context.Clientes.AnyAsync(c => c.Id != id && c.NumeroDocumento == solicitud.NumeroDocumento.Trim(), cancellationToken);
            if (documentoDuplicado)
            {
                return ResultadoAutenticacion.Fallido("Ya existe otro cliente con ese número de documento.");
            }

            cliente.NumeroDocumento = solicitud.NumeroDocumento.Trim();
        }

        if (solicitud.Telefono is not null)
        {
            cliente.Telefono = string.IsNullOrWhiteSpace(solicitud.Telefono) ? null : solicitud.Telefono.Trim();
        }

        if (solicitud.CorreoElectronico is not null)
        {
            cliente.CorreoElectronico = string.IsNullOrWhiteSpace(solicitud.CorreoElectronico) ? null : solicitud.CorreoElectronico.Trim();
        }

        if (solicitud.Direccion is not null)
        {
            cliente.Direccion = string.IsNullOrWhiteSpace(solicitud.Direccion) ? null : solicitud.Direccion.Trim();
        }

        if (solicitud.Ciudad is not null)
        {
            cliente.Ciudad = string.IsNullOrWhiteSpace(solicitud.Ciudad) ? null : solicitud.Ciudad.Trim();
        }

        if (solicitud.EstaActivo.HasValue)
        {
            cliente.EstaActivo = solicitud.EstaActivo.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ResultadoAutenticacion.Correcto("Cliente actualizado correctamente.", MapearCliente(cliente));
    }

    public async Task<ResultadoAutenticacion> EliminarClienteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarPedidos())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para eliminar clientes.");
        }

        var cliente = await _context.Clientes.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (cliente is null)
        {
            return ResultadoAutenticacion.Fallido("Cliente no encontrado.");
        }

        var tienePedidos = await _context.Pedidos.AnyAsync(p => p.ClienteId == id, cancellationToken);
        if (tienePedidos)
        {
            return ResultadoAutenticacion.Fallido("No se puede eliminar un cliente con pedidos asociados.");
        }

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Cliente eliminado correctamente.");
    }

    public async Task<ResultadoAutenticacion> ObtenerPedidosAsync(CancellationToken cancellationToken = default)
    {
        var pedidos = await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Cliente)
            .OrderByDescending(p => p.FechaPedido)
            .ToListAsync(cancellationToken);

        var pedidoIds = pedidos.Select(p => p.Id).ToList();
        var detalles = await _context.DetallesPedido
            .AsNoTracking()
            .Include(d => d.Producto)
            .Where(d => pedidoIds.Contains(d.PedidoId))
            .ToListAsync(cancellationToken);

        var resultado = pedidos
            .Select(p => MapearPedido(p, detalles.Where(d => d.PedidoId == p.Id)))
            .ToList();

        return ResultadoAutenticacion.Correcto("Pedidos obtenidos correctamente.", resultado);
    }

    public async Task<ResultadoAutenticacion> ObtenerPedidoPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pedido = await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Cliente)
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (pedido is null)
        {
            return ResultadoAutenticacion.Fallido("Pedido no encontrado.");
        }

        var detalles = await _context.DetallesPedido
            .AsNoTracking()
            .Include(d => d.Producto)
            .Where(d => d.PedidoId == id)
            .ToListAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Pedido encontrado.", MapearPedido(pedido, detalles));
    }

    public async Task<ResultadoAutenticacion> CrearPedidoAsync(CrearPedidoSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarPedidos())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para crear pedidos.");
        }

        if (solicitud.ClienteId == Guid.Empty)
        {
            return ResultadoAutenticacion.Fallido("Debe indicar un cliente válido.");
        }

        if (string.IsNullOrWhiteSpace(solicitud.DireccionEntrega) || string.IsNullOrWhiteSpace(solicitud.CiudadEntrega))
        {
            return ResultadoAutenticacion.Fallido("Dirección y ciudad de entrega son obligatorias.");
        }

        if (solicitud.Detalles is null || solicitud.Detalles.Count == 0)
        {
            return ResultadoAutenticacion.Fallido("Debe incluir al menos un producto en el pedido.");
        }

        var cliente = await _context.Clientes.SingleOrDefaultAsync(c => c.Id == solicitud.ClienteId, cancellationToken);
        if (cliente is null)
        {
            return ResultadoAutenticacion.Fallido("El cliente indicado no existe.");
        }

        var idsProductos = solicitud.Detalles.Select(d => d.ProductoId).Distinct().ToList();
        var productos = await _context.Productos
            .Where(p => idsProductos.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (productos.Count != idsProductos.Count)
        {
            return ResultadoAutenticacion.Fallido("Uno o varios productos indicados no existen.");
        }

        decimal subtotal = 0m;
        var detalles = new List<DetallePedido>();

        foreach (var item in solicitud.Detalles)
        {
            if (item.Cantidad <= 0)
            {
                return ResultadoAutenticacion.Fallido("La cantidad de cada producto debe ser mayor que cero.");
            }

            var producto = productos.Single(p => p.Id == item.ProductoId);
            if (!producto.EstaActivo)
            {
                return ResultadoAutenticacion.Fallido($"El producto {producto.Nombre} está inactivo.");
            }

            if (producto.StockActual < item.Cantidad)
            {
                return ResultadoAutenticacion.Fallido($"No hay stock suficiente para el producto {producto.Nombre}.");
            }

            var precio = producto.PrecioUnitario;
            subtotal += item.Cantidad * precio;

            producto.StockActual -= item.Cantidad;
            producto.FechaActualizacion = DateTime.UtcNow;

            detalles.Add(new DetallePedido
            {
                ProductoId = producto.Id,
                Producto = producto,
                Cantidad = item.Cantidad,
                PrecioUnitario = precio
            });
        }

        var pedido = new Pedido
        {
            NumeroPedido = $"PED-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..4].ToUpperInvariant()}",
            ClienteId = cliente.Id,
            DireccionEntrega = solicitud.DireccionEntrega.Trim(),
            CiudadEntrega = solicitud.CiudadEntrega.Trim(),
            Estado = EstadoPedido.Pendiente,
            Subtotal = subtotal,
            CostoEnvio = solicitud.CostoEnvio,
            Total = subtotal + solicitud.CostoEnvio,
            Observaciones = string.IsNullOrWhiteSpace(solicitud.Observaciones) ? null : solicitud.Observaciones.Trim(),
            FechaPedido = DateTime.UtcNow,
            FechaEntregaEsperada = DateTime.UtcNow.AddDays(2)
        };

        await _context.Pedidos.AddAsync(pedido, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var detalle in detalles)
        {
            detalle.PedidoId = pedido.Id;
        }

        await _context.DetallesPedido.AddRangeAsync(detalles, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var pedidoGuardado = await _context.Pedidos
            .Include(p => p.Cliente)
            .SingleAsync(p => p.Id == pedido.Id, cancellationToken);

        var detalleGuardado = await _context.DetallesPedido
            .Include(d => d.Producto)
            .Where(d => d.PedidoId == pedido.Id)
            .ToListAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Pedido creado correctamente.", MapearPedido(pedidoGuardado, detalleGuardado));
    }

    public async Task<ResultadoAutenticacion> ActualizarPedidoAsync(Guid id, ActualizarPedidoSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarPedidos())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para actualizar pedidos.");
        }

        var pedido = await _context.Pedidos.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (pedido is null)
        {
            return ResultadoAutenticacion.Fallido("Pedido no encontrado.");
        }

        if (!string.IsNullOrWhiteSpace(solicitud.DireccionEntrega))
        {
            pedido.DireccionEntrega = solicitud.DireccionEntrega.Trim();
        }

        if (!string.IsNullOrWhiteSpace(solicitud.CiudadEntrega))
        {
            pedido.CiudadEntrega = solicitud.CiudadEntrega.Trim();
        }

        if (solicitud.CostoEnvio.HasValue)
        {
            pedido.CostoEnvio = solicitud.CostoEnvio.Value;
            pedido.Total = pedido.Subtotal + pedido.CostoEnvio;
        }

        if (solicitud.Estado.HasValue)
        {
            pedido.Estado = solicitud.Estado.Value;
            if (solicitud.Estado.Value == EstadoPedido.Entregado && pedido.FechaEntregaReal is null)
            {
                pedido.FechaEntregaReal = DateTime.UtcNow;
            }
        }

        if (solicitud.Observaciones is not null)
        {
            pedido.Observaciones = string.IsNullOrWhiteSpace(solicitud.Observaciones) ? null : solicitud.Observaciones.Trim();
        }

        if (solicitud.FechaEntregaEsperada.HasValue)
        {
            pedido.FechaEntregaEsperada = solicitud.FechaEntregaEsperada.Value;
        }

        if (solicitud.FechaEntregaReal.HasValue)
        {
            pedido.FechaEntregaReal = solicitud.FechaEntregaReal.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var pedidoActualizado = await _context.Pedidos
            .Include(p => p.Cliente)
            .SingleAsync(p => p.Id == pedido.Id, cancellationToken);

        var detallesActualizados = await _context.DetallesPedido
            .Include(d => d.Producto)
            .Where(d => d.PedidoId == pedido.Id)
            .ToListAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Pedido actualizado correctamente.", MapearPedido(pedidoActualizado, detallesActualizados));
    }

    public async Task<ResultadoAutenticacion> EliminarPedidoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarPedidos())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para eliminar pedidos.");
        }

        var pedido = await _context.Pedidos.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (pedido is null)
        {
            return ResultadoAutenticacion.Fallido("Pedido no encontrado.");
        }

        var detalles = await _context.DetallesPedido.Where(d => d.PedidoId == id).ToListAsync(cancellationToken);
        if (detalles.Count > 0)
        {
            _context.DetallesPedido.RemoveRange(detalles);
        }

        _context.Pedidos.Remove(pedido);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Pedido eliminado correctamente.");
    }

    private bool PuedeGestionarPedidos()
    {
        return _usuarioActualServicio.TieneRol(RolesSistema.Administrador)
            || _usuarioActualServicio.TieneRol(RolesSistema.CoordinadorLogistico)
            || _usuarioActualServicio.TieneRol(RolesSistema.AuxiliarBodega);
    }

    private static ClienteDto MapearCliente(Cliente cliente) => new()
    {
        Id = cliente.Id,
        NombreCompleto = cliente.NombreCompleto,
        NumeroDocumento = cliente.NumeroDocumento,
        Telefono = cliente.Telefono,
        CorreoElectronico = cliente.CorreoElectronico,
        Direccion = cliente.Direccion,
        Ciudad = cliente.Ciudad,
        EstaActivo = cliente.EstaActivo,
        FechaRegistro = cliente.FechaRegistro
    };

    private static PedidoDto MapearPedido(Pedido pedido, IEnumerable<DetallePedido>? detalles = null)
    {
        var detalleDtos = (detalles ?? Enumerable.Empty<DetallePedido>())
            .Select(d => new DetallePedidoDto
            {
                Id = d.Id,
                ProductoId = d.ProductoId,
                ProductoNombre = d.Producto?.Nombre ?? string.Empty,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = d.Subtotal
            })
            .ToList();

        return new PedidoDto
        {
            Id = pedido.Id,
            NumeroPedido = pedido.NumeroPedido,
            ClienteId = pedido.ClienteId,
            ClienteNombre = pedido.Cliente?.NombreCompleto ?? string.Empty,
            DireccionEntrega = pedido.DireccionEntrega,
            CiudadEntrega = pedido.CiudadEntrega,
            Estado = pedido.Estado,
            Subtotal = pedido.Subtotal,
            CostoEnvio = pedido.CostoEnvio,
            Total = pedido.Total,
            Observaciones = pedido.Observaciones,
            FechaPedido = pedido.FechaPedido,
            FechaEntregaEsperada = pedido.FechaEntregaEsperada,
            FechaEntregaReal = pedido.FechaEntregaReal,
            Detalles = detalleDtos
        };
    }
}
