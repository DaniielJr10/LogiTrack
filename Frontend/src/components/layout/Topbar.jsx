import { Link, useNavigate } from 'react-router-dom';
import Icon from '../common/Icon';
import { useAuth } from '../../contextos/AuthContext';

export default function Topbar() {
  const navigate = useNavigate();
  const { user, logout, isAdmin } = useAuth();

  const cerrarSesion = async () => {
    await logout();
    navigate('/login', { replace: true });
  };

  return (
    <header className="topbar">
      <label className="search-box" aria-label="Buscar">
        <span className="search-icon"><Icon name="search" /></span>
        <input type="text" placeholder="Buscar en LogiTrack..." aria-label="Buscar en LogiTrack" />
      </label>

      <div className="header-actions">
        <button type="button" className="icon-button notification" aria-label="Notificaciones">
          <span className="badge">3</span>
          <Icon name="bell" />
        </button>
        <button type="button" className="icon-button settings" aria-label="Configuración">
          <Icon name="cog" />
        </button>
        <button type="button" className="theme-button" aria-label="Tema">
          <Icon name="spark" />
        </button>

        <Link to="/perfil" className="user-chip" title="Mi perfil">
          <span className="user-chip__avatar">{(user?.nombres || 'U').charAt(0).toUpperCase()}</span>
          <span className="user-chip__label">{user?.nombres || 'Usuario'}</span>
        </Link>

        {isAdmin && (
          <Link to="/admin" className="secondary-button header-admin-button">Admin</Link>
        )}

        <button type="button" className="ghost-button" onClick={cerrarSesion}>Salir</button>
      </div>
    </header>
  );
}
