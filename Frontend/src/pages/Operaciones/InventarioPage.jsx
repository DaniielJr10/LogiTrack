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

const crearProductoBase = () => ({
  codigo: '',
  nombre: '',
  descripcion: '',
  unidadMedida: 'Unidad',
  precioUnitario: '',
  stockActual: '',
  stockMinimo: '',
  categoriaId: '',
  estaActivo: true,
});

const formatearMoneda = (valor) => {
  const numero = Number(valor ?? 0);
  return numero.toLocaleString('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
  });
};

export default function InventarioPage() {
  const { token } = useAuth();
  const [productos, setProductos] = useState([]);
  const [categorias, setCategorias] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState('');
  const [exito, setExito] = useState('');
  const [busqueda, setBusqueda] = useState('');
  const [filtroEstado, setFiltroEstado] = useState('Todos');
  const [mostrarFormulario, setMostrarFormulario] = useState(false);
  const [productoEditando, setProductoEditando] = useState(null);
  const [formulario, setFormulario] = useState(crearProductoBase());
  const [guardando, setGuardando] = useState(false);

  const cargarProductos = async () => {
    const respuesta = await apiFetch('/inventario/productos', { method: 'GET', token });
    setProductos(normalizarLista(respuesta));
  };

  const cargarCategorias = async () => {
    const respuesta = await apiFetch('/inventario/categorias', { method: 'GET', token });
    setCategorias(normalizarLista(respuesta));
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
        await Promise.all([cargarProductos(), cargarCategorias()]);
      } catch (err) {
        setError(err.message || 'No se pudo cargar el inventario.');
      } finally {
        setCargando(false);
      }
    };

    cargar();
  }, [token]);

  const abrirFormulario = (producto = null) => {
    setError('');
    setExito('');

    if (!producto) {
      setProductoEditando(null);
      setFormulario(crearProductoBase());
      setMostrarFormulario(true);
      return;
    }

    setProductoEditando(producto);
    setFormulario({
      codigo: producto.codigo ?? producto.Codigo ?? '',
      nombre: producto.nombre ?? producto.Nombre ?? '',
      descripcion: producto.descripcion ?? producto.Descripcion ?? '',
      unidadMedida: producto.unidadMedida ?? producto.UnidadMedida ?? 'Unidad',
      precioUnitario: producto.precioUnitario ?? producto.PrecioUnitario ?? '',
      stockActual: producto.stockActual ?? producto.StockActual ?? '',
      stockMinimo: producto.stockMinimo ?? producto.StockMinimo ?? '',
      categoriaId: producto.categoriaId ?? producto.CategoriaId ?? '',
      estaActivo: Boolean(producto.estaActivo ?? producto.EstaActivo ?? true),
    });
    setMostrarFormulario(true);
  };

  const cerrarFormulario = () => {
    setMostrarFormulario(false);
    setProductoEditando(null);
    setFormulario(crearProductoBase());
  };

  const manejarCambio = (evento) => {
    const { name, value, type, checked } = evento.target;
    setFormulario((actual) => ({
      ...actual,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };

  const manejarEnvio = async (evento) => {
    evento.preventDefault();
    setError('');
    setExito('');
    setGuardando(true);

    try {
      const payload = {
        codigo: formulario.codigo.trim(),
        nombre: formulario.nombre.trim(),
        descripcion: formulario.descripcion.trim(),
        unidadMedida: formulario.unidadMedida.trim() || 'Unidad',
        precioUnitario: Number(formulario.precioUnitario ?? 0),
        stockActual: Number(formulario.stockActual ?? 0),
        stockMinimo: Number(formulario.stockMinimo ?? 0),
        categoriaId: formulario.categoriaId,
        estaActivo: Boolean(formulario.estaActivo),
      };

      if (!payload.codigo || !payload.nombre || !payload.categoriaId) {
        throw new Error('Código, nombre y categoría son obligatorios.');
      }

      if (productoEditando) {
        await apiFetch(`/inventario/productos/${productoEditando.id ?? productoEditando.Id}`, {
          method: 'PUT',
          body: payload,
          token,
        });
      } else {
        await apiFetch('/inventario/productos', {
          method: 'POST',
          body: payload,
          token,
        });
      }

      setExito(productoEditando ? 'Producto actualizado correctamente.' : 'Producto creado correctamente.');
      cerrarFormulario();
      await cargarProductos();
    } catch (err) {
      setError(err.message || 'No se pudo guardar el producto.');
    } finally {
      setGuardando(false);
    }
  };

  const eliminarProducto = async (producto) => {
    const id = producto.id ?? producto.Id;
    if (!window.confirm(`¿Desea eliminar el producto ${producto.nombre ?? producto.Nombre ?? 'seleccionado'}?`)) {
      return;
    }

    try {
      setError('');
      await apiFetch(`/inventario/productos/${id}`, { method: 'DELETE', token });
      await cargarProductos();
      setExito('Producto eliminado correctamente.');
    } catch (err) {
      setError(err.message || 'No se pudo eliminar el producto.');
    }
  };

  const productosFiltrados = useMemo(() => {
    const termino = busqueda.trim().toLowerCase();

    return productos.filter((producto) => {
      const nombre = `${producto.nombre ?? producto.Nombre ?? ''}`.toLowerCase();
      const codigo = `${producto.codigo ?? producto.Codigo ?? ''}`.toLowerCase();
      const categoria = `${producto.categoriaNombre ?? producto.CategoriaNombre ?? ''}`.toLowerCase();
      const activo = Boolean(producto.estaActivo ?? producto.EstaActivo);
      const coincideBusqueda = !termino || nombre.includes(termino) || codigo.includes(termino) || categoria.includes(termino);
      const coincideEstado = filtroEstado === 'Todos' || (filtroEstado === 'Activos' ? activo : !activo);
      return coincideBusqueda && coincideEstado;
    });
  }, [productos, busqueda, filtroEstado]);

  const resumen = useMemo(() => ({
    total: productos.length,
    activos: productos.filter((producto) => Boolean(producto.estaActivo ?? producto.EstaActivo)).length,
    conBajoStock: productos.filter((producto) => Number(producto.stockActual ?? producto.StockActual ?? 0) <= 10).length,
  }), [productos]);

  return (
    <div className="page-shell">
      <div className="page-card">
        <div className="page-card__header">
          <div>
            <p className="eyebrow">Operación</p>
            <h2>Inventario</h2>
          </div>
          <div className="page-card__actions">
            <button type="button" className="primary-button" onClick={() => abrirFormulario()}>
              + Nuevo producto
            </button>
          </div>
        </div>

        <div className="module-metrics">
          <div className="module-metric module-metric--blue">
            <span>Total</span>
            <strong>{resumen.total}</strong>
          </div>
          <div className="module-metric module-metric--green">
            <span>Activos</span>
            <strong>{resumen.activos}</strong>
          </div>
          <div className="module-metric module-metric--amber">
            <span>Bajo stock</span>
            <strong>{resumen.conBajoStock}</strong>
          </div>
        </div>

        <div className="module-toolbar">
          <label className="toolbar-field">
            <span>Buscar</span>
            <input
              type="text"
              value={busqueda}
              onChange={(evento) => setBusqueda(evento.target.value)}
              placeholder="Buscar producto o código"
            />
          </label>
          <label className="toolbar-field toolbar-field--compact">
            <span>Estado</span>
            <select className="select-input" value={filtroEstado} onChange={(evento) => setFiltroEstado(evento.target.value)}>
              <option>Todos</option>
              <option>Activos</option>
              <option>Inactivos</option>
            </select>
          </label>
        </div>

        {error && <div className="form-message form-message--error">{error}</div>}
        {exito && <div className="form-message form-message--success">{exito}</div>}

        {cargando ? (
          <div className="loading-state">Cargando inventario...</div>
        ) : productosFiltrados.length === 0 ? (
          <div className="empty-state">No hay productos para los filtros seleccionados.</div>
        ) : (
          <div className="table-wrap">
            <table className="table-admin">
              <thead>
                <tr>
                  <th>Producto</th>
                  <th>Código</th>
                  <th>Stock</th>
                  <th>Precio</th>
                  <th>Estado</th>
                  <th>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {productosFiltrados.map((producto) => (
                  <tr key={producto.id ?? producto.Id}>
                    <td>
                      <div className="user-cell">
                        <span className="user-avatar">{(producto.nombre ?? producto.Nombre ?? 'P').charAt(0).toUpperCase()}</span>
                        <div>
                          <strong>{producto.nombre ?? producto.Nombre ?? 'Sin nombre'}</strong>
                          <small>{producto.categoriaNombre ?? producto.CategoriaNombre ?? 'Sin categoría'}</small>
                        </div>
                      </div>
                    </td>
                    <td>{producto.codigo ?? producto.Codigo ?? '-'}</td>
                    <td>{producto.stockActual ?? producto.StockActual ?? 0}</td>
                    <td>{formatearMoneda(producto.precioUnitario ?? producto.PrecioUnitario ?? 0)}</td>
                    <td>
                      <span className={`status-pill ${producto.estaActivo ?? producto.EstaActivo ? 'is-active' : 'is-inactive'}`}>
                        {(producto.estaActivo ?? producto.EstaActivo) ? 'Activo' : 'Inactivo'}
                      </span>
                    </td>
                    <td className="table-actions-cell">
                      <button type="button" className="table-action table-action--secondary" onClick={() => abrirFormulario(producto)}>
                        Editar
                      </button>
                      <button type="button" className="table-action table-action--danger" onClick={() => eliminarProducto(producto)}>
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
                <p className="eyebrow">Inventario</p>
                <h3>{productoEditando ? 'Editar producto' : 'Nuevo producto'}</h3>
              </div>
              <button type="button" className="ghost-button modal-close-button" onClick={cerrarFormulario}>
                Cerrar
              </button>
            </div>

            <form className="modal-form" onSubmit={manejarEnvio}>
              <div className="field-row">
                <label className="field">
                  <span>Código</span>
                  <input name="codigo" value={formulario.codigo} onChange={manejarCambio} required />
                </label>
                <label className="field">
                  <span>Unidad</span>
                  <input name="unidadMedida" value={formulario.unidadMedida} onChange={manejarCambio} />
                </label>
              </div>

              <div className="field-row">
                <label className="field">
                  <span>Nombre</span>
                  <input name="nombre" value={formulario.nombre} onChange={manejarCambio} required />
                </label>
                <label className="field">
                  <span>Categoría</span>
                  <select name="categoriaId" value={formulario.categoriaId} onChange={manejarCambio} required>
                    <option value="">Seleccione...</option>
                    {categorias.map((categoria) => (
                      <option key={categoria.id ?? categoria.Id} value={categoria.id ?? categoria.Id}>
                        {categoria.nombre ?? categoria.Nombre}
                      </option>
                    ))}
                  </select>
                </label>
              </div>

              <label className="field">
                <span>Descripción</span>
                <input name="descripcion" value={formulario.descripcion} onChange={manejarCambio} />
              </label>

              <div className="field-row">
                <label className="field">
                  <span>Precio unitario</span>
                  <input type="number" step="0.01" min="0" name="precioUnitario" value={formulario.precioUnitario} onChange={manejarCambio} />
                </label>
                <label className="field">
                  <span>Stock mínimo</span>
                  <input type="number" min="0" name="stockMinimo" value={formulario.stockMinimo} onChange={manejarCambio} />
                </label>
              </div>

              <div className="field-row">
                <label className="field">
                  <span>Stock actual</span>
                  <input type="number" min="0" name="stockActual" value={formulario.stockActual} onChange={manejarCambio} />
                </label>
                <label className="field checkbox-field">
                  <span>Activo</span>
                  <input type="checkbox" name="estaActivo" checked={formulario.estaActivo} onChange={manejarCambio} />
                </label>
              </div>

              {error && <div className="form-message form-message--error">{error}</div>}
              {exito && <div className="form-message form-message--success">{exito}</div>}

              <div className="modal-actions">
                <button type="button" className="ghost-button" onClick={cerrarFormulario}>
                  Cancelar
                </button>
                <button type="submit" className="primary-button" disabled={guardando}>
                  {guardando ? 'Guardando...' : productoEditando ? 'Actualizar producto' : 'Crear producto'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
