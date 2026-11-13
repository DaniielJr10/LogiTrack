import Icon from '../common/Icon'

export default function LogisticsActivity() {
  return (
    <article className="card chart-card">
      <div className="card-header">
        <h3>Actividad Logística</h3>
        <div className="toggle">
          Últimos 30 días <Icon name="chevron" />
        </div>
      </div>
      <div className="legend">
        <span><i className="swatch blue" />Pedidos</span>
        <span><i className="swatch cyan" />Entregas</span>
      </div>
      <div className="chart" aria-label="Estadísticas de actividad">
        <div className="bars">
          <span style={{ '--h': '25%' }} />
          <span style={{ '--h': '45%' }} />
          <span style={{ '--h': '62%' }} />
          <span style={{ '--h': '58%' }} />
          <span style={{ '--h': '74%' }} />
          <span style={{ '--h': '88%' }} />
          <span style={{ '--h': '71%' }} />
        </div>
        <div className="chart-labels">
          <span>Lun</span><span>Mar</span><span>Mié</span><span>Jue</span><span>Vie</span><span>Sáb</span><span>Dom</span>
        </div>
      </div>
    </article>
  )
}
