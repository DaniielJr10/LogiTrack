using LogiTrack.Aplicacion;
using LogiTrack.Aplicacion.DTOs.Logistica;
using LogiTrack.Aplicacion.Interfaces.Seguridad;
using LogiTrack.Aplicacion.Interfaces.Servicios;
using LogiTrack.Dominio.Entidades.Logistica;
using LogiTrack.Infraestructura.BaseDatos.Contexto;
using LogiTrack.Infraestructura.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Infraestructura.Servicios;

public class ServicioLogistica : IServicioLogistica
{
    private readonly LogiTrackDbContext _context;
    private readonly IUsuarioActualServicio _usuarioActualServicio;

    public ServicioLogistica(LogiTrackDbContext context, IUsuarioActualServicio usuarioActualServicio)
    {
        _context = context;
        _usuarioActualServicio = usuarioActualServicio;
    }

    public async Task<ResultadoAutenticacion> ObtenerVehiculosAsync(CancellationToken cancellationToken = default)
    {
        var vehiculos = await _context.Vehiculos
            .AsNoTracking()
            .OrderBy(v => v.Marca)
            .ThenBy(v => v.Modelo)
            .Select(v => new VehiculoDto
            {
                Id = v.Id,
                Placa = v.Placa,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Tipo = v.Tipo,
                Anio = v.Anio,
                CapacidadKg = v.CapacidadKg,
                EstaDisponible = v.EstaDisponible,
                FechaRegistro = v.FechaRegistro
            })
            .ToListAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Vehículos obtenidos correctamente.", vehiculos);
    }

    public async Task<ResultadoAutenticacion> ObtenerVehiculoPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var vehiculo = await _context.Vehiculos.AsNoTracking().SingleOrDefaultAsync(v => v.Id == id, cancellationToken);
        if (vehiculo is null)
        {
            return ResultadoAutenticacion.Fallido("Vehículo no encontrado.");
        }

