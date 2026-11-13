import { iconMap } from '../../data/dashboardData'

export default function Icon({ name, className = '' }) {
  const markup = iconMap[name] ?? iconMap.truck

  return <span className={className} dangerouslySetInnerHTML={{ __html: markup }} />
}
