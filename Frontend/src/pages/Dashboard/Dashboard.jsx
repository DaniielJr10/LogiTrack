import Sidebar from '../../components/layout/Sidebar'
import Topbar from '../../components/layout/Topbar'
import WelcomeBanner from '../../components/dashboard/WelcomeBanner'
import KpiCard from '../../components/dashboard/KpiCard'
import LogisticsActivity from '../../components/dashboard/LogisticsActivity'
import DeliveryTracking from '../../components/dashboard/DeliveryTracking'
import FleetStatus from '../../components/dashboard/FleetStatus'
import TopProducts from '../../components/dashboard/TopProducts'
import LogiTrackAI from '../../components/dashboard/LogiTrackAI'
import RecentActivity from '../../components/dashboard/RecentActivity'
import { summaryCards } from '../../data/dashboardData'

export default function Dashboard() {
  return (
    <div className="app-shell">
      <Sidebar />

      <main className="main-panel">
        <Topbar />
        <WelcomeBanner />

        <section className="stats-grid">
          {summaryCards.map((card) => (
            <KpiCard key={card.label} {...card} />
          ))}
        </section>

        <section className="content-grid">
          <LogisticsActivity />
          <DeliveryTracking />
          <FleetStatus />
        </section>

        <section className="bottom-grid">
          <TopProducts />
          <LogiTrackAI />
          <RecentActivity />
        </section>

        <footer className="app-footer">
          <div className="system-state"><span className="dot" />Sistema en línea</div>
          <div className="last-sync">Última actualización: 14/04/2025 - 14:25</div>
          <div className="brand-mark-footer">
            <span className="mini-icon">◧</span> LogiTrack <small>• Conectando tu mundo logístico</small>
          </div>
        </footer>
      </main>
    </div>
  )
}
