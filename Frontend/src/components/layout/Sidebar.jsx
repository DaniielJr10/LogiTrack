import { NavLink } from 'react-router-dom';
import Icon from '../common/Icon';
import { navItems } from '../../data/dashboardData';
import { useAuth } from '../../contextos/AuthContext';

export default function Sidebar() {
  const { user, roles } = useAuth();

  return (
    <aside className="sidebar">
      <div className="brand-block">
        <div className="brand-mark">
          <span className="truck-icon"><Icon name="truck" /></span>
        </div>
        <div className="brand-text">LogiTrack</div>
        <small>Logística Inteligente</small>
      </div>

      <nav className="sidebar-nav" aria-label="Principal">
        <ul>
          {navItems.map(({ label, icon, to, new: isNew }) => (
            <li key={label}>
              <NavLink
                to={to}
                end={to === '/'}
                className={({ isActive }) => `nav-item ${isActive ? 'active' : ''}`}
              >
                <span className="nav-icon"><Icon name={icon} /></span>
                <span className="nav-text">{label}</span>
                {isNew && <span className="nav-badge">Nuevo</span>}
              </NavLink>
            </li>
          ))}
        </ul>
      </nav>

      <div className="ai-panel">
        <div className="ai-badge">AI</div>
        <div className="ai-text">
          <h3>LogiTrack AI</h3>
          <p>Tu asistente inteligente de logística</p>
        </div>
        <button type="button">Probar ahora</button>
      </div>

      <div className="profile-card">
        <div className="avatar">{(user?.nombres || 'U').charAt(0).toUpperCase()}</div>
        <div className="profile-meta">
          <strong>{`${user?.nombres || ''} ${user?.apellidos || ''}`.trim() || 'Usuario'}</strong>
          <span>{roles[0] || 'Usuario'}</span>
        </div>
      </div>
    </aside>
  );
}
