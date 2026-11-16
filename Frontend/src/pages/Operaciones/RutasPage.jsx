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

const crearRutaBase = () => ({
  nombre: '',
  origen: '',
  destino: '',
  descripcion: '',
  estado: 'Pendiente',
});

const mapearEstadoRuta = (valor) => {
  const estados = {
    Pendiente: 'Pendiente',
    Asignada: 'Asignada',
    'En curso': 'EnCurso',
    EnCurso: 'EnCurso',
    Completada: 'Completada',
    Cancelada: 'Cancelada',
  };

  return estados[valor] ?? 'Pendiente';
};

const formatearFecha = (valor) => {
  if (!valor) return 'Sin fecha';
  const fecha = new Date(valor);
  if (Number.isNaN(fecha.getTime())) return 'Sin fecha';
  return fecha.toLocaleDateString('es-CO');
};

export default function RutasPage() {
  const { token } = useAuth();
  const [rutas, setRutas] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState('');
  const [exito, setExito] = useState('');
  const [busqueda, setBusqueda] = useState('');
  const [filtroEstado, setFiltroEstado] = useState('Todos');
  const [mostrarFormulario, setMostrarFormulario] = useState(false);
  const [rutaEditando, setRutaEditando] = useState(null);
  const [formulario, setFormulario] = useState(crearRutaBase());
  const [guardando, setGuardando] = useState(false);

  const cargarRutas = async () => {
    const respuesta = await apiFetch('/logistica/rutas', { method: 'GET', token });
    setRutas(normalizarLista(respuesta));
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
        await cargarRutas();
      } catch (err) {
        setError(err.message || 'No se pudieron cargar las rutas.');
      } finally {
        setCargando(false);
      }
    };

    cargar();
  }, [token]);

  const abrirFormulario = (ruta = null) => {
    setError('');
    setExito('');

    if (!ruta) {
      setRutaEditando(null);
      setFormulario(crearRutaBase());
      setMostrarFormulario(true);
      return;
    }

    setRutaEditando(ruta);
    setFormulario({
      nombre: ruta.nombre ?? ruta.Nombre ?? '',
      origen: ruta.origen ?? ruta.Origen ?? '',
      destino: ruta.destino ?? ruta.Destino ?? '',
      descripcion: ruta.descripcion ?? ruta.Descripcion ?? '',
      estado: ruta.estado ?? ruta.Estado ?? 'Pendiente',
    });
    setMostrarFormulario(true);
  };

  const cerrarFormulario = () => {
    setMostrarFormulario(false);
    setRutaEditando(null);
    setFormulario(crearRutaBase());
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
        nombre: formulario.nombre.trim(),
        origen: formulario.origen.trim(),
        destino: formulario.destino.trim(),
        descripcion: formulario.descripcion.trim(),
        estado: mapearEstadoRuta(formulario.estado),
      };

      if (!payload.nombre || !payload.origen || !payload.destino) {
        throw new Error('Nombre, origen y destino son obligatorios.');
      }

      if (rutaEditando) {
        await apiFetch(`/logistica/rutas/${rutaEditando.id ?? rutaEditando.Id}`, {
          method: 'PUT',
          body: payload,
          token,
        });
      } else {
        await apiFetch('/logistica/rutas', {
          method: 'POST',
          body: payload,
          token,
        });
      }

      setExito(rutaEditando ? 'Ruta actualizada correctamente.' : 'Ruta creada correctamente.');
      cerrarFormulario();
      await cargarRutas();
    } catch (err) {
      setError(err.message || 'No se pudo guardar la ruta.');
    } finally {
      setGuardando(false);
    }
  };

  const eliminarRuta = async (ruta) => {
    const id = ruta.id ?? ruta.Id;
    if (!window.confirm(`¿Desea eliminar la ruta ${ruta.nombre ?? ruta.Nombre ?? 'seleccionada'}?`)) {
      return;
    }

    try {
      setError('');
      await apiFetch(`/logistica/rutas/${id}`, { method: 'DELETE', token });
      await cargarRutas();
      setExito('Ruta eliminada correctamente.');
    } catch (err) {
      setError(err.message || 'No se pudo eliminar la ruta.');
    }
  };

  const rutasFiltradas = useMemo(() => {
    const termino = busqueda.trim().toLowerCase();

    return rutas.filter((ruta) => {
      const nombre = `${ruta.nombre ?? ruta.Nombre ?? ''}`.toLowerCase();
      const origen = `${ruta.origen ?? ruta.Origen ?? ''}`.toLowerCase();
      const destino = `${ruta.destino ?? ruta.Destino ?? ''}`.toLowerCase();
      const estado = `${ruta.estado ?? ruta.Estado ?? 'Pendiente'}`.toLowerCase();
      const coincideBusqueda = !termino || nombre.includes(termino) || origen.includes(termino) || destino.includes(termino);
      const coincideEstado = filtroEstado === 'Todos' || estado.includes(filtroEstado.toLowerCase()) || (filtroEstado === 'En curso' && estado.includes('curso'));
      return coincideBusqueda && coincideEstado;
    });
  }, [rutas, busqueda, filtroEstado]);

  const resumen = useMemo(() => ({
    total: rutas.length,
    activas: rutas.filter((ruta) => `${ruta.estado ?? ruta.Estado ?? ''}`.toLowerCase().includes('asignada') || `${ruta.estado ?? ruta.Estado ?? ''}`.toLowerCase().includes('curso')).length,
    pendientes: rutas.filter((ruta) => `${ruta.estado ?? ruta.Estado ?? 'Pendiente'}`.toLowerCase().includes('pendiente')).length,
  }), [rutas]);

  return (
    <div className="page-shell">
      <div className="page-card">
        <div className="page-card__header">
          <div>
            <p className="eyebrow">Logística</p>
            <h2>Rutas</h2>
          </div>
          <div className="page-card__actions">
            <button type="button" className="primary-button" onClick={() => abrirFormulario()}>
              + Nueva ruta
            </button>
          </div>
        </div>

        <div className="module-metrics">
          <div className="module-metric module-metric--blue">
            <span>Total</span>
            <strong>{resumen.total}</strong>
          </div>
          <div className="module-metric module-metric--green">
            <span>Activas</span>
            <strong>{resumen.activas}</strong>
          </div>
          <div className="module-metric module-metric--amber">
            <span>Pendientes</span>
            <strong>{resumen.pendientes}</strong>
          </div>
        </div>

        <div className="module-toolbar">
          <label className="toolbar-field">
            <span>Buscar</span>
            <input
              type="text"
              value={busqueda}
              onChange={(evento) => setBusqueda(evento.target.value)}
              placeholder="Buscar nombre, origen o destino"
            />
          </label>
          <label className="toolbar-field toolbar-field--compact">
            <span>Estado</span>
            <select className="select-input" value={filtroEstado} onChange={(evento) => setFiltroEstado(evento.target.value)}>
              <option>Todos</option>
              <option>Pendiente</option>
              <option>Asignada</option>
              <option>En curso</option>
              <option>Completada</option>
              <option>Cancelada</option>
            </select>
          </label>
        </div>

        {error && <div className="form-message form-message--error">{error}</div>}
        {exito && <div className="form-message form-message--success">{exito}</div>}

        {cargando ? (
          <div className="loading-state">Cargando rutas...</div>
        ) : rutasFiltradas.length === 0 ? (
          <div className="empty-state">No hay rutas para los filtros seleccionados.</div>
        ) : (
          <div className="table-wrap">
            <table className="table-admin">
              <thead>
                <tr>
                  <th>Ruta</th>
                  <th>Origen</th>
                  <th>Destino</th>
                  <th>Estado</th>
                  <th>Creación</th>
                  <th>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {rutasFiltradas.map((ruta) => (
                  <tr key={ruta.id ?? ruta.Id}>
                    <td>{ruta.nombre ?? ruta.Nombre ?? 'Sin nombre'}</td>
                    <td>{ruta.origen ?? ruta.Origen ?? '-'}</td>
                    <td>{ruta.destino ?? ruta.Destino ?? '-'}</td>
                    <td>
                      <span className={`status-pill ${((ruta.estado ?? ruta.Estado) === 'Completada' || (ruta.estado ?? ruta.Estado) === 'Completado') ? 'is-active' : 'is-inactive'}`}>
                        {ruta.estado ?? ruta.Estado ?? 'Pendiente'}
                      </span>
                    </td>
                    <td>{formatearFecha(ruta.fechaCreacion ?? ruta.FechaCreacion)}</td>
                    <td className="table-actions-cell">
                      <button type="button" className="table-action table-action--secondary" onClick={() => abrirFormulario(ruta)}>
                        Editar
                      </button>
                      <button type="button" className="table-action table-action--danger" onClick={() => eliminarRuta(ruta)}>
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
                <h3>{rutaEditando ? 'Editar ruta' : 'Nueva ruta'}</h3>
              </div>
              <button type="button" className="ghost-button modal-close-button" onClick={cerrarFormulario}>
                Cerrar
              </button>
            </div>

            <form className="modal-form" onSubmit={manejarEnvio}>
              <label className="field">
                <span>Nombre</span>
                <input name="nombre" value={formulario.nombre} onChange={manejarCambio} required />
              </label>

              <div className="field-row">
                <label className="field">
                  <span>Origen</span>
                  <input name="origen" value={formulario.origen} onChange={manejarCambio} required />
                </label>
                <label className="field">
                  <span>Destino</span>
                  <input name="destino" value={formulario.destino} onChange={manejarCambio} required />
                </label>
              </div>

              <label className="field">
                <span>Descripción</span>
                <input name="descripcion" value={formulario.descripcion} onChange={manejarCambio} />
              </label>

              <label className="field">
                <span>Estado</span>
                <select name="estado" value={formulario.estado} onChange={manejarCambio}>
                  <option>Pendiente</option>
                  <option>Asignada</option>
                  <option>En curso</option>
                  <option>Completada</option>
                  <option>Cancelada</option>
                </select>
              </label>

              {error && <div className="form-message form-message--error">{error}</div>}
              {exito && <div className="form-message form-message--success">{exito}</div>}

              <div className="modal-actions">
                <button type="button" className="ghost-button" onClick={cerrarFormulario}>
                  Cancelar
                </button>
                <button type="submit" className="primary-button" disabled={guardando}>
                  {guardando ? 'Guardando...' : rutaEditando ? 'Actualizar ruta' : 'Crear ruta'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
