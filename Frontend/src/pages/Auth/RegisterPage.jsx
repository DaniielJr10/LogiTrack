import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../contextos/AuthContext';

const estadoInicial = {
  nombres: '',
  apellidos: '',
  email: '',
  phoneNumber: '',
  password: '',
};

export default function RegisterPage() {
  const navigate = useNavigate();
  const { register } = useAuth();
  const [formulario, setFormulario] = useState(estadoInicial);
  const [error, setError] = useState('');
  const [cargando, setCargando] = useState(false);

  const manejarCambio = (evento) => {
    const { name, value } = evento.target;
    setFormulario((actual) => ({ ...actual, [name]: value }));
  };

  const manejarEnvio = async (evento) => {
    evento.preventDefault();
    setError('');
    setCargando(true);

    try {
      await register({
        nombres: formulario.nombres.trim(),
        apellidos: formulario.apellidos.trim(),
        email: formulario.email.trim(),
        phoneNumber: formulario.phoneNumber.trim(),
        password: formulario.password,
      });
      navigate('/', { replace: true });
    } catch (err) {
      setError(err.message || 'No se pudo registrar el usuario.');
    } finally {
      setCargando(false);
    }
  };

  return (
    <div className="auth-shell">
      <div className="auth-card auth-card--wide">
        <div className="auth-visual">
          <div className="auth-brand">
            <span className="auth-brand__mark">L</span>
            <div>
              <strong>LogiTrack</strong>
              <small>Unete a la operación</small>
            </div>
          </div>

          <div className="auth-visual__panel">
            <h1>Tu equipo logistico, mejor conectado.</h1>
            <p>Crea tu cuenta y comienza a gestionar operaciones, envios y control de inventario.</p>
          </div>
        </div>

        <div className="auth-form-panel">
          <div className="auth-header">
            <p className="eyebrow">Nueva cuenta</p>
            <h2>Registro de usuario</h2>
            <p className="auth-note">Modo frontend activo: el registro es de demostración.</p>
          </div>

          <form className="auth-form" onSubmit={manejarEnvio}>
            <div className="field-row">
              <label className="field">
                <span>Nombres</span>
                <input name="nombres" value={formulario.nombres} onChange={manejarCambio} placeholder="Tu nombre" required />
              </label>
              <label className="field">
                <span>Apellidos</span>
                <input name="apellidos" value={formulario.apellidos} onChange={manejarCambio} placeholder="Tus apellidos" required />
              </label>
            </div>

            <label className="field">
              <span>Correo electrónico</span>
              <input type="email" name="email" value={formulario.email} onChange={manejarCambio} placeholder="tu@empresa.com" required />
            </label>

            <label className="field">
              <span>Teléfono</span>
              <input name="phoneNumber" value={formulario.phoneNumber} onChange={manejarCambio} placeholder="555-1234" />
            </label>

            <label className="field">
              <span>Contraseña</span>
              <input type="password" name="password" value={formulario.password} onChange={manejarCambio} placeholder="Mínimo 8 caracteres" required />
            </label>

            {error && <div className="form-message form-message--error">{error}</div>}

            <button type="submit" className="primary-button" disabled={cargando}>
              {cargando ? 'Creando cuenta...' : 'Crear cuenta'}
            </button>
          </form>

          <div className="auth-links">
            <span>¿Ya tienes cuenta?</span>
            <Link to="/login">Inicia sesión</Link>
          </div>
        </div>
      </div>
    </div>
  );
}
