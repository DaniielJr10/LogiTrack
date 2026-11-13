import Icon from '../common/Icon'

export default function KpiCard({ label, value, delta, tone }) {
  return (
    <article className={`stat-card ${tone}`}>
      <div className="stat-head">
        <div className="stat-icon"><Icon name="truck" /></div>
        <div className="stat-label">{label}</div>
      </div>
      <div className="stat-value">{value}</div>
      <div className="stat-trend"><span>↑</span> {delta}</div>
      <div className="mini-line"><span /></div>
    </article>
  )
}
