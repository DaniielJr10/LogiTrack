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

const crearPedidoBase = () => ({
  clienteId: '',
  direccionEntrega: '',
  ciudadEntrega: '',
  costoEnvio: '0',
  observaciones: '',
  productoId: '',
  cantidad: '1',
  estado: 'Pendiente',
});

const formatearMoneda = (valor) => {
  const numero = Number(valor ?? 0);
  return numero.toLocaleString('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
  });
};

export default function PedidosPage() {
  const { token } = useAuth();
  const [pedidos, setPedidos] = useState([]);
  const [clientes, setClientes] = useState([]);
  const [productos, setProductos] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState('');
  const [exito, setExito] = useState('');
  const [busqueda, setBusqueda] = useState('');
  const [filtroEstado, setFiltroEstado] = useState('Todos');
  const [mostrarFormulario, setMostrarFormulario] = useState(false);
  const [pedidoEditando, setPedidoEditando] = useState(null);
  const [formulario, setFormulario] = useState(crearPedidoBase());
  const [guardando, setGuardando] = useState(false);

  const cargarPedidos = async () => {
    const respuesta = await apiFetch('/pedidos', { method: 'GET', token });
    setPedidos(normalizarLista(respuesta));
  };

  const cargarClientes = async () => {
    const respuesta = await apiFetch('/clientes', { method: 'GET', token });
    setClientes(normalizarLista(respuesta));
  };

  const cargarProductos = async () => {
    const respuesta = await apiFetch('/inventario/productos', { method: 'GET', token });
    setProductos(normalizarLista(respuesta));
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
        await Promise.all([cargarPedidos(), cargarClientes(), cargarProductos()]);
      } catch (err) {
        setError(err.message || 'No se pudieron cargar los pedidos.');
      } finally {
        setCargando(false);
      }
    };

    cargar();
  }, [token]);

  const abrirFormulario = (pedido = null) => {
    setError('');
    setExito('');

    if (!pedido) {
      setPedidoEditando(null);
      setFormulario(crearPedidoBase());
      setMostrarFormulario(true);
      return;
    }

    setPedidoEditando(pedido);
    setFormulario({
      clienteId: pedido.clienteId ?? pedido.ClienteId ?? '',
      direccionEntrega: pedido.direccionEntrega ?? pedido.DireccionEntrega ?? '',
      ciudadEntrega: pedido.ciudadEntrega ?? pedido.CiudadEntrega ?? '',
      costoEnvio: pedido.costoEnvio ?? pedido.CostoEnvio ?? '0',
      observaciones: pedido.observaciones ?? pedido.Observaciones ?? '',
      productoId: '',
      cantidad: '1',
      estado: pedido.estado ?? pedido.Estado ?? 'Pendiente',
    });
    setMostrarFormulario(true);
  };

  const cerrarFormulario = () => {
    setMostrarFormulario(false);
    setPedidoEditando(null);
    setFormulario(crearPedidoBase());
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
        clienteId: formulario.clienteId,
        direccionEntrega: formulario.direccionEntrega.trim(),
        ciudadEntrega: formulario.ciudadEntrega.trim(),
        costoEnvio: Number(formulario.costoEnvio || 0),
        observaciones: formulario.observaciones.trim(),
        detalles: [],
      };

      if (!payload.clienteId) {
        throw new Error('Debe seleccionar un cliente.');
      }

      if (!payload.direccionEntrega || !payload.ciudadEntrega) {
        throw new Error('Dirección y ciudad de entrega son obligatorias.');
      }

      if (!pedidoEditando) {
        if (!formulario.productoId) {
          throw new Error('Debe seleccionar un producto para el pedido.');
        }

        const cantidad = Number(formulario.cantidad || 0);
        if (!cantidad || cantidad < 1) {
          throw new Error('La cantidad del producto debe ser mayor que cero.');
        }

        payload.detalles = [{ productoId: formulario.productoId, cantidad }];
      }

      if (pedidoEditando) {
        await apiFetch(`/pedidos/${pedidoEditando.id ?? pedidoEditando.Id}`, {
          method: 'PUT',
          body: {
            direccionEntrega: payload.direccionEntrega,
            ciudadEntrega: payload.ciudadEntrega,
            costoEnvio: payload.costoEnvio,
            observaciones: payload.observaciones,
            estado: formulario.estado,
          },
          token,
        });
      } else {
        await apiFetch('/pedidos', {
          method: 'POST',
          body: payload,
          token,
        });
      }

      setExito(pedidoEditando ? 'Pedido actualizado correctamente.' : 'Pedido creado correctamente.');
      cerrarFormulario();
      await cargarPedidos();
    } catch (err) {
      setError(err.message || 'No se pudo guardar el pedido.');
    } finally {
      setGuardando(false);
    }
  };

  const eliminarPedido = async (pedido) => {
    const id = pedido.id ?? pedido.Id;
    if (!window.confirm(`¿Desea eliminar el pedido ${pedido.numeroPedido ?? pedido.NumeroPedido ?? 'seleccionado'}?`)) {
      return;
    }

    try {
      setError('');
      await apiFetch(`/pedidos/${id}`, { method: 'DELETE', token });
      await cargarPedidos();
      setExito('Pedido eliminado correctamente.');
    } catch (err) {
      setError(err.message || 'No se pudo eliminar el pedido.');
    }
  };

  const pedidosFiltrados = useMemo(() => {
    const termino = busqueda.trim().toLowerCase();

    return pedidos.filter((pedido) => {
      const numero = `${pedido.numeroPedido ?? pedido.NumeroPedido ?? ''}`.toLowerCase();
      const cliente = `${pedido.clienteNombre ?? pedido.ClienteNombre ?? pedido.clienteId ?? pedido.ClienteId ?? ''}`.toLowerCase();
      const ciudad = `${pedido.ciudadEntrega ?? pedido.CiudadEntrega ?? ''}`.toLowerCase();
      const estado = `${pedido.estado ?? pedido.Estado ?? 'Pendiente'}`.toLowerCase();
      const coincideBusqueda = !termino || numero.includes(termino) || cliente.includes(termino) || ciudad.includes(termino);
      const coincideEstado = filtroEstado === 'Todos' || estado.includes(filtroEstado.toLowerCase());
      return coincideBusqueda && coincideEstado;
    });
  }, [pedidos, busqueda, filtroEstado]);

  const resumen = useMemo(() => ({
    total: pedidos.length,
    entregados: pedidos.filter((pedido) => `${pedido.estado ?? pedido.Estado ?? ''}`.toLowerCase().includes('entregado')).length,
    pendientes: pedidos.filter((pedido) => `${pedido.estado ?? pedido.Estado ?? 'Pendiente'}`.toLowerCase().includes('pendiente')).length,
  }), [pedidos]);

  return (
    <div className="page-shell">
      <div className="page-card">
        <div className="page-card__header">
          <div>
            <p className="eyebrow">Comercial</p>
            <h2>Pedidos</h2>
          </div>
          <div className="page-card__actions">
            <button type="button" className="primary-button" onClick={() => abrirFormulario()}>
              + Nuevo pedido
            </button>
          </div>
        </div>

        <div className="module-metrics">
          <div className="module-metric module-metric--blue">
            <span>Total</span>
            <strong>{resumen.total}</strong>
          </div>
          <div className="module-metric module-metric--green">
            <span>Entregados</span>
            <strong>{resumen.entregados}</strong>
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
              placeholder="Buscar pedido o cliente"
            />
          </label>
          <label className="toolbar-field toolbar-field--compact">
            <span>Estado</span>
            <select className="select-input" value={filtroEstado} onChange={(evento) => setFiltroEstado(evento.target.value)}>
              <option>Todos</option>
              <option>Pendiente</option>
              <option>En tránsito</option>
              <option>Entregado</option>
            </select>
          </label>
        </div>

        {error && <div className="form-message form-message--error">{error}</div>}
        {exito && <div className="form-message form-message--success">{exito}</div>}

        {cargando ? (
          <div className="loading-state">Cargando pedidos...</div>
        ) : pedidosFiltrados.length === 0 ? (
          <div className="empty-state">No hay pedidos para los filtros seleccionados.</div>
        ) : (
          <div className="table-wrap">
            <table className="table-admin">
              <thead>
                <tr>
                  <th>Pedido</th>
                  <th>Cliente</th>
                  <th>Estado</th>
                  <th>Total</th>
                  <th>Entrega</th>
                  <th>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {pedidosFiltrados.map((pedido) => (
                  <tr key={pedido.id ?? pedido.Id}>
                    <td>
                      <div className="user-cell">
                        <span className="user-avatar">#{(pedido.numeroPedido ?? pedido.NumeroPedido ?? 'N').slice(-2)}</span>
                        <div>
                          <strong>{pedido.numeroPedido ?? pedido.NumeroPedido ?? 'Sin número'}</strong>
                          <small>{(pedido.fechaPedido ?? pedido.FechaPedido)
                            ? new Date(pedido.fechaPedido ?? pedido.FechaPedido).toLocaleDateString('es-CO')
                            : 'Sin fecha'}</small>
                        </div>
                      </div>
                    </td>
                    <td>{pedido.clienteNombre ?? pedido.ClienteNombre ?? pedido.clienteId ?? pedido.ClienteId}</td>
                    <td>
                      <span className={`status-pill ${(pedido.estado ?? pedido.Estado) === 'Entregado' || (pedido.estado ?? pedido.Estado) === 'Entregada' ? 'is-active' : 'is-inactive'}`}>
                        {pedido.estado ?? pedido.Estado ?? 'Pendiente'}
                      </span>
                    </td>
                    <td>{formatearMoneda(pedido.total ?? pedido.Total ?? 0)}</td>
                    <td>{pedido.ciudadEntrega ?? pedido.CiudadEntrega ?? '-'}</td>
                    <td className="table-actions-cell">
                      <button type="button" className="table-action table-action--secondary" onClick={() => abrirFormulario(pedido)}>
                        Editar
                      </button>
                      <button type="button" className="table-action table-action--danger" onClick={() => eliminarPedido(pedido)}>
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
                <p className="eyebrow">Comercial</p>
                <h3>{pedidoEditando ? 'Editar pedido' : 'Nuevo pedido'}</h3>
              </div>
              <button type="button" className="ghost-button modal-close-button" onClick={cerrarFormulario}>
                Cerrar
              </button>
            </div>

            <form className="modal-form" onSubmit={manejarEnvio}>
              <div className="field-row">
                <label className="field">
                  <span>Cliente</span>
                  <select name="clienteId" value={formulario.clienteId} onChange={manejarCambio} required={!pedidoEditando}>
                    <option value="">Seleccione...</option>
                    {clientes.map((cliente) => (
                      <option key={cliente.id ?? cliente.Id} value={cliente.id ?? cliente.Id}>
                        {cliente.nombreCompleto ?? cliente.NombreCompleto}
                      </option>
                    ))}
                  </select>
                </label>
                <label className="field">
                  <span>Estado</span>
                  <select name="estado" value={formulario.estado} onChange={manejarCambio}>
                    <option>Pendiente</option>
                    <option>En tránsito</option>
                    <option>Entregado</option>
                  </select>
                </label>
              </div>

              {!pedidoEditando && (
                <div className="field-row">
                  <label className="field">
                    <span>Producto</span>
                    <select name="productoId" value={formulario.productoId} onChange={manejarCambio} required>
                      <option value="">Seleccione...</option>
                      {productos.map((producto) => (
                        <option key={producto.id ?? producto.Id} value={producto.id ?? producto.Id}>
                          {producto.nombre ?? producto.Nombre}
                        </option>
                      ))}
                    </select>
                  </label>
                  <label className="field">
                    <span>Cantidad</span>
                    <input type="number" min="1" name="cantidad" value={formulario.cantidad} onChange={manejarCambio} required={!pedidoEditando} />
                  </label>
                </div>
              )}

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
                  <span>Costo envío</span>
                  <input type="number" min="0" step="0.01" name="costoEnvio" value={formulario.costoEnvio} onChange={manejarCambio} />
                </label>
              </div>

              <label className="field">
                <span>Observaciones</span>
                <input name="observaciones" value={formulario.observaciones} onChange={manejarCambio} />
              </label>

              {error && <div className="form-message form-message--error">{error}</div>}
              {exito && <div className="form-message form-message--success">{exito}</div>}

              <div className="modal-actions">
                <button type="button" className="ghost-button" onClick={cerrarFormulario}>
                  Cancelar
                </button>
                <button type="submit" className="primary-button" disabled={guardando}>
                  {guardando ? 'Guardando...' : pedidoEditando ? 'Actualizar pedido' : 'Crear pedido'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