        return ResultadoAutenticacion.Correcto("Vehículo encontrado.", MapearVehiculo(vehiculo));
    }

    public async Task<ResultadoAutenticacion> CrearVehiculoAsync(CrearVehiculoSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para gestionar vehículos.");
        }

        if (string.IsNullOrWhiteSpace(solicitud.Placa) || string.IsNullOrWhiteSpace(solicitud.Marca) || string.IsNullOrWhiteSpace(solicitud.Modelo))
        {
            return ResultadoAutenticacion.Fallido("La placa, marca y modelo del vehículo son obligatorios.");
        }

        var placaExiste = await _context.Vehiculos.AnyAsync(v => v.Placa == solicitud.Placa.Trim(), cancellationToken);
        if (placaExiste)
        {
            return ResultadoAutenticacion.Fallido("Ya existe un vehículo con esa placa.");
        }

        var vehiculo = new Vehiculo
        {
            Placa = solicitud.Placa.Trim(),
            Marca = solicitud.Marca.Trim(),
            Modelo = solicitud.Modelo.Trim(),
            Tipo = string.IsNullOrWhiteSpace(solicitud.Tipo) ? "Camión" : solicitud.Tipo.Trim(),
            Anio = solicitud.Anio,
            CapacidadKg = solicitud.CapacidadKg,
            EstaDisponible = solicitud.EstaDisponible,
            FechaRegistro = DateTime.UtcNow
        };

        await _context.Vehiculos.AddAsync(vehiculo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Vehículo creado correctamente.", MapearVehiculo(vehiculo));
    }

    public async Task<ResultadoAutenticacion> ActualizarVehiculoAsync(Guid id, ActualizarVehiculoSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para actualizar vehículos.");
        }

        var vehiculo = await _context.Vehiculos.SingleOrDefaultAsync(v => v.Id == id, cancellationToken);
        if (vehiculo is null)
        {
            return ResultadoAutenticacion.Fallido("Vehículo no encontrado.");
        }

        if (!string.IsNullOrWhiteSpace(solicitud.Placa))
        {
            var placaDuplicada = await _context.Vehiculos.AnyAsync(v => v.Id != id && v.Placa == solicitud.Placa.Trim(), cancellationToken);
            if (placaDuplicada)
            {
                return ResultadoAutenticacion.Fallido("Ya existe otro vehículo con esa placa.");
            }

            vehiculo.Placa = solicitud.Placa.Trim();
        }

        if (!string.IsNullOrWhiteSpace(solicitud.Marca)) vehiculo.Marca = solicitud.Marca.Trim();
        if (!string.IsNullOrWhiteSpace(solicitud.Modelo)) vehiculo.Modelo = solicitud.Modelo.Trim();
        if (!string.IsNullOrWhiteSpace(solicitud.Tipo)) vehiculo.Tipo = solicitud.Tipo.Trim();
        if (solicitud.Anio.HasValue) vehiculo.Anio = solicitud.Anio.Value;
        if (solicitud.CapacidadKg.HasValue) vehiculo.CapacidadKg = solicitud.CapacidadKg.Value;
        if (solicitud.EstaDisponible.HasValue) vehiculo.EstaDisponible = solicitud.EstaDisponible.Value;

        await _context.SaveChangesAsync(cancellationToken);
        return ResultadoAutenticacion.Correcto("Vehículo actualizado correctamente.", MapearVehiculo(vehiculo));
    }

    public async Task<ResultadoAutenticacion> EliminarVehiculoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para eliminar vehículos.");
        }

        var vehiculo = await _context.Vehiculos.SingleOrDefaultAsync(v => v.Id == id, cancellationToken);
        if (vehiculo is null)
        {
            return ResultadoAutenticacion.Fallido("Vehículo no encontrado.");
        }

        var tieneDespachos = await _context.Despachos.AnyAsync(d => d.VehiculoId == id, cancellationToken);
        if (tieneDespachos)
        {
            return ResultadoAutenticacion.Fallido("No se puede eliminar un vehículo que está asignado a despachos.");
        }

        _context.Vehiculos.Remove(vehiculo);
        await _context.SaveChangesAsync(cancellationToken);
        return ResultadoAutenticacion.Correcto("Vehículo eliminado correctamente.");
    }

    public async Task<ResultadoAutenticacion> ObtenerConductoresAsync(CancellationToken cancellationToken = default)
    {
        var conductores = await _context.Conductores
            .AsNoTracking()
            .OrderBy(c => c.Apellidos)
            .ThenBy(c => c.Nombres)
            .Select(c => new ConductorDto
            {
                Id = c.Id,
                Nombres = c.Nombres,
                Apellidos = c.Apellidos,
                NumeroDocumento = c.NumeroDocumento,
                Licencia = c.Licencia,
                Telefono = c.Telefono,
                EstaDisponible = c.EstaDisponible,
                FechaRegistro = c.FechaRegistro
            })
            .ToListAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Conductores obtenidos correctamente.", conductores);
    }

    public async Task<ResultadoAutenticacion> ObtenerConductorPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var conductor = await _context.Conductores.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (conductor is null)
        {
            return ResultadoAutenticacion.Fallido("Conductor no encontrado.");
        }

        return ResultadoAutenticacion.Correcto("Conductor encontrado.", MapearConductor(conductor));
    }

    public async Task<ResultadoAutenticacion> CrearConductorAsync(CrearConductorSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para gestionar conductores.");
        }

        if (string.IsNullOrWhiteSpace(solicitud.Nombres) || string.IsNullOrWhiteSpace(solicitud.Apellidos) || string.IsNullOrWhiteSpace(solicitud.NumeroDocumento))
        {
            return ResultadoAutenticacion.Fallido("Nombre, apellidos y documento del conductor son obligatorios.");
        }

        var documentoExiste = await _context.Conductores.AnyAsync(c => c.NumeroDocumento == solicitud.NumeroDocumento.Trim(), cancellationToken);
        if (documentoExiste)
        {
            return ResultadoAutenticacion.Fallido("Ya existe un conductor con ese número de documento.");
        }

        var conductor = new Conductor
        {
            Nombres = solicitud.Nombres.Trim(),
            Apellidos = solicitud.Apellidos.Trim(),
            NumeroDocumento = solicitud.NumeroDocumento.Trim(),
            Licencia = string.IsNullOrWhiteSpace(solicitud.Licencia) ? null : solicitud.Licencia.Trim(),
            Telefono = string.IsNullOrWhiteSpace(solicitud.Telefono) ? null : solicitud.Telefono.Trim(),
            EstaDisponible = solicitud.EstaDisponible,
            FechaRegistro = DateTime.UtcNow
        };

        await _context.Conductores.AddAsync(conductor, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Conductor creado correctamente.", MapearConductor(conductor));
    }

    public async Task<ResultadoAutenticacion> ActualizarConductorAsync(Guid id, ActualizarConductorSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para actualizar conductores.");
        }

        var conductor = await _context.Conductores.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (conductor is null)
        {
            return ResultadoAutenticacion.Fallido("Conductor no encontrado.");
        }

        if (!string.IsNullOrWhiteSpace(solicitud.Nombres)) conductor.Nombres = solicitud.Nombres.Trim();
        if (!string.IsNullOrWhiteSpace(solicitud.Apellidos)) conductor.Apellidos = solicitud.Apellidos.Trim();
        if (!string.IsNullOrWhiteSpace(solicitud.NumeroDocumento))
        {
            var documentoDuplicado = await _context.Conductores.AnyAsync(c => c.Id != id && c.NumeroDocumento == solicitud.NumeroDocumento.Trim(), cancellationToken);
            if (documentoDuplicado)
            {
                return ResultadoAutenticacion.Fallido("Ya existe otro conductor con ese número de documento.");
            }

            conductor.NumeroDocumento = solicitud.NumeroDocumento.Trim();
        }

        if (solicitud.Licencia is not null) conductor.Licencia = string.IsNullOrWhiteSpace(solicitud.Licencia) ? null : solicitud.Licencia.Trim();
        if (solicitud.Telefono is not null) conductor.Telefono = string.IsNullOrWhiteSpace(solicitud.Telefono) ? null : solicitud.Telefono.Trim();
        if (solicitud.EstaDisponible.HasValue) conductor.EstaDisponible = solicitud.EstaDisponible.Value;

        await _context.SaveChangesAsync(cancellationToken);
        return ResultadoAutenticacion.Correcto("Conductor actualizado correctamente.", MapearConductor(conductor));
    }

    public async Task<ResultadoAutenticacion> EliminarConductorAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para eliminar conductores.");
        }

        var conductor = await _context.Conductores.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (conductor is null)
        {
            return ResultadoAutenticacion.Fallido("Conductor no encontrado.");
        }

        var tieneDespachos = await _context.Despachos.AnyAsync(d => d.ConductorId == id, cancellationToken);
        if (tieneDespachos)
        {
            return ResultadoAutenticacion.Fallido("No se puede eliminar un conductor que está asignado a despachos.");
        }

        _context.Conductores.Remove(conductor);
        await _context.SaveChangesAsync(cancellationToken);
        return ResultadoAutenticacion.Correcto("Conductor eliminado correctamente.");
    }

    public async Task<ResultadoAutenticacion> ObtenerRutasAsync(CancellationToken cancellationToken = default)
    {
        var rutas = await _context.Rutas
            .AsNoTracking()
            .OrderBy(r => r.Nombre)
            .Select(r => new RutaDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Origen = r.Origen,
                Destino = r.Destino,
                Descripcion = r.Descripcion,
                Estado = r.Estado,
                FechaCreacion = r.FechaCreacion
            })
            .ToListAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Rutas obtenidas correctamente.", rutas);
    }

    public async Task<ResultadoAutenticacion> ObtenerRutaPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ruta = await _context.Rutas.AsNoTracking().SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (ruta is null)
        {
            return ResultadoAutenticacion.Fallido("Ruta no encontrada.");
        }

        return ResultadoAutenticacion.Correcto("Ruta encontrada.", MapearRuta(ruta));
    }

    public async Task<ResultadoAutenticacion> CrearRutaAsync(CrearRutaSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para gestionar rutas.");
        }

        if (string.IsNullOrWhiteSpace(solicitud.Nombre) || string.IsNullOrWhiteSpace(solicitud.Origen) || string.IsNullOrWhiteSpace(solicitud.Destino))
        {
            return ResultadoAutenticacion.Fallido("Nombre, origen y destino de la ruta son obligatorios.");
        }

        var ruta = new Ruta
        {
            Nombre = solicitud.Nombre.Trim(),
            Origen = solicitud.Origen.Trim(),
            Destino = solicitud.Destino.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(solicitud.Descripcion) ? null : solicitud.Descripcion.Trim(),
            Estado = solicitud.Estado,
            FechaCreacion = DateTime.UtcNow
        };

        await _context.Rutas.AddAsync(ruta, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Ruta creada correctamente.", MapearRuta(ruta));
    }

    public async Task<ResultadoAutenticacion> ActualizarRutaAsync(Guid id, ActualizarRutaSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para actualizar rutas.");
        }

        var ruta = await _context.Rutas.SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (ruta is null)
        {
            return ResultadoAutenticacion.Fallido("Ruta no encontrada.");
        }

        if (!string.IsNullOrWhiteSpace(solicitud.Nombre)) ruta.Nombre = solicitud.Nombre.Trim();
        if (!string.IsNullOrWhiteSpace(solicitud.Origen)) ruta.Origen = solicitud.Origen.Trim();
        if (!string.IsNullOrWhiteSpace(solicitud.Destino)) ruta.Destino = solicitud.Destino.Trim();
        if (solicitud.Descripcion is not null) ruta.Descripcion = string.IsNullOrWhiteSpace(solicitud.Descripcion) ? null : solicitud.Descripcion.Trim();
        if (solicitud.Estado.HasValue) ruta.Estado = solicitud.Estado.Value;

        await _context.SaveChangesAsync(cancellationToken);
        return ResultadoAutenticacion.Correcto("Ruta actualizada correctamente.", MapearRuta(ruta));
    }

    public async Task<ResultadoAutenticacion> EliminarRutaAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para eliminar rutas.");
        }

        var ruta = await _context.Rutas.SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (ruta is null)
        {
            return ResultadoAutenticacion.Fallido("Ruta no encontrada.");
        }

        var tieneDespachos = await _context.Despachos.AnyAsync(d => d.RutaId == id, cancellationToken);
        if (tieneDespachos)
        {
            return ResultadoAutenticacion.Fallido("No se puede eliminar una ruta con despachos asociados.");
        }

        _context.Rutas.Remove(ruta);
        await _context.SaveChangesAsync(cancellationToken);
        return ResultadoAutenticacion.Correcto("Ruta eliminada correctamente.");
    }

    public async Task<ResultadoAutenticacion> ObtenerDespachosAsync(CancellationToken cancellationToken = default)
    {
        var despachos = await _context.Despachos
            .AsNoTracking()
            .Include(d => d.Ruta)
            .Include(d => d.Vehiculo)
            .Include(d => d.Conductor)
            .OrderByDescending(d => d.FechaProgramada)
            .ToListAsync(cancellationToken);

        var resultado = despachos
            .Select(MapearDespacho)
            .ToList();

        return ResultadoAutenticacion.Correcto("Despachos obtenidos correctamente.", resultado);
    }

    public async Task<ResultadoAutenticacion> ObtenerDespachoPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var despacho = await _context.Despachos
            .AsNoTracking()
            .Include(d => d.Ruta)
            .Include(d => d.Vehiculo)
            .Include(d => d.Conductor)
            .SingleOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (despacho is null)
        {
            return ResultadoAutenticacion.Fallido("Despacho no encontrado.");
        }

        return ResultadoAutenticacion.Correcto("Despacho encontrado.", MapearDespacho(despacho));
    }

    public async Task<ResultadoAutenticacion> CrearDespachoAsync(CrearDespachoSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para gestionar despachos.");
        }

        var rutaExiste = await _context.Rutas.AnyAsync(r => r.Id == solicitud.RutaId, cancellationToken);
        if (!rutaExiste)
        {
            return ResultadoAutenticacion.Fallido("La ruta especificada no existe.");
        }

        if (solicitud.VehiculoId.HasValue)
        {
            var vehiculoExiste = await _context.Vehiculos.AnyAsync(v => v.Id == solicitud.VehiculoId.Value, cancellationToken);
            if (!vehiculoExiste)
            {
                return ResultadoAutenticacion.Fallido("El vehículo especificado no existe.");
            }
        }

        if (solicitud.ConductorId.HasValue)
        {
            var conductorExiste = await _context.Conductores.AnyAsync(c => c.Id == solicitud.ConductorId.Value, cancellationToken);
            if (!conductorExiste)
            {
                return ResultadoAutenticacion.Fallido("El conductor especificado no existe.");
            }
        }

        var despacho = new Despacho
        {
            RutaId = solicitud.RutaId,
            VehiculoId = solicitud.VehiculoId,
            ConductorId = solicitud.ConductorId,
            Estado = solicitud.Estado,
            FechaProgramada = solicitud.FechaProgramada ?? DateTime.UtcNow,
            FechaInicio = null,
            FechaFin = null,
            Observaciones = string.IsNullOrWhiteSpace(solicitud.Observaciones) ? null : solicitud.Observaciones.Trim()
        };

        await _context.Despachos.AddAsync(despacho, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Despacho creado correctamente.", await ObtenerDespachoMapeadoAsync(despacho.Id, cancellationToken));
    }

    public async Task<ResultadoAutenticacion> ActualizarDespachoAsync(Guid id, ActualizarDespachoSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para actualizar despachos.");
        }

        var despacho = await _context.Despachos.SingleOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (despacho is null)
        {
            return ResultadoAutenticacion.Fallido("Despacho no encontrado.");
        }

        if (solicitud.RutaId.HasValue)
        {
            var rutaExiste = await _context.Rutas.AnyAsync(r => r.Id == solicitud.RutaId.Value, cancellationToken);
            if (!rutaExiste) return ResultadoAutenticacion.Fallido("La ruta especificada no existe.");
            despacho.RutaId = solicitud.RutaId.Value;
        }

        if (solicitud.VehiculoId.HasValue)
        {
            var vehiculoExiste = await _context.Vehiculos.AnyAsync(v => v.Id == solicitud.VehiculoId.Value, cancellationToken);
            if (!vehiculoExiste) return ResultadoAutenticacion.Fallido("El vehículo especificado no existe.");
            despacho.VehiculoId = solicitud.VehiculoId.Value;
        }

        if (solicitud.ConductorId.HasValue)
        {
            var conductorExiste = await _context.Conductores.AnyAsync(c => c.Id == solicitud.ConductorId.Value, cancellationToken);
            if (!conductorExiste) return ResultadoAutenticacion.Fallido("El conductor especificado no existe.");
            despacho.ConductorId = solicitud.ConductorId.Value;
        }

        if (solicitud.Estado.HasValue) despacho.Estado = solicitud.Estado.Value;
        if (solicitud.FechaProgramada.HasValue) despacho.FechaProgramada = solicitud.FechaProgramada.Value;
        if (solicitud.FechaInicio.HasValue) despacho.FechaInicio = solicitud.FechaInicio.Value;
        if (solicitud.FechaFin.HasValue) despacho.FechaFin = solicitud.FechaFin.Value;
        if (solicitud.Observaciones is not null) despacho.Observaciones = string.IsNullOrWhiteSpace(solicitud.Observaciones) ? null : solicitud.Observaciones.Trim();

        await _context.SaveChangesAsync(cancellationToken);
        return ResultadoAutenticacion.Correcto("Despacho actualizado correctamente.", await ObtenerDespachoMapeadoAsync(despacho.Id, cancellationToken));
    }

    public async Task<ResultadoAutenticacion> EliminarDespachoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para eliminar despachos.");
        }

        var despacho = await _context.Despachos.SingleOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (despacho is null)
        {
            return ResultadoAutenticacion.Fallido("Despacho no encontrado.");
        }

        var tieneEntregas = await _context.Entregas.AnyAsync(e => e.DespachoId == id, cancellationToken);
        if (tieneEntregas)
        {
            return ResultadoAutenticacion.Fallido("No se puede eliminar un despacho que ya tiene entregas asociadas.");
        }

        _context.Despachos.Remove(despacho);
        await _context.SaveChangesAsync(cancellationToken);
        return ResultadoAutenticacion.Correcto("Despacho eliminado correctamente.");
    }

    public async Task<ResultadoAutenticacion> ObtenerEntregasAsync(CancellationToken cancellationToken = default)
    {
        var entregas = await _context.Entregas
            .AsNoTracking()
            .OrderBy(e => e.FechaProgramada)
            .ToListAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Entregas obtenidas correctamente.", entregas.Select(MapearEntrega).ToList());
    }

    public async Task<ResultadoAutenticacion> ObtenerEntregaPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entrega = await _context.Entregas.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entrega is null)
        {
            return ResultadoAutenticacion.Fallido("Entrega no encontrada.");
        }

        return ResultadoAutenticacion.Correcto("Entrega encontrada.", MapearEntrega(entrega));
    }

    public async Task<ResultadoAutenticacion> CrearEntregaAsync(CrearEntregaSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para gestionar entregas.");
        }

        var despachoExiste = await _context.Despachos.AnyAsync(d => d.Id == solicitud.DespachoId, cancellationToken);
        if (!despachoExiste)
        {
            return ResultadoAutenticacion.Fallido("El despacho especificado no existe.");
        }

        var entrega = new Entrega
        {
            DespachoId = solicitud.DespachoId,
            PedidoId = solicitud.PedidoId,
            DireccionEntrega = string.IsNullOrWhiteSpace(solicitud.DireccionEntrega) ? null : solicitud.DireccionEntrega.Trim(),
            CiudadEntrega = string.IsNullOrWhiteSpace(solicitud.CiudadEntrega) ? null : solicitud.CiudadEntrega.Trim(),
            Estado = solicitud.Estado,
            FechaProgramada = solicitud.FechaProgramada ?? DateTime.UtcNow,
            FechaEntregaReal = null,
            Latitud = solicitud.Latitud,
            Longitud = solicitud.Longitud
        };

        await _context.Entregas.AddAsync(entrega, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Entrega creada correctamente.", MapearEntrega(entrega));
    }

    public async Task<ResultadoAutenticacion> ActualizarEntregaAsync(Guid id, ActualizarEntregaSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para actualizar entregas.");
        }

        var entrega = await _context.Entregas.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entrega is null)
        {
            return ResultadoAutenticacion.Fallido("Entrega no encontrada.");
        }

        if (solicitud.DespachoId.HasValue)
        {
            var despachoExiste = await _context.Despachos.AnyAsync(d => d.Id == solicitud.DespachoId.Value, cancellationToken);
            if (!despachoExiste) return ResultadoAutenticacion.Fallido("El despacho especificado no existe.");
            entrega.DespachoId = solicitud.DespachoId.Value;
        }

        if (solicitud.PedidoId.HasValue) entrega.PedidoId = solicitud.PedidoId.Value;
        if (solicitud.DireccionEntrega is not null) entrega.DireccionEntrega = string.IsNullOrWhiteSpace(solicitud.DireccionEntrega) ? null : solicitud.DireccionEntrega.Trim();
        if (solicitud.CiudadEntrega is not null) entrega.CiudadEntrega = string.IsNullOrWhiteSpace(solicitud.CiudadEntrega) ? null : solicitud.CiudadEntrega.Trim();
        if (solicitud.Estado.HasValue) entrega.Estado = solicitud.Estado.Value;
        if (solicitud.FechaProgramada.HasValue) entrega.FechaProgramada = solicitud.FechaProgramada.Value;
        if (solicitud.FechaEntregaReal.HasValue) entrega.FechaEntregaReal = solicitud.FechaEntregaReal.Value;
        if (solicitud.Latitud.HasValue) entrega.Latitud = solicitud.Latitud.Value;
        if (solicitud.Longitud.HasValue) entrega.Longitud = solicitud.Longitud.Value;

        await _context.SaveChangesAsync(cancellationToken);
        return ResultadoAutenticacion.Correcto("Entrega actualizada correctamente.", MapearEntrega(entrega));
    }

    public async Task<ResultadoAutenticacion> EliminarEntregaAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para eliminar entregas.");
        }

        var entrega = await _context.Entregas.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entrega is null)
        {
            return ResultadoAutenticacion.Fallido("Entrega no encontrada.");
        }

        var tieneSeguimiento = await _context.HistorialSeguimientos.AnyAsync(h => h.EntregaId == id, cancellationToken);
        if (tieneSeguimiento)
        {
            return ResultadoAutenticacion.Fallido("No se puede eliminar una entrega con historial de seguimiento asociado.");
        }

        _context.Entregas.Remove(entrega);
        await _context.SaveChangesAsync(cancellationToken);
        return ResultadoAutenticacion.Correcto("Entrega eliminada correctamente.");
    }

    public async Task<ResultadoAutenticacion> ObtenerSeguimientoEntregaAsync(Guid entregaId, CancellationToken cancellationToken = default)
    {
        var entregaExiste = await _context.Entregas.AnyAsync(e => e.Id == entregaId, cancellationToken);
        if (!entregaExiste)
        {
            return ResultadoAutenticacion.Fallido("La entrega especificada no existe.");
        }

        var seguimiento = await _context.HistorialSeguimientos
            .AsNoTracking()
            .Where(h => h.EntregaId == entregaId)
            .OrderByDescending(h => h.FechaHora)
            .Select(h => new HistorialSeguimientoDto
            {
                Id = h.Id,
                EntregaId = h.EntregaId,
                Latitud = h.Latitud,
                Longitud = h.Longitud,
                Descripcion = h.Descripcion,
                FechaHora = h.FechaHora
            })
            .ToListAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Seguimiento obtenido correctamente.", seguimiento);
    }

    public async Task<ResultadoAutenticacion> RegistrarSeguimientoAsync(CrearHistorialSeguimientoSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (!PuedeGestionarLogistica())
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para registrar seguimiento.");
        }

        var entregaExiste = await _context.Entregas.AnyAsync(e => e.Id == solicitud.EntregaId, cancellationToken);
        if (!entregaExiste)
        {
            return ResultadoAutenticacion.Fallido("La entrega especificada no existe.");
        }

        if (string.IsNullOrWhiteSpace(solicitud.Descripcion))
        {
            return ResultadoAutenticacion.Fallido("La descripción del seguimiento es obligatoria.");
        }

        var registro = new HistorialSeguimiento
        {
            EntregaId = solicitud.EntregaId,
            Latitud = solicitud.Latitud,
            Longitud = solicitud.Longitud,
            Descripcion = solicitud.Descripcion.Trim(),
            FechaHora = DateTime.UtcNow
        };

        await _context.HistorialSeguimientos.AddAsync(registro, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultadoAutenticacion.Correcto("Registro de seguimiento agregado correctamente.", MapearSeguimiento(registro));
    }

    private bool PuedeGestionarLogistica()
    {
        return _usuarioActualServicio.TieneRol(RolesSistema.Administrador)
            || _usuarioActualServicio.TieneRol(RolesSistema.CoordinadorLogistico);
    }

    private static VehiculoDto MapearVehiculo(Vehiculo vehiculo) => new()
    {
        Id = vehiculo.Id,
        Placa = vehiculo.Placa,
        Marca = vehiculo.Marca,
        Modelo = vehiculo.Modelo,
        Tipo = vehiculo.Tipo,
        Anio = vehiculo.Anio,
        CapacidadKg = vehiculo.CapacidadKg,
        EstaDisponible = vehiculo.EstaDisponible,
        FechaRegistro = vehiculo.FechaRegistro
    };

    private static ConductorDto MapearConductor(Conductor conductor) => new()
    {
        Id = conductor.Id,
        Nombres = conductor.Nombres,
        Apellidos = conductor.Apellidos,
        NumeroDocumento = conductor.NumeroDocumento,
        Licencia = conductor.Licencia,
        Telefono = conductor.Telefono,
        EstaDisponible = conductor.EstaDisponible,
        FechaRegistro = conductor.FechaRegistro
    };

    private static RutaDto MapearRuta(Ruta ruta) => new()
    {
        Id = ruta.Id,
        Nombre = ruta.Nombre,
        Origen = ruta.Origen,
        Destino = ruta.Destino,
        Descripcion = ruta.Descripcion,
        Estado = ruta.Estado,
        FechaCreacion = ruta.FechaCreacion
    };

    private static DespachoDto MapearDespacho(Despacho despacho) => new()
    {
        Id = despacho.Id,
        RutaId = despacho.RutaId,
        RutaNombre = despacho.Ruta?.Nombre,
        VehiculoId = despacho.VehiculoId,
        VehiculoPlaca = despacho.Vehiculo?.Placa,
        ConductorId = despacho.ConductorId,
        ConductorNombre = despacho.Conductor != null ? $"{despacho.Conductor.Nombres} {despacho.Conductor.Apellidos}".Trim() : null,
        Estado = despacho.Estado,
        FechaProgramada = despacho.FechaProgramada,
        FechaInicio = despacho.FechaInicio,
        FechaFin = despacho.FechaFin,
        Observaciones = despacho.Observaciones
    };

    private async Task<DespachoDto> ObtenerDespachoMapeadoAsync(Guid id, CancellationToken cancellationToken)
    {
        var despacho = await _context.Despachos
            .Include(d => d.Ruta)
            .Include(d => d.Vehiculo)
            .Include(d => d.Conductor)
            .SingleAsync(d => d.Id == id, cancellationToken);

        return MapearDespacho(despacho);
    }

    private static EntregaDto MapearEntrega(Entrega entrega) => new()
    {
        Id = entrega.Id,
        DespachoId = entrega.DespachoId,
        PedidoId = entrega.PedidoId,
        DireccionEntrega = entrega.DireccionEntrega,
        CiudadEntrega = entrega.CiudadEntrega,
        Estado = entrega.Estado,
        FechaProgramada = entrega.FechaProgramada,
        FechaEntregaReal = entrega.FechaEntregaReal,
        Latitud = entrega.Latitud,
        Longitud = entrega.Longitud
    };

    private static HistorialSeguimientoDto MapearSeguimiento(HistorialSeguimiento registro) => new()
    {
        Id = registro.Id,
        EntregaId = registro.EntregaId,
        Latitud = registro.Latitud,
        Longitud = registro.Longitud,
        Descripcion = registro.Descripcion,
        FechaHora = registro.FechaHora
    };
}
