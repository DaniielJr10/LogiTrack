import { activities } from '../../data/dashboardData'

export default function RecentActivity() {
  return (
    <article className="card activity-card">
      <div className="card-header compact">
        <h3>Últimas Actividades</h3>
        <button type="button">
          Ver todas <span>›</span>
        </button>
      </div>
      <ul className="activity-list">
        {activities.map(({ text, time, status }) => (
          <li key={text} className={`activity-item ${status}`}>
            <span className="dot" />
            <span className="text">{text}</span>
            <span className="time">{time}</span>
          </li>
        ))}
      </ul>
    </article>
  )
}
