import { useEffect, useState } from 'react';
import Sidebar from '../../components/layout/Sidebar';
import Topbar from '../../components/layout/Topbar';
import WelcomeBanner from '../../components/dashboard/WelcomeBanner';
import KpiCard from '../../components/dashboard/KpiCard';
import DeliveryTracking from '../../components/dashboard/DeliveryTracking';
import FleetStatus from '../../components/dashboard/FleetStatus';
import LogiTrackAI from '../../components/dashboard/LogiTrackAI';
import RecentActivity from '../../components/dashboard/RecentActivity';
import { summaryCards } from '../../data/dashboardData';
import { useAuth } from '../../contextos/AuthContext';
import { apiFetch } from '../../servicios/api';

const normalizarLista = (valor) => {
  if (Array.isArray(valor)) {
    return valor;
  }

  const datos = valor?.datos ?? valor?.data ?? [];
  return Array.isArray(datos) ? datos : [];
};

export default function Dashboard() {
  const { token } = useAuth();
  const [kpis, setKpis] = useState(summaryCards);

  useEffect(() => {
    if (!token) {
      setKpis(summaryCards);
      return;
    }

    let activo = true;

    const cargarMetrica = async () => {
      try {
        const [pedidosRespuesta, productosRespuesta, vehiculosRespuesta, entregasRespuesta] = await Promise.all([
          apiFetch('/pedidos', { method: 'GET', token }),
          apiFetch('/inventario/productos', { method: 'GET', token }),
          apiFetch('/logistica/vehiculos', { method: 'GET', token }),
          apiFetch('/logistica/entregas', { method: 'GET', token }),
        ]);

        if (!activo) {
          return;
        }

        const pedidos = normalizarLista(pedidosRespuesta);
        const productos = normalizarLista(productosRespuesta);
        const vehiculos = normalizarLista(vehiculosRespuesta);
        const entregas = normalizarLista(entregasRespuesta);

        const stockTotal = productos.reduce((total, item) => total + Number(item.stockActual ?? item.StockActual ?? 0), 0);
        const vehiculosActivos = vehiculos.filter((vehiculo) =>
          vehiculo.estaDisponible ?? vehiculo.EstaDisponible ?? true,
        ).length;

        setKpis([
          { label: 'Total de Pedidos', value: `${pedidos.length}`.replace(/\B(?=(\d{3})+(?!\d))/g, ','), delta: '+12.5%', tone: 'blue' },
          { label: 'Inventario Disponible', value: `${stockTotal}`.replace(/\B(?=(\d{3})+(?!\d))/g, ','), delta: '+8.3%', tone: 'green' },
          { label: 'Vehículos Activos', value: `${vehiculosActivos} / ${vehiculos.length || 0}`, delta: '+3.7%', tone: 'purple' },
          { label: 'Entregas del Día', value: `${entregas.length}`.replace(/\B(?=(\d{3})+(?!\d))/g, ','), delta: '+15.2%', tone: 'orange' },
        ]);
      } catch {
        if (activo) {
          setKpis(summaryCards);
        }
      }
    };

    cargarMetrica();

    return () => {
      activo = false;
    };
  }, [token]);

  return (
    <div className="app-shell">
      <Sidebar />

      <main className="main-panel">
        <Topbar />
        <WelcomeBanner />

        <section className="stats-grid">
          {kpis.map((card) => (
            <KpiCard key={card.label} {...card} />
          ))}
        </section>

        <section className="content-grid">
          <DeliveryTracking />
          <FleetStatus />
        </section>

        <section className="bottom-grid">
          <LogiTrackAI />
          <RecentActivity />
        </section>

        <footer className="app-footer">
          <div className="system-state"><span className="dot" />Sistema en línea</div>
          <div className="last-sync">Última actualización: {new Date().toLocaleString('es-CO')}</div>
          <div className="brand-mark-footer">
            <span className="mini-icon">◧</span> LogiTrack <small>• Conectando tu mundo logístico</small>
          </div>
        </footer>
      </main>
    </div>
  );
}
