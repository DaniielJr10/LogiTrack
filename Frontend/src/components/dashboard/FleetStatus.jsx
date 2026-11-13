export default function FleetStatus() {
  return (
    <article className="card status-card">
      <div className="card-header compact">
        <h3>Estado de la Flota</h3>
      </div>
      <div className="donut-wrap">
        <div className="donut">
          <div className="donut-center">
            <strong>85%</strong>
            <span>En Ruta</span>
          </div>
        </div>
      </div>
      <ul className="status-list">
        <li><span className="bullet green" />En Ruta <b>20</b></li>
        <li><span className="bullet gray" />Mantenimiento <b>3</b></li>
        <li><span className="bullet dark" />Fuera de Servicio <b>1</b></li>
      </ul>
      <div className="trend-box"><span>↑</span> +5% vs. ayer</div>
    </article>
  )
}
