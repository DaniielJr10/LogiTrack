import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contextos/AuthContext';

const rolesDisponibles = ['Administrador', 'CoordinadorLogistico', 'AuxiliarBodega', 'Conductor'];

export default function AdminUsersPage() {
  const { listUsers, toggleUserStatus, updateUserRole, user } = useAuth();
  const [usuarios, setUsuarios] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState('');

  const cargarUsuarios = async () => {
    setCargando(true);
    setError('');

    try {
      const datos = await listUsers();
      setUsuarios(Array.isArray(datos) ? datos : []);
    } catch (err) {
      setError(err.message || 'No se pudieron cargar los usuarios.');
    } finally {
      setCargando(false);
    }
  };

  useEffect(() => {
    cargarUsuarios();
  }, []);

  const cambiarEstado = async (id, estadoActual) => {
    try {
      await toggleUserStatus(id, !estadoActual);
      setUsuarios((actual) => actual.map((usuario) =>
        usuario.id === id ? { ...usuario, estaActivo: !estadoActual } : usuario,
      ));
    } catch (err) {
      setError(err.message || 'No se pudo cambiar el estado del usuario.');
    }
  };

  const cambiarRol = async (id, rol) => {
    try {
      await updateUserRole(id, rol);
      setUsuarios((actual) => actual.map((usuario) =>
        usuario.id === id ? { ...usuario, rolPrincipal: rol } : usuario,
      ));
    } catch (err) {
      setError(err.message || 'No se pudo cambiar el rol del usuario.');
    }
  };

  return (
    <div className="page-shell page-shell--admins">
      <div className="page-card">
        <div className="page-card__header">
          <div>
            <p className="eyebrow">Administración</p>
            <h2>Usuarios</h2>
          </div>
          <div className="page-card__actions">
            <Link to="/" className="secondary-button">Volver al panel</Link>
          </div>
        </div>

        {error && <div className="form-message form-message--error">{error}</div>}

        {cargando ? (
          <div className="loading-state">Cargando usuarios...</div>
        ) : usuarios.length === 0 ? (
          <div className="empty-state">No hay usuarios registrados.</div>
        ) : (
          <div className="table-wrap">
            <table className="table-admin">
              <thead>
                <tr>
                  <th>Usuario</th>
                  <th>Rol</th>
                  <th>Estado</th>
                  <th>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {usuarios.map((usuario) => (
                  <tr key={usuario.id}>
                    <td>
                      <div className="user-cell">
                        <span className="user-avatar">{(usuario.nombres || 'U').charAt(0).toUpperCase()}</span>
                        <div>
                          <strong>{`${usuario.nombres || ''} ${usuario.apellidos || ''}`.trim() || 'Sin nombre'}</strong>
                          <small>{usuario.email}</small>
                        </div>
                      </div>
                    </td>
                    <td>
                      <select
                        value={usuario.rolPrincipal || 'CoordinadorLogistico'}
                        onChange={(event) => cambiarRol(usuario.id, event.target.value)}
                        className="select-input"
                        disabled={usuario.email === user?.email}
                      >
                        {rolesDisponibles.map((rol) => (
                          <option key={rol} value={rol}>{rol}</option>
                        ))}
                      </select>
                    </td>
                    <td>
                      <span className={`status-pill ${usuario.estaActivo ? 'is-active' : 'is-inactive'}`}>
                        {usuario.estaActivo ? 'Activo' : 'Inactivo'}
                      </span>
                    </td>
                    <td>
                      <button
                        type="button"
                        className="secondary-button"
                        onClick={() => cambiarEstado(usuario.id, usuario.estaActivo)}
                        disabled={usuario.email === user?.email}
                      >
                        {usuario.estaActivo ? 'Desactivar' : 'Activar'}
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
