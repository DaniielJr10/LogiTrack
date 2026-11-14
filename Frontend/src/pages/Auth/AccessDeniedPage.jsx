import { Link } from 'react-router-dom';

export default function AccessDeniedPage() {
  return (
    <div className="status-page">
      <div className="status-card">
        <p className="eyebrow">Acceso restringido</p>
        <h1>403</h1>
        <h2>No tienes permisos para ver esta sección</h2>
        <p>Tu cuenta no tiene autorización para administrar esta área.</p>
        <div className="status-actions">
          <Link to="/" className="primary-button">Volver al inicio</Link>
          <Link to="/perfil" className="secondary-button">Mi perfil</Link>
        </div>
      </div>
    </div>
  );
}
