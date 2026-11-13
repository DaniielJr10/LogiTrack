import Icon from '../common/Icon'

export default function Topbar() {
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
      </div>
    </header>
  )
}
