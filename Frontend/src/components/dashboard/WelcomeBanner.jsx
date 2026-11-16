import { useAuth } from '../../contextos/AuthContext';

export default function WelcomeBanner() {
  const { user } = useAuth();
  const nombreUsuario = `${user?.nombres || ''} ${user?.apellidos || ''}`.trim() || 'Usuario';

  return (
    <section className="hero-banner">
      <div className="hero-copy">
        <h1>
          ¡Hola, {nombreUsuario}!
          <span className="hero-wave" aria-hidden="true">👋</span>
        </h1>
        <p>Aquí tienes el resumen de tu operación logística de hoy.</p>
        <div className="hero-divider" />
      </div>
      <div className="hero-visual" aria-hidden="true">
        <div className="hero-plane">✈</div>
        <div className="hero-truck">
          <div className="truck-cabin" />
          <div className="truck-cab" />
          <div className="truck-trailer" />
          <div className="truck-wheel wheel-front" />
          <div className="truck-wheel wheel-mid" />
          <div className="truck-wheel wheel-back" />
        </div>
      </div>
    </section>
  )
}
