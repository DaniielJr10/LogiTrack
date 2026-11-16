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

const crearVehiculoBase = () => ({
  placa: '',
  marca: '',
  modelo: '',
  tipo: 'Camión',
  anio: '',
  capacidadKg: '',
  estaDisponible: true,
});

export default function FlotaPage() {
  const { token } = useAuth();
  const [vehiculos, setVehiculos] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState('');
  const [exito, setExito] = useState('');
  const [busqueda, setBusqueda] = useState('');
  const [filtroEstado, setFiltroEstado] = useState('Todos');
  const [mostrarFormulario, setMostrarFormulario] = useState(false);
  const [vehiculoEditando, setVehiculoEditando] = useState(null);
  const [formulario, setFormulario] = useState(crearVehiculoBase());
  const [guardando, setGuardando] = useState(false);

  const cargarVehiculos = async () => {
    const respuesta = await apiFetch('/logistica/vehiculos', { method: 'GET', token });
    setVehiculos(normalizarLista(respuesta));
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
        await cargarVehiculos();
      } catch (err) {
        setError(err.message || 'No se pudo cargar la flota.');
      } finally {
        setCargando(false);
      }
    };

    cargar();
  }, [token]);

  const abrirFormulario = (vehiculo = null) => {
    setError('');
    setExito('');

    if (!vehiculo) {
      setVehiculoEditando(null);
      setFormulario(crearVehiculoBase());
      setMostrarFormulario(true);
      return;
    }

    setVehiculoEditando(vehiculo);
    setFormulario({
      placa: vehiculo.placa ?? vehiculo.Placa ?? '',
      marca: vehiculo.marca ?? vehiculo.Marca ?? '',
      modelo: vehiculo.modelo ?? vehiculo.Modelo ?? '',
      tipo: vehiculo.tipo ?? vehiculo.Tipo ?? 'Camión',
      anio: vehiculo.anio ?? vehiculo.Anio ?? '',
      capacidadKg: vehiculo.capacidadKg ?? vehiculo.CapacidadKg ?? '',
      estaDisponible: Boolean(vehiculo.estaDisponible ?? vehiculo.EstaDisponible ?? true),
    });
    setMostrarFormulario(true);
  };

  const cerrarFormulario = () => {
    setMostrarFormulario(false);
    setVehiculoEditando(null);
    setFormulario(crearVehiculoBase());
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
        placa: formulario.placa.trim(),
        marca: formulario.marca.trim(),
        modelo: formulario.modelo.trim(),
        tipo: formulario.tipo.trim() || 'Camión',
        anio: Number(formulario.anio || 0),
        capacidadKg: Number(formulario.capacidadKg || 0),
        estaDisponible: Boolean(formulario.estaDisponible),
      };

      if (!payload.placa || !payload.marca || !payload.modelo) {
        throw new Error('Placa, marca y modelo son obligatorios.');
      }

      if (vehiculoEditando) {
        await apiFetch(`/logistica/vehiculos/${vehiculoEditando.id ?? vehiculoEditando.Id}`, {
          method: 'PUT',
          body: payload,
          token,
        });
      } else {
        await apiFetch('/logistica/vehiculos', {
          method: 'POST',
          body: payload,
          token,
        });
      }

      setExito(vehiculoEditando ? 'Vehículo actualizado correctamente.' : 'Vehículo creado correctamente.');
      cerrarFormulario();
      await cargarVehiculos();
    } catch (err) {
      setError(err.message || 'No se pudo guardar el vehículo.');
    } finally {
      setGuardando(false);
    }
  };

  const eliminarVehiculo = async (vehiculo) => {
    const id = vehiculo.id ?? vehiculo.Id;
    if (!window.confirm(`¿Desea eliminar el vehículo ${vehiculo.placa ?? vehiculo.Placa ?? 'seleccionado'}?`)) {
      return;
    }

    try {
      setError('');
      await apiFetch(`/logistica/vehiculos/${id}`, { method: 'DELETE', token });
      await cargarVehiculos();
      setExito('Vehículo eliminado correctamente.');
    } catch (err) {
      setError(err.message || 'No se pudo eliminar el vehículo.');
    }
  };

  const vehiculosFiltrados = useMemo(() => {
    const termino = busqueda.trim().toLowerCase();

    return vehiculos.filter((vehiculo) => {
      const marca = `${vehiculo.marca ?? vehiculo.Marca ?? ''}`.toLowerCase();
      const modelo = `${vehiculo.modelo ?? vehiculo.Modelo ?? ''}`.toLowerCase();
      const placa = `${vehiculo.placa ?? vehiculo.Placa ?? ''}`.toLowerCase();
      const tipo = `${vehiculo.tipo ?? vehiculo.Tipo ?? ''}`.toLowerCase();
      const disponible = Boolean(vehiculo.estaDisponible ?? vehiculo.EstaDisponible);
      const coincideBusqueda = !termino || marca.includes(termino) || modelo.includes(termino) || placa.includes(termino) || tipo.includes(termino);
      const coincideEstado = filtroEstado === 'Todos' || (filtroEstado === 'Disponibles' ? disponible : !disponible);
      return coincideBusqueda && coincideEstado;
    });
  }, [vehiculos, busqueda, filtroEstado]);

  const resumen = useMemo(() => ({
    total: vehiculos.length,
    disponibles: vehiculos.filter((vehiculo) => Boolean(vehiculo.estaDisponible ?? vehiculo.EstaDisponible)).length,
    noDisponibles: vehiculos.filter((vehiculo) => !Boolean(vehiculo.estaDisponible ?? vehiculo.EstaDisponible)).length,
  }), [vehiculos]);

  return (
    <div className="page-shell">
      <div className="page-card">
        <div className="page-card__header">
          <div>
            <p className="eyebrow">Logística</p>
            <h2>Flota</h2>
          </div>
          <div className="page-card__actions">
            <button type="button" className="primary-button" onClick={() => abrirFormulario()}>
              + Nuevo vehículo
            </button>
          </div>
        </div>

        <div className="module-metrics">
          <div className="module-metric module-metric--blue">
            <span>Total</span>
            <strong>{resumen.total}</strong>
          </div>
          <div className="module-metric module-metric--green">
            <span>Disponibles</span>
            <strong>{resumen.disponibles}</strong>
          </div>
          <div className="module-metric module-metric--amber">
            <span>En reserva</span>
            <strong>{resumen.noDisponibles}</strong>
          </div>
        </div>

        <div className="module-toolbar">
          <label className="toolbar-field">
            <span>Buscar</span>
            <input
              type="text"
              value={busqueda}
              onChange={(evento) => setBusqueda(evento.target.value)}
              placeholder="Buscar placa o marca"
            />
          </label>
          <label className="toolbar-field toolbar-field--compact">
            <span>Estado</span>
            <select className="select-input" value={filtroEstado} onChange={(evento) => setFiltroEstado(evento.target.value)}>
              <option>Todos</option>
              <option>Disponibles</option>
              <option>No disponibles</option>
            </select>
          </label>
        </div>

        {error && <div className="form-message form-message--error">{error}</div>}
        {exito && <div className="form-message form-message--success">{exito}</div>}

        {cargando ? (
          <div className="loading-state">Cargando flota...</div>
        ) : vehiculosFiltrados.length === 0 ? (
          <div className="empty-state">No hay vehículos para los filtros seleccionados.</div>
        ) : (
          <div className="table-wrap">
            <table className="table-admin">
              <thead>
                <tr>
                  <th>Vehículo</th>
                  <th>Placa</th>
                  <th>Tipo</th>
                  <th>Capacidad</th>
                  <th>Estado</th>
                  <th>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {vehiculosFiltrados.map((vehiculo) => (
                  <tr key={vehiculo.id ?? vehiculo.Id}>
                    <td>
                      <div className="user-cell">
                        <span className="user-avatar">{(vehiculo.marca ?? vehiculo.Marca ?? 'V').charAt(0).toUpperCase()}</span>
                        <div>
                          <strong>{`${vehiculo.marca ?? vehiculo.Marca ?? ''} ${vehiculo.modelo ?? vehiculo.Modelo ?? ''}`.trim() || 'Sin marca'}</strong>
                          <small>{vehiculo.tipo ?? vehiculo.Tipo ?? 'Sin tipo'}</small>
                        </div>
                      </div>
                    </td>
                    <td>{vehiculo.placa ?? vehiculo.Placa ?? '-'}</td>
                    <td>{vehiculo.tipo ?? vehiculo.Tipo ?? '-'}</td>
                    <td>{`${vehiculo.capacidadKg ?? vehiculo.CapacidadKg ?? 0} kg`}</td>
                    <td>
                      <span className={`status-pill ${vehiculo.estaDisponible ?? vehiculo.EstaDisponible ? 'is-active' : 'is-inactive'}`}>
                        {(vehiculo.estaDisponible ?? vehiculo.EstaDisponible) ? 'Disponible' : 'No disponible'}
                      </span>
                    </td>
                    <td className="table-actions-cell">
                      <button type="button" className="table-action table-action--secondary" onClick={() => abrirFormulario(vehiculo)}>
                        Editar
                      </button>
                      <button type="button" className="table-action table-action--danger" onClick={() => eliminarVehiculo(vehiculo)}>
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
                <h3>{vehiculoEditando ? 'Editar vehículo' : 'Nuevo vehículo'}</h3>
              </div>
              <button type="button" className="ghost-button modal-close-button" onClick={cerrarFormulario}>
                Cerrar
              </button>
            </div>

            <form className="modal-form" onSubmit={manejarEnvio}>
              <div className="field-row">
                <label className="field">
                  <span>Placa</span>
                  <input name="placa" value={formulario.placa} onChange={manejarCambio} required />
                </label>
                <label className="field">
                  <span>Tipo</span>
                  <input name="tipo" value={formulario.tipo} onChange={manejarCambio} />
                </label>
              </div>

              <div className="field-row">
                <label className="field">
                  <span>Marca</span>
                  <input name="marca" value={formulario.marca} onChange={manejarCambio} required />
                </label>
                <label className="field">
                  <span>Modelo</span>
                  <input name="modelo" value={formulario.modelo} onChange={manejarCambio} required />
                </label>
              </div>

              <div className="field-row">
                <label className="field">
                  <span>Año</span>
                  <input type="number" min="1900" max="2100" name="anio" value={formulario.anio} onChange={manejarCambio} />
                </label>
                <label className="field">
                  <span>Capacidad (kg)</span>
                  <input type="number" min="0" step="0.01" name="capacidadKg" value={formulario.capacidadKg} onChange={manejarCambio} />
                </label>
              </div>

              <label className="field checkbox-field">
                <span>Disponible</span>
                <input type="checkbox" name="estaDisponible" checked={formulario.estaDisponible} onChange={manejarCambio} />
              </label>

              {error && <div className="form-message form-message--error">{error}</div>}
              {exito && <div className="form-message form-message--success">{exito}</div>}

              <div className="modal-actions">
                <button type="button" className="ghost-button" onClick={cerrarFormulario}>
                  Cancelar
                </button>
                <button type="submit" className="primary-button" disabled={guardando}>
                  {guardando ? 'Guardando...' : vehiculoEditando ? 'Actualizar vehículo' : 'Crear vehículo'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
