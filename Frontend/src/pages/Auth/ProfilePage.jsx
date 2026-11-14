import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../contextos/AuthContext';

export default function ProfilePage() {
  const navigate = useNavigate();
  const { user, updateProfile, logout, refreshProfile, roles } = useAuth();
  const [formulario, setFormulario] = useState({
    nombres: '',
    apellidos: '',
    phoneNumber: '',
  });
  const [error, setError] = useState('');
  const [exito, setExito] = useState('');
  const [cargando, setCargando] = useState(false);

  useEffect(() => {
    if (!user) {
      navigate('/login', { replace: true });
      return;
    }

    setFormulario({
      nombres: user.nombres || '',
      apellidos: user.apellidos || '',
      phoneNumber: user.phoneNumber || '',
    });

    refreshProfile().catch(() => {});
  }, [user, navigate, refreshProfile]);

  const manejarCambio = (evento) => {
    const { name, value } = evento.target;
    setFormulario((actual) => ({ ...actual, [name]: value }));
  };

  const manejarEnvio = async (evento) => {
    evento.preventDefault();
    setError('');
    setExito('');
    setCargando(true);

    try {
      await updateProfile(formulario);
      setExito('Perfil actualizado correctamente.');
    } catch (err) {
      setError(err.message || 'No se pudo actualizar el perfil.');
    } finally {
      setCargando(false);
    }
  };

  const cerrarSesion = async () => {
    await logout();
    navigate('/login', { replace: true });
  };

  if (!user) {
    return null;
  }

  return (
    <div className="page-shell page-shell--profile">
      <div className="page-card">
        <div className="page-card__header">
          <div>
            <p className="eyebrow">Mi cuenta</p>
            <h2>Perfil de usuario</h2>
          </div>
          <div className="page-card__actions">
            <Link to="/" className="secondary-button">Volver al panel</Link>
            <button type="button" className="ghost-button" onClick={cerrarSesion}>Cerrar sesión</button>
          </div>
        </div>

        <div className="profile-summary">
          <div className="profile-summary__avatar">{(user.nombres || 'U').charAt(0).toUpperCase()}</div>
          <div>
            <h3>{`${user.nombres} ${user.apellidos}`.trim() || 'Usuario'}</h3>
            <p>{user.email}</p>
            <div className="role-badges">
              {(roles.length ? roles : ['Usuario']).map((rol) => (
                <span key={rol} className="role-pill">{rol}</span>
              ))}
            </div>
          </div>
        </div>

        <form className="auth-form" onSubmit={manejarEnvio}>
          <div className="field-row">
            <label className="field">
              <span>Nombres</span>
              <input name="nombres" value={formulario.nombres} onChange={manejarCambio} required />
            </label>
            <label className="field">
              <span>Apellidos</span>
              <input name="apellidos" value={formulario.apellidos} onChange={manejarCambio} required />
            </label>
          </div>

          <label className="field">
            <span>Correo electrónico</span>
            <input value={user.email} disabled />
          </label>

          <label className="field">
            <span>Teléfono</span>
            <input name="phoneNumber" value={formulario.phoneNumber} onChange={manejarCambio} placeholder="555-1234" />
          </label>

          {error && <div className="form-message form-message--error">{error}</div>}
          {exito && <div className="form-message form-message--success">{exito}</div>}

          <button type="submit" className="primary-button" disabled={cargando}>
            {cargando ? 'Guardando...' : 'Guardar cambios'}
          </button>
        </form>
      </div>
    </div>
  );
}
