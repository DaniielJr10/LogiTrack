import Icon from '../common/Icon'
import { navItems } from '../../data/dashboardData'

export default function Sidebar() {
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
          {navItems.map(({ label, icon, active, new: isNew }) => (
            <li key={label} className={`nav-item ${active ? 'active' : ''}`}>
              <span className="nav-icon"><Icon name={icon} /></span>
              <span className="nav-text">{label}</span>
              {isNew && <span className="nav-badge">Nuevo</span>}
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
        <div className="avatar">DJ</div>
        <div className="profile-meta">
          <strong>Daniel Jr.</strong>
          <span>Administrador</span>
        </div>
      </div>
    </aside>
  )
}
