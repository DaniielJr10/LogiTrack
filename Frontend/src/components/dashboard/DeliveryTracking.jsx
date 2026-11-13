import Icon from '../common/Icon'

export default function DeliveryTracking() {
  return (
    <article className="card map-card">
      <div className="card-header compact">
        <h3>Seguimiento de Entregas</h3>
        <button type="button">
          Ver todas <Icon name="chevron" />
        </button>
      </div>
      <div className="map-surface">
        <div className="map-road road-1" />
        <div className="map-road road-2" />
        <div className="map-road road-3" />
        <div className="marker start" />
        <div className="marker end" />
        <div className="route-line" />
        <div className="delivery-card">
          <span className="status-pill">En camino</span>
          <strong>Llegada estimada:</strong>
          <em>14:30</em>
        </div>
      </div>
    </article>
  )
}
