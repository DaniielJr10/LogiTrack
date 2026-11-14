import { Link } from 'react-router-dom';

export default function NotFoundPage() {
  return (
    <div className="status-page">
      <div className="status-card">
        <p className="eyebrow">Página no encontrada</p>
        <h1>404</h1>
        <h2>La ruta solicitada no existe</h2>
        <p>Verifica la URL o regresa al panel principal.</p>
        <div className="status-actions">
          <Link to="/" className="primary-button">Ir al inicio</Link>
        </div>
      </div>
    </div>
  );
}
