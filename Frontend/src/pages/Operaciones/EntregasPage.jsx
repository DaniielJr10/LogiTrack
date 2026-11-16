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

const crearEntregaBase = () => ({
  despachoId: '',
  pedidoId: '',
  direccionEntrega: '',
  ciudadEntrega: '',
  estado: 'Pendiente',
  fechaProgramada: new Date().toISOString().slice(0, 10),
  latitud: '0',
  longitud: '0',
});

const formatearFecha = (valor) => {
  if (!valor) return 'Sin fecha';
  const fecha = new Date(valor);
  if (Number.isNaN(fecha.getTime())) return 'Sin fecha';
  return fecha.toLocaleDateString('es-CO');
};

const mapearEstado = (valor) => {
  const estados = {
    Pendiente: 'Pendiente',
    'En tránsito': 'EnTransito',
    EnTransito: 'EnTransito',
    Entregada: 'Entregada',
    Fallida: 'Fallida',
    Reprogramada: 'Reprogramada',
  };

  return estados[valor] ?? 'Pendiente';
};

export default function EntregasPage() {
  const { token } = useAuth();
  const [entregas, setEntregas] = useState([]);
  const [despachos, setDespachos] = useState([]);
  const [pedidos, setPedidos] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState('');
  const [exito, setExito] = useState('');
  const [busqueda, setBusqueda] = useState('');
  const [filtroEstado, setFiltroEstado] = useState('Todos');
  const [mostrarFormulario, setMostrarFormulario] = useState(false);
  const [entregaEditando, setEntregaEditando] = useState(null);
  const [formulario, setFormulario] = useState(crearEntregaBase());
  const [guardando, setGuardando] = useState(false);

  const cargarEntregas = async () => {
    const respuesta = await apiFetch('/logistica/entregas', { method: 'GET', token });
    setEntregas(normalizarLista(respuesta));
  };

  const cargarDespachos = async () => {
    const respuesta = await apiFetch('/logistica/despachos', { method: 'GET', token });
    setDespachos(normalizarLista(respuesta));
  };

  const cargarPedidos = async () => {
    const respuesta = await apiFetch('/pedidos', { method: 'GET', token });
    setPedidos(normalizarLista(respuesta));
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
        await Promise.all([cargarEntregas(), cargarDespachos(), cargarPedidos()]);
      } catch (err) {
        setError(err.message || 'No se pudieron cargar las entregas.');
      } finally {
        setCargando(false);
      }
    };

    cargar();
  }, [token]);

  const abrirFormulario = (entrega = null) => {
    setError('');
    setExito('');

    if (!entrega) {
      setEntregaEditando(null);
      setFormulario(crearEntregaBase());
      setMostrarFormulario(true);
      return;
    }

    setEntregaEditando(entrega);
    setFormulario({
      despachoId: entrega.despachoId ?? entrega.DespachoId ?? '',
      pedidoId: entrega.pedidoId ?? entrega.PedidoId ?? '',
      direccionEntrega: entrega.direccionEntrega ?? entrega.DireccionEntrega ?? '',
      ciudadEntrega: entrega.ciudadEntrega ?? entrega.CiudadEntrega ?? '',
      estado: entrega.estado ?? entrega.Estado ?? 'Pendiente',
      fechaProgramada: entrega.fechaProgramada ?? entrega.FechaProgramada ? new Date(entrega.fechaProgramada ?? entrega.FechaProgramada).toISOString().slice(0, 10) : new Date().toISOString().slice(0, 10),
      latitud: `${entrega.latitud ?? entrega.Latitud ?? 0}`,
      longitud: `${entrega.longitud ?? entrega.Longitud ?? 0}`,
    });
    setMostrarFormulario(true);
  };

  const cerrarFormulario = () => {
    setMostrarFormulario(false);
    setEntregaEditando(null);
    setFormulario(crearEntregaBase());
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
        despachoId: formulario.despachoId,
        pedidoId: formulario.pedidoId,
        direccionEntrega: formulario.direccionEntrega.trim(),
        ciudadEntrega: formulario.ciudadEntrega.trim(),
        estado: mapearEstado(formulario.estado),
        fechaProgramada: formulario.fechaProgramada ? new Date(`${formulario.fechaProgramada}T00:00:00`).toISOString() : new Date().toISOString(),
        latitud: Number(formulario.latitud || 0),
        longitud: Number(formulario.longitud || 0),
      };

      if (!payload.despachoId || !payload.pedidoId) {
        throw new Error('Debe seleccionar un despacho y un pedido.');
      }

      if (!payload.direccionEntrega || !payload.ciudadEntrega) {
        throw new Error('La dirección y la ciudad de entrega son obligatorias.');
      }

      if (entregaEditando) {
        await apiFetch(`/logistica/entregas/${entregaEditando.id ?? entregaEditando.Id}`, {
          method: 'PUT',
          body: payload,
          token,
        });
      } else {
        await apiFetch('/logistica/entregas', {
          method: 'POST',
          body: payload,
          token,
        });
      }

      setExito(entregaEditando ? 'Entrega actualizada correctamente.' : 'Entrega creada correctamente.');
      cerrarFormulario();
      await cargarEntregas();
    } catch (err) {
      setError(err.message || 'No se pudo guardar la entrega.');
    } finally {
      setGuardando(false);
    }
  };

  const eliminarEntrega = async (entrega) => {
    const id = entrega.id ?? entrega.Id;
    if (!window.confirm('¿Desea eliminar esta entrega?')) {
      return;
    }

    try {
      setError('');
      await apiFetch(`/logistica/entregas/${id}`, { method: 'DELETE', token });
      await cargarEntregas();
      setExito('Entrega eliminada correctamente.');
    } catch (err) {
      setError(err.message || 'No se pudo eliminar la entrega.');
    }
  };

  const entregasFiltradas = useMemo(() => {
    const termino = busqueda.trim().toLowerCase();

    return entregas.filter((entrega) => {
      const direccion = `${entrega.direccionEntrega ?? entrega.DireccionEntrega ?? ''}`.toLowerCase();
      const ciudad = `${entrega.ciudadEntrega ?? entrega.CiudadEntrega ?? ''}`.toLowerCase();
      const despacho = `${entrega.despachoId ?? entrega.DespachoId ?? ''}`.toLowerCase();
      const estado = `${entrega.estado ?? entrega.Estado ?? 'Pendiente'}`.toLowerCase();
      const coincideBusqueda = !termino || direccion.includes(termino) || ciudad.includes(termino) || despacho.includes(termino);
      const coincideEstado = filtroEstado === 'Todos' || estado.includes(filtroEstado.toLowerCase()) || (filtroEstado === 'En tránsito' && estado.includes('transito'));
      return coincideBusqueda && coincideEstado;
    });
  }, [entregas, busqueda, filtroEstado]);

  const resumen = useMemo(() => ({
    total: entregas.length,
    entregadas: entregas.filter((entrega) => `${entrega.estado ?? entrega.Estado ?? ''}`.toLowerCase().includes('entregada')).length,
    pendientes: entregas.filter((entrega) => `${entrega.estado ?? entrega.Estado ?? 'Pendiente'}`.toLowerCase().includes('pendiente')).length,
  }), [entregas]);

  return (
    <div className="page-shell">
      <div className="page-card">
        <div className="page-card__header">
          <div>
            <p className="eyebrow">Operación</p>
            <h2>Entregas</h2>
          </div>
          <div className="page-card__actions">
            <button type="button" className="primary-button" onClick={() => abrirFormulario()}>
              + Nueva entrega
            </button>
          </div>
        </div>

        <div className="module-metrics">
          <div className="module-metric module-metric--blue">
            <span>Total</span>
            <strong>{resumen.total}</strong>
          </div>
          <div className="module-metric module-metric--green">
            <span>Entregadas</span>
            <strong>{resumen.entregadas}</strong>
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
              placeholder="Buscar dirección o ciudad"
            />
          </label>
          <label className="toolbar-field toolbar-field--compact">
            <span>Estado</span>
            <select className="select-input" value={filtroEstado} onChange={(evento) => setFiltroEstado(evento.target.value)}>
              <option>Todos</option>
              <option>Pendiente</option>
              <option>En tránsito</option>
              <option>Entregada</option>
              <option>Fallida</option>
              <option>Reprogramada</option>
            </select>
          </label>
        </div>

        {error && <div className="form-message form-message--error">{error}</div>}
        {exito && <div className="form-message form-message--success">{exito}</div>}

        {cargando ? (
          <div className="loading-state">Cargando entregas...</div>
        ) : entregasFiltradas.length === 0 ? (
          <div className="empty-state">No hay entregas para los filtros seleccionados.</div>
        ) : (
          <div className="table-wrap">
            <table className="table-admin">
              <thead>
                <tr>
                  <th>Entrega</th>
                  <th>Ciudad</th>
                  <th>Estado</th>
                  <th>Programada</th>
                  <th>Despacho</th>
                  <th>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {entregasFiltradas.map((entrega) => (
                  <tr key={entrega.id ?? entrega.Id}>
                    <td>{entrega.direccionEntrega ?? entrega.DireccionEntrega ?? 'Sin dirección'}</td>
                    <td>{entrega.ciudadEntrega ?? entrega.CiudadEntrega ?? '-'}</td>
                    <td>
                      <span className={`status-pill ${((entrega.estado ?? entrega.Estado) === 'Entregada' || (entrega.estado ?? entrega.Estado) === 'Entregado') ? 'is-active' : 'is-inactive'}`}>
                        {entrega.estado ?? entrega.Estado ?? 'Pendiente'}
                      </span>
                    </td>
                    <td>{formatearFecha(entrega.fechaProgramada ?? entrega.FechaProgramada)}</td>
                    <td>{entrega.despachoId ?? entrega.DespachoId ?? '-'}</td>
                    <td className="table-actions-cell">
                      <button type="button" className="table-action table-action--secondary" onClick={() => abrirFormulario(entrega)}>
                        Editar
                      </button>
                      <button type="button" className="table-action table-action--danger" onClick={() => eliminarEntrega(entrega)}>
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
                <p className="eyebrow">Operación</p>
                <h3>{entregaEditando ? 'Editar entrega' : 'Nueva entrega'}</h3>
              </div>
              <button type="button" className="ghost-button modal-close-button" onClick={cerrarFormulario}>
                Cerrar
              </button>
            </div>

            <form className="modal-form" onSubmit={manejarEnvio}>
              <div className="field-row">
                <label className="field">
                  <span>Despacho</span>
                  <select name="despachoId" value={formulario.despachoId} onChange={manejarCambio} required>
                    <option value="">Seleccione...</option>
                    {despachos.map((despacho) => (
                      <option key={despacho.id ?? despacho.Id} value={despacho.id ?? despacho.Id}>
                        {despacho.rutaNombre ?? despacho.RutaNombre ?? 'Despacho'}
                      </option>
                    ))}
                  </select>
                </label>
                <label className="field">
                  <span>Pedido</span>
                  <select name="pedidoId" value={formulario.pedidoId} onChange={manejarCambio} required>
                    <option value="">Seleccione...</option>
                    {pedidos.map((pedido) => (
                      <option key={pedido.id ?? pedido.Id} value={pedido.id ?? pedido.Id}>
                        {pedido.numeroPedido ?? pedido.NumeroPedido ?? 'Pedido sin número'}
                      </option>
                    ))}
                  </select>
                </label>
              </div>

              <label className="field">
                <span>Dirección de entrega</span>
                <input name="direccionEntrega" value={formulario.direccionEntrega} onChange={manejarCambio} required />
              </label>

              <div className="field-row">
                <label className="field">
                  <span>Ciudad</span>
                  <input name="ciudadEntrega" value={formulario.ciudadEntrega} onChange={manejarCambio} required />
                </label>
                <label className="field">
                  <span>Estado</span>
                  <select name="estado" value={formulario.estado} onChange={manejarCambio}>
                    <option>Pendiente</option>
                    <option>En tránsito</option>
                    <option>Entregada</option>
                    <option>Fallida</option>
                    <option>Reprogramada</option>
                  </select>
                </label>
              </div>

              <div className="field-row">
                <label className="field">
                  <span>Fecha programada</span>
                  <input type="date" name="fechaProgramada" value={formulario.fechaProgramada} onChange={manejarCambio} />
                </label>
                <label className="field">
                  <span>Latitud</span>
                  <input type="number" step="0.000001" name="latitud" value={formulario.latitud} onChange={manejarCambio} />
                </label>
                <label className="field">
                  <span>Longitud</span>
                  <input type="number" step="0.000001" name="longitud" value={formulario.longitud} onChange={manejarCambio} />
                </label>
              </div>

              {error && <div className="form-message form-message--error">{error}</div>}
              {exito && <div className="form-message form-message--success">{exito}</div>}

              <div className="modal-actions">
                <button type="button" className="ghost-button" onClick={cerrarFormulario}>
                  Cancelar
                </button>
                <button type="submit" className="primary-button" disabled={guardando}>
                  {guardando ? 'Guardando...' : entregaEditando ? 'Actualizar entrega' : 'Crear entrega'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
