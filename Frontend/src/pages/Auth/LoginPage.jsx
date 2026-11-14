import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../contextos/AuthContext';

export default function LoginPage() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [formulario, setFormulario] = useState({ email: '', password: '' });
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
      await login({
        email: formulario.email.trim(),
        password: formulario.password,
      });
      navigate('/', { replace: true });
    } catch (err) {
      setError(err.message || 'No se pudo iniciar sesión.');
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
              <small>Logística inteligente</small>
            </div>
          </div>

          <div className="auth-visual__panel">
            <h1>Controla tu operación logística en tiempo real.</h1>
            <p>
              Gestiona pedidos, inventario, flota y equipos desde una sola plataforma empresarial.
            </p>
          </div>
        </div>

        <div className="auth-form-panel">
          <div className="auth-header">
            <p className="eyebrow">Bienvenido</p>
            <h2>Iniciar sesión</h2>
            <p className="auth-note">Modo frontend activo: el acceso es de demostración.</p>
          </div>

          <form className="auth-form" onSubmit={manejarEnvio}>
            <label className="field">
              <span>Correo electrónico</span>
              <input
                type="email"
                name="email"
                value={formulario.email}
                onChange={manejarCambio}
                placeholder="tu@empresa.com"
                required
              />
            </label>

            <label className="field">
              <span>Contraseña</span>
              <input
                type="password"
                name="password"
                value={formulario.password}
                onChange={manejarCambio}
                placeholder="••••••••"
                required
              />
            </label>

            {error && <div className="form-message form-message--error">{error}</div>}

            <button type="submit" className="primary-button" disabled={cargando}>
              {cargando ? 'Ingresando...' : 'Ingresar'}
            </button>
          </form>

          <div className="auth-links">
            <span>¿No tienes cuenta?</span>
            <Link to="/registro">Crear cuenta</Link>
          </div>
        </div>
      </div>
    </div>
  );
}
