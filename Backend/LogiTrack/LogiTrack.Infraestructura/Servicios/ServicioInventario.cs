using LogiTrack.Aplicacion;
using LogiTrack.Aplicacion.DTOs.Inventario;
using LogiTrack.Aplicacion.Interfaces.Seguridad;
using LogiTrack.Aplicacion.Interfaces.Servicios;
using LogiTrack.Dominio.Entidades.Inventario;
using LogiTrack.Dominio.Enumeraciones;
using LogiTrack.Infraestructura.BaseDatos.Contexto;
using LogiTrack.Infraestructura.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Infraestructura.Servicios;

public class ServicioInventario : IServicioInventario
{
    private readonly LogiTrackDbContext _context;
    private readonly IUsuarioActualServicio _usuarioActualServicio;

    public ServicioInventario(LogiTrackDbContext context, IUsuarioActualServicio usuarioActualServicio)
    {
        _context = context;
        _usuarioActualServicio = usuarioActualServicio;
    }

    public async Task<ResultadoAutenticacion> ObtenerCategoriasAsync(CancellationToken cancellationToken = default)
    {
        var categorias = await _context.Categorias
            .AsNoTracking()
            .OrderBy(c => c.Nombre)
            .Select(c => new CategoriaDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                EstaActiva = c.EstaActiva,
                FechaCreacion = c.FechaCreacion
            })
            .ToListAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Categorías obtenidas correctamente.", categorias);
    }

    public async Task<ResultadoAutenticacion> ObtenerCategoriaPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var categoria = await _context.Categorias
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (categoria is null)
        {
            return ResultadoAutenticacion.Fallido("Categoría no encontrada.");
        }

        return ResultadoAutenticacion.Correcto("Categoría encontrada.", MapearCategoria(categoria));
    }

    public async Task<ResultadoAutenticacion> CrearCategoriaAsync(CrearCategoriaSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarInventario())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para gestionar categorías.");
        }

        if (string.IsNullOrWhiteSpace(solicitud.Nombre))
        {
            return ResultadoAutenticacion.Fallido("El nombre de la categoría es obligatorio.");
        }

        var categoria = new Categoria
        {
            Nombre = solicitud.Nombre.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(solicitud.Descripcion) ? null : solicitud.Descripcion.Trim(),
            EstaActiva = true,
            FechaCreacion = DateTime.UtcNow
        };

        await _context.Categorias.AddAsync(categoria, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Categoría creada correctamente.", MapearCategoria(categoria));
    }

    public async Task<ResultadoAutenticacion> ActualizarCategoriaAsync(Guid id, ActualizarCategoriaSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarInventario())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para actualizar categorías.");
        }

        var categoria = await _context.Categorias.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (categoria is null)
        {
            return ResultadoAutenticacion.Fallido("Categoría no encontrada.");
        }

        if (!string.IsNullOrWhiteSpace(solicitud.Nombre))
        {
            categoria.Nombre = solicitud.Nombre.Trim();
        }

        if (solicitud.Descripcion is not null)
        {
            categoria.Descripcion = string.IsNullOrWhiteSpace(solicitud.Descripcion) ? null : solicitud.Descripcion.Trim();
        }

        if (solicitud.EstaActiva.HasValue)
        {
            categoria.EstaActiva = solicitud.EstaActiva.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ResultadoAutenticacion.Correcto("Categoría actualizada correctamente.", MapearCategoria(categoria));
    }

    public async Task<ResultadoAutenticacion> EliminarCategoriaAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarInventario())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para eliminar categorías.");
        }

        var categoria = await _context.Categorias.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (categoria is null)
        {
            return ResultadoAutenticacion.Fallido("Categoría no encontrada.");
        }

        var tieneProductos = await _context.Productos.AnyAsync(p => p.CategoriaId == id, cancellationToken);
        if (tieneProductos)
        {
            return ResultadoAutenticacion.Fallido("No se puede eliminar una categoría con productos asociados.");
        }

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Categoría eliminada correctamente.");
    }

    public async Task<ResultadoAutenticacion> ObtenerProductosAsync(CancellationToken cancellationToken = default)
    {
        var productos = await _context.Productos
            .AsNoTracking()
            .Include(p => p.Categoria)
            .OrderBy(p => p.Nombre)
            .Select(p => new ProductoDto
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                UnidadMedida = p.UnidadMedida,
                PrecioUnitario = p.PrecioUnitario,
                StockActual = p.StockActual,
                StockMinimo = p.StockMinimo,
                EstaActivo = p.EstaActivo,
                CategoriaId = p.CategoriaId,
                CategoriaNombre = p.Categoria != null ? p.Categoria.Nombre : null,
                FechaCreacion = p.FechaCreacion,
                FechaActualizacion = p.FechaActualizacion
            })
            .ToListAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Productos obtenidos correctamente.", productos);
    }

    public async Task<ResultadoAutenticacion> ObtenerProductoPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var producto = await _context.Productos
            .AsNoTracking()
            .Include(p => p.Categoria)
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (producto is null)
        {
            return ResultadoAutenticacion.Fallido("Producto no encontrado.");
        }

        return ResultadoAutenticacion.Correcto("Producto encontrado.", MapearProducto(producto));
    }

    public async Task<ResultadoAutenticacion> CrearProductoAsync(CrearProductoSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarInventario())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para gestionar productos.");
        }

        if (string.IsNullOrWhiteSpace(solicitud.Codigo) || string.IsNullOrWhiteSpace(solicitud.Nombre))
        {
            return ResultadoAutenticacion.Fallido("Código y nombre del producto son obligatorios.");
        }

        if (solicitud.PrecioUnitario < 0)
        {
            return ResultadoAutenticacion.Fallido("El precio del producto no puede ser negativo.");
        }

        if (solicitud.StockActual < 0 || solicitud.StockMinimo < 0)
        {
            return ResultadoAutenticacion.Fallido("El stock y el mínimo no pueden ser negativos.");
        }

        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == solicitud.CategoriaId, cancellationToken);
        if (!categoriaExiste)
        {
            return ResultadoAutenticacion.Fallido("La categoría indicada no existe.");
        }

        var codigoExiste = await _context.Productos.AnyAsync(p => p.Codigo == solicitud.Codigo.Trim(), cancellationToken);
        if (codigoExiste)
        {
            return ResultadoAutenticacion.Fallido("Ya existe un producto con ese código.");
        }

        var producto = new Producto
        {
            Codigo = solicitud.Codigo.Trim(),
            Nombre = solicitud.Nombre.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(solicitud.Descripcion) ? null : solicitud.Descripcion.Trim(),
            UnidadMedida = string.IsNullOrWhiteSpace(solicitud.UnidadMedida) ? "Unidad" : solicitud.UnidadMedida.Trim(),
            PrecioUnitario = solicitud.PrecioUnitario,
            StockActual = solicitud.StockActual,
            StockMinimo = solicitud.StockMinimo,
            EstaActivo = solicitud.EstaActivo,
            CategoriaId = solicitud.CategoriaId,
            FechaCreacion = DateTime.UtcNow
        };

        await _context.Productos.AddAsync(producto, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Producto creado correctamente.", MapearProducto(producto));
    }

    public async Task<ResultadoAutenticacion> ActualizarProductoAsync(Guid id, ActualizarProductoSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarInventario())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para actualizar productos.");
        }

        var producto = await _context.Productos.Include(p => p.Categoria).SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (producto is null)
        {
            return ResultadoAutenticacion.Fallido("Producto no encontrado.");
        }

        if (!string.IsNullOrWhiteSpace(solicitud.Codigo))
        {
            var existeCodigo = await _context.Productos.AnyAsync(p => p.Id != id && p.Codigo == solicitud.Codigo.Trim(), cancellationToken);
            if (existeCodigo)
            {
                return ResultadoAutenticacion.Fallido("Ya existe un producto con ese código.");
            }

            producto.Codigo = solicitud.Codigo.Trim();
        }

        if (!string.IsNullOrWhiteSpace(solicitud.Nombre))
        {
            producto.Nombre = solicitud.Nombre.Trim();
        }

        if (solicitud.Descripcion is not null)
        {
            producto.Descripcion = string.IsNullOrWhiteSpace(solicitud.Descripcion) ? null : solicitud.Descripcion.Trim();
        }

        if (!string.IsNullOrWhiteSpace(solicitud.UnidadMedida))
        {
            producto.UnidadMedida = solicitud.UnidadMedida.Trim();
        }

        if (solicitud.PrecioUnitario.HasValue)
        {
            if (solicitud.PrecioUnitario.Value < 0)
            {
                return ResultadoAutenticacion.Fallido("El precio del producto no puede ser negativo.");
            }

            producto.PrecioUnitario = solicitud.PrecioUnitario.Value;
        }

        if (solicitud.StockActual.HasValue)
        {
            if (solicitud.StockActual.Value < 0)
            {
                return ResultadoAutenticacion.Fallido("El stock no puede ser negativo.");
            }

            producto.StockActual = solicitud.StockActual.Value;
        }

        if (solicitud.StockMinimo.HasValue)
        {
            if (solicitud.StockMinimo.Value < 0)
            {
                return ResultadoAutenticacion.Fallido("El stock mínimo no puede ser negativo.");
            }

            producto.StockMinimo = solicitud.StockMinimo.Value;
        }

        if (solicitud.EstaActivo.HasValue)
        {
            producto.EstaActivo = solicitud.EstaActivo.Value;
        }

        if (solicitud.CategoriaId.HasValue)
        {
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == solicitud.CategoriaId.Value, cancellationToken);
            if (!categoriaExiste)
            {
                return ResultadoAutenticacion.Fallido("La categoría indicada no existe.");
            }

            producto.CategoriaId = solicitud.CategoriaId.Value;
        }

        producto.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Producto actualizado correctamente.", MapearProducto(producto));
    }

    public async Task<ResultadoAutenticacion> EliminarProductoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarInventario())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para eliminar productos.");
        }

        var producto = await _context.Productos.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (producto is null)
        {
            return ResultadoAutenticacion.Fallido("Producto no encontrado.");
        }

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Producto eliminado correctamente.");
    }

    public async Task<ResultadoAutenticacion> ObtenerExistenciasAsync(CancellationToken cancellationToken = default)
    {
        var existencias = await _context.ExistenciasInventario
            .AsNoTracking()
            .Include(e => e.Producto)
            .Include(e => e.Almacen)
            .OrderBy(e => e.Producto!.Nombre)
            .Select(e => new ExistenciaInventarioDto
            {
                Id = e.Id,
                ProductoId = e.ProductoId,
                ProductoNombre = e.Producto != null ? e.Producto.Nombre : string.Empty,
                AlmacenId = e.AlmacenId,
                AlmacenNombre = e.Almacen != null ? e.Almacen.Nombre : string.Empty,
                CantidadDisponible = e.CantidadDisponible,
                CantidadReservada = e.CantidadReservada,
                StockDisponible = e.StockDisponible,
                FechaActualizacion = e.FechaActualizacion
            })
            .ToListAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Existencias del inventario obtenidas correctamente.", existencias);
    }

    public async Task<ResultadoAutenticacion> RegistrarMovimientoAsync(RegistrarMovimientoInventarioSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarInventario())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para registrar movimientos de inventario.");
        }

        if (solicitud.Cantidad == 0)
        {
            return ResultadoAutenticacion.Fallido("La cantidad del movimiento debe ser distinta de cero.");
        }

        var producto = await _context.Productos.SingleOrDefaultAsync(p => p.Id == solicitud.ProductoId, cancellationToken);
        if (producto is null)
        {
            return ResultadoAutenticacion.Fallido("Producto no encontrado.");
        }

        var almacen = await _context.Almacenes.SingleOrDefaultAsync(a => a.Id == solicitud.AlmacenId, cancellationToken);
        if (almacen is null)
        {
            return ResultadoAutenticacion.Fallido("Almacén no encontrado.");
        }

        var stockAnterior = producto.StockActual;
        var stockResultante = stockAnterior;

        switch (solicitud.Tipo)
        {
            case TipoMovimiento.Entrada:
                stockResultante = stockAnterior + solicitud.Cantidad;
                break;
            case TipoMovimiento.Salida:
                if (solicitud.Cantidad > stockAnterior)
                {
                    return ResultadoAutenticacion.Fallido("No hay suficiente stock para registrar la salida solicitada.");
                }

                stockResultante = stockAnterior - solicitud.Cantidad;
                break;
            case TipoMovimiento.Ajuste:
                stockResultante = stockAnterior + solicitud.Cantidad;
                break;
            default:
                return ResultadoAutenticacion.Fallido("Tipo de movimiento no válido.");
        }

        producto.StockActual = stockResultante;
        producto.FechaActualizacion = DateTime.UtcNow;

        var existencia = await _context.ExistenciasInventario
            .SingleOrDefaultAsync(e => e.ProductoId == solicitud.ProductoId && e.AlmacenId == solicitud.AlmacenId, cancellationToken);

        if (existencia is null)
        {
            existencia = new ExistenciaInventario
            {
                ProductoId = solicitud.ProductoId,
                AlmacenId = solicitud.AlmacenId,
                CantidadDisponible = stockResultante,
                CantidadReservada = 0,
                FechaActualizacion = DateTime.UtcNow
            };
            await _context.ExistenciasInventario.AddAsync(existencia, cancellationToken);
        }
        else
        {
            existencia.CantidadDisponible = stockResultante;
            existencia.CantidadReservada = 0;
            existencia.FechaActualizacion = DateTime.UtcNow;
        }

        var movimiento = new MovimientoInventario
        {
            ProductoId = solicitud.ProductoId,
            AlmacenId = solicitud.AlmacenId,
            Tipo = solicitud.Tipo,
            Cantidad = solicitud.Cantidad,
            StockAnterior = stockAnterior,
            StockResultante = stockResultante,
            Observacion = string.IsNullOrWhiteSpace(solicitud.Observacion) ? null : solicitud.Observacion.Trim(),
            FechaMovimiento = DateTime.UtcNow
        };

        await _context.MovimientosInventario.AddAsync(movimiento, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Movimiento de inventario registrado correctamente.", new MovimientoInventarioDto
        {
            Id = movimiento.Id,
            ProductoId = producto.Id,
            ProductoNombre = producto.Nombre,
            AlmacenId = almacen.Id,
            AlmacenNombre = almacen.Nombre,
            Tipo = movimiento.Tipo,
            Cantidad = movimiento.Cantidad,
            StockAnterior = movimiento.StockAnterior,
            StockResultante = movimiento.StockResultante,
            Observacion = movimiento.Observacion,
            FechaMovimiento = movimiento.FechaMovimiento
        });
    }

    private bool PuedeGestionarInventario()
    {
        return _usuarioActualServicio.TieneRol(RolesSistema.Administrador)
            || _usuarioActualServicio.TieneRol(RolesSistema.CoordinadorLogistico)
            || _usuarioActualServicio.TieneRol(RolesSistema.AuxiliarBodega);
    }

    private static CategoriaDto MapearCategoria(Categoria categoria) => new()
    {
        Id = categoria.Id,
        Nombre = categoria.Nombre,
        Descripcion = categoria.Descripcion,
        EstaActiva = categoria.EstaActiva,
        FechaCreacion = categoria.FechaCreacion
    };

    private static ProductoDto MapearProducto(Producto producto) => new()
    {
        Id = producto.Id,
        Codigo = producto.Codigo,
        Nombre = producto.Nombre,
        Descripcion = producto.Descripcion,
        UnidadMedida = producto.UnidadMedida,
        PrecioUnitario = producto.PrecioUnitario,
        StockActual = producto.StockActual,
        StockMinimo = producto.StockMinimo,
        EstaActivo = producto.EstaActivo,
        CategoriaId = producto.CategoriaId,
        CategoriaNombre = producto.Categoria?.Nombre,
        FechaCreacion = producto.FechaCreacion,
        FechaActualizacion = producto.FechaActualizacion
    };
}
