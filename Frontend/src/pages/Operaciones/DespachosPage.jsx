import { useEffect, useMemo, useState } from 'react';
import { useAuth } from '../../contextos/AuthContext';
import { apiFetch } from '../../servicios/api';

const normalizarLista = (valor) => {
  if (Array.isArray(valor)) {
    return valor;
  }

  const datos = valor?.datos ?? valor?.data ?? [];
  return Array.isArray(datos) ? datos : [];
};

const crearDespachoBase = () => ({
  rutaId: '',
  vehiculoId: '',
  conductorId: '',
  estado: 'Programado',
  fechaProgramada: new Date().toISOString().slice(0, 10),
  observaciones: '',
});

const mapearEstadoDespacho = (valor) => {
  const estados = {
    Programado: 'Programado',
    'En ruta': 'EnRuta',
    EnRuta: 'EnRuta',
    Entregado: 'Entregado',
    Cancelado: 'Cancelado',
  };

  return estados[valor] ?? 'Programado';
};

const formatearFecha = (valor) => {
  if (!valor) return 'Sin fecha';
  const fecha = new Date(valor);
  if (Number.isNaN(fecha.getTime())) return 'Sin fecha';
  return fecha.toLocaleDateString('es-CO');
};

export default function DespachosPage() {
  const { token } = useAuth();
  const [despachos, setDespachos] = useState([]);
  const [rutas, setRutas] = useState([]);
  const [vehiculos, setVehiculos] = useState([]);
  const [conductores, setConductores] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState('');
  const [exito, setExito] = useState('');
  const [busqueda, setBusqueda] = useState('');
  const [filtroEstado, setFiltroEstado] = useState('Todos');
  const [mostrarFormulario, setMostrarFormulario] = useState(false);
  const [despachoEditando, setDespachoEditando] = useState(null);
  const [formulario, setFormulario] = useState(crearDespachoBase());
  const [guardando, setGuardando] = useState(false);

  const cargarDespachos = async () => {
    const respuesta = await apiFetch('/logistica/despachos', { method: 'GET', token });
    setDespachos(normalizarLista(respuesta));
  };

  const cargarRutas = async () => {
    const respuesta = await apiFetch('/logistica/rutas', { method: 'GET', token });
    setRutas(normalizarLista(respuesta));
  };

  const cargarVehiculos = async () => {
    const respuesta = await apiFetch('/logistica/vehiculos', { method: 'GET', token });
    setVehiculos(normalizarLista(respuesta));
  };

  const cargarConductores = async () => {
    const respuesta = await apiFetch('/logistica/conductores', { method: 'GET', token });
    setConductores(normalizarLista(respuesta));
  };

  useEffect(() => {
    if (!token) {
      setCargando(false);
      return;
    }

    const cargar = async () => {
      setCargando(true);
      setError('');
      try {
        await Promise.all([cargarDespachos(), cargarRutas(), cargarVehiculos(), cargarConductores()]);
      } catch (err) {
        setError(err.message || 'No se pudieron cargar los despachos.');
      } finally {
        setCargando(false);
      }
    };

    cargar();
  }, [token]);

  const abrirFormulario = (despacho = null) => {
    setError('');
    setExito('');

    if (!despacho) {
      setDespachoEditando(null);
      setFormulario(crearDespachoBase());
      setMostrarFormulario(true);
      return;
    }

    setDespachoEditando(despacho);
    setFormulario({
      rutaId: despacho.rutaId ?? despacho.RutaId ?? '',
      vehiculoId: despacho.vehiculoId ?? despacho.VehiculoId ?? '',
      conductorId: despacho.conductorId ?? despacho.ConductorId ?? '',
      estado: despacho.estado ?? despacho.Estado ?? 'Programado',
      fechaProgramada: despacho.fechaProgramada ?? despacho.FechaProgramada ? new Date(despacho.fechaProgramada ?? despacho.FechaProgramada).toISOString().slice(0, 10) : new Date().toISOString().slice(0, 10),
      observaciones: despacho.observaciones ?? despacho.Observaciones ?? '',
    });
    setMostrarFormulario(true);
  };

  const cerrarFormulario = () => {
    setMostrarFormulario(false);
    setDespachoEditando(null);
    setFormulario(crearDespachoBase());
  };

  const manejarCambio = (evento) => {
    const { name, value } = evento.target;
    setFormulario((actual) => ({ ...actual, [name]: value }));
  };

  const manejarEnvio = async (evento) => {
    evento.preventDefault();
    setError('');
    setExito('');
    setGuardando(true);

    try {
      const payload = {
        rutaId: formulario.rutaId,
        vehiculoId: formulario.vehiculoId || null,
        conductorId: formulario.conductorId || null,
        estado: mapearEstadoDespacho(formulario.estado),
        fechaProgramada: formulario.fechaProgramada ? new Date(`${formulario.fechaProgramada}T00:00:00`).toISOString() : new Date().toISOString(),
        observaciones: formulario.observaciones.trim(),
      };

      if (!payload.rutaId) {
        throw new Error('Debe seleccionar una ruta.');
      }

      if (despachoEditando) {
        await apiFetch(`/logistica/despachos/${despachoEditando.id ?? despachoEditando.Id}`, {
          method: 'PUT',
          body: payload,
          token,
        });
      } else {
        await apiFetch('/logistica/despachos', {
          method: 'POST',
          body: payload,
          token,
        });
      }

      setExito(despachoEditando ? 'Despacho actualizado correctamente.' : 'Despacho creado correctamente.');
      cerrarFormulario();
      await cargarDespachos();
    } catch (err) {
      setError(err.message || 'No se pudo guardar el despacho.');
    } finally {
      setGuardando(false);
    }
  };

  const eliminarDespacho = async (despacho) => {
    const id = despacho.id ?? despacho.Id;
    if (!window.confirm('¿Desea eliminar este despacho?')) {
      return;
    }

    try {
      setError('');
      await apiFetch(`/logistica/despachos/${id}`, { method: 'DELETE', token });
      await cargarDespachos();
      setExito('Despacho eliminado correctamente.');
    } catch (err) {
      setError(err.message || 'No se pudo eliminar el despacho.');
    }
  };

  const despachosFiltrados = useMemo(() => {
    const termino = busqueda.trim().toLowerCase();

    return despachos.filter((despacho) => {
      const ruta = `${despacho.rutaNombre ?? despacho.RutaNombre ?? despacho.rutaId ?? despacho.RutaId ?? ''}`.toLowerCase();
      const vehiculo = `${despacho.vehiculoPlaca ?? despacho.VehiculoPlaca ?? despacho.vehiculoId ?? despacho.VehiculoId ?? ''}`.toLowerCase();
      const conductor = `${despacho.conductorNombre ?? despacho.ConductorNombre ?? despacho.conductorId ?? despacho.ConductorId ?? ''}`.toLowerCase();
      const estado = `${despacho.estado ?? despacho.Estado ?? 'Programado'}`.toLowerCase();
      const coincideBusqueda = !termino || ruta.includes(termino) || vehiculo.includes(termino) || conductor.includes(termino);
      const coincideEstado = filtroEstado === 'Todos' || estado.includes(filtroEstado.toLowerCase()) || (filtroEstado === 'En ruta' && estado.includes('ruta'));
      return coincideBusqueda && coincideEstado;
    });
  }, [despachos, busqueda, filtroEstado]);

  const resumen = useMemo(() => ({
    total: despachos.length,
    programados: despachos.filter((despacho) => `${despacho.estado ?? despacho.Estado ?? ''}`.toLowerCase().includes('programado')).length,
    enRuta: despachos.filter((despacho) => `${despacho.estado ?? despacho.Estado ?? ''}`.toLowerCase().includes('ruta')).length,
  }), [despachos]);

  return (
    <div className="page-shell">
      <div className="page-card">
        <div className="page-card__header">
          <div>
            <p className="eyebrow">Logística</p>
            <h2>Despachos</h2>
          </div>
          <div className="page-card__actions">
            <button type="button" className="primary-button" onClick={() => abrirFormulario()}>
              + Nuevo despacho
            </button>
          </div>
        </div>

        <div className="module-metrics">
          <div className="module-metric module-metric--blue">
            <span>Total</span>
            <strong>{resumen.total}</strong>
          </div>
          <div className="module-metric module-metric--green">
            <span>Programados</span>
            <strong>{resumen.programados}</strong>
          </div>
          <div className="module-metric module-metric--amber">
            <span>En ruta</span>
            <strong>{resumen.enRuta}</strong>
          </div>
        </div>

        <div className="module-toolbar">
          <label className="toolbar-field">
            <span>Buscar</span>
            <input
              type="text"
              value={busqueda}
              onChange={(evento) => setBusqueda(evento.target.value)}
              placeholder="Buscar ruta, vehículo o conductor"
            />
          </label>
          <label className="toolbar-field toolbar-field--compact">
            <span>Estado</span>
            <select className="select-input" value={filtroEstado} onChange={(evento) => setFiltroEstado(evento.target.value)}>
              <option>Todos</option>
              <option>Programado</option>
              <option>En ruta</option>
              <option>Entregado</option>
              <option>Cancelado</option>
            </select>
          </label>
        </div>

        {error && <div className="form-message form-message--error">{error}</div>}
        {exito && <div className="form-message form-message--success">{exito}</div>}

        {cargando ? (
          <div className="loading-state">Cargando despachos...</div>
        ) : despachosFiltrados.length === 0 ? (
          <div className="empty-state">No hay despachos para los filtros seleccionados.</div>
        ) : (
          <div className="table-wrap">
            <table className="table-admin">
              <thead>
                <tr>
                  <th>Ruta</th>
                  <th>Vehículo</th>
                  <th>Conductor</th>
                  <th>Estado</th>
                  <th>Programado</th>
                  <th>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {despachosFiltrados.map((despacho) => (
                  <tr key={despacho.id ?? despacho.Id}>
                    <td>{despacho.rutaNombre ?? despacho.RutaNombre ?? despacho.rutaId ?? despacho.RutaId ?? '-'}</td>
                    <td>{despacho.vehiculoPlaca ?? despacho.VehiculoPlaca ?? despacho.vehiculoId ?? despacho.VehiculoId ?? '-'}</td>
                    <td>{despacho.conductorNombre ?? despacho.ConductorNombre ?? despacho.conductorId ?? despacho.ConductorId ?? '-'}</td>
                    <td>
                      <span className={`status-pill ${((despacho.estado ?? despacho.Estado) === 'Entregado' || (despacho.estado ?? despacho.Estado) === 'Entregada') ? 'is-active' : 'is-inactive'}`}>
                        {despacho.estado ?? despacho.Estado ?? 'Programado'}
                      </span>
                    </td>
                    <td>{formatearFecha(despacho.fechaProgramada ?? despacho.FechaProgramada)}</td>
                    <td className="table-actions-cell">
                      <button type="button" className="table-action table-action--secondary" onClick={() => abrirFormulario(despacho)}>
                        Editar
                      </button>
                      <button type="button" className="table-action table-action--danger" onClick={() => eliminarDespacho(despacho)}>
                        Eliminar
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {mostrarFormulario && (
        <div className="modal-backdrop" onClick={cerrarFormulario}>
          <div className="modal-card" onClick={(evento) => evento.stopPropagation()}>
            <div className="modal-header">
              <div>
                <p className="eyebrow">Logística</p>
                <h3>{despachoEditando ? 'Editar despacho' : 'Nuevo despacho'}</h3>
              </div>
              <button type="button" className="ghost-button modal-close-button" onClick={cerrarFormulario}>
                Cerrar
              </button>
            </div>

            <form className="modal-form" onSubmit={manejarEnvio}>
              <div className="field-row">
                <label className="field">
                  <span>Ruta</span>
                  <select name="rutaId" value={formulario.rutaId} onChange={manejarCambio} required>
                    <option value="">Seleccione...</option>
                    {rutas.map((ruta) => (
                      <option key={ruta.id ?? ruta.Id} value={ruta.id ?? ruta.Id}>
                        {ruta.nombre ?? ruta.Nombre}
                      </option>
                    ))}
                  </select>
                </label>
                <label className="field">
                  <span>Estado</span>
                  <select name="estado" value={formulario.estado} onChange={manejarCambio}>
                    <option>Programado</option>
                    <option>En ruta</option>
                    <option>Entregado</option>
                    <option>Cancelado</option>
                  </select>
                </label>
              </div>

              <div className="field-row">
                <label className="field">
                  <span>Vehículo</span>
                  <select name="vehiculoId" value={formulario.vehiculoId} onChange={manejarCambio}>
                    <option value="">Seleccione...</option>
                    {vehiculos.map((vehiculo) => (
                      <option key={vehiculo.id ?? vehiculo.Id} value={vehiculo.id ?? vehiculo.Id}>
                        {vehiculo.placa ?? vehiculo.Placa}
                      </option>
                    ))}
                  </select>
                </label>
                <label className="field">
                  <span>Conductor</span>
                  <select name="conductorId" value={formulario.conductorId} onChange={manejarCambio}>
                    <option value="">Seleccione...</option>
                    {conductores.map((conductor) => (
                      <option key={conductor.id ?? conductor.Id} value={conductor.id ?? conductor.Id}>
                        {`${conductor.nombres ?? conductor.Nombres ?? ''} ${conductor.apellidos ?? conductor.Apellidos ?? ''}`.trim() || 'Conductor'}
                      </option>
                    ))}
                  </select>
                </label>
              </div>

              <div className="field-row">
                <label className="field">
                  <span>Fecha programada</span>
                  <input type="date" name="fechaProgramada" value={formulario.fechaProgramada} onChange={manejarCambio} />
                </label>
                <label className="field">
                  <span>Observaciones</span>
                  <input name="observaciones" value={formulario.observaciones} onChange={manejarCambio} />
                </label>
              </div>

              {error && <div className="form-message form-message--error">{error}</div>}
              {exito && <div className="form-message form-message--success">{exito}</div>}

              <div className="modal-actions">
                <button type="button" className="ghost-button" onClick={cerrarFormulario}>
                  Cancelar
                </button>
                <button type="submit" className="primary-button" disabled={guardando}>
                  {guardando ? 'Guardando...' : despachoEditando ? 'Actualizar despacho' : 'Crear despacho'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
