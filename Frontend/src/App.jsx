import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { AuthProvider, useAuth } from './contextos/AuthContext';
import Dashboard from './pages/Dashboard/Dashboard';
import LoginPage from './pages/Auth/LoginPage';
import RegisterPage from './pages/Auth/RegisterPage';
import ProfilePage from './pages/Auth/ProfilePage';
import AdminUsersPage from './pages/Auth/AdminUsersPage';
import AccessDeniedPage from './pages/Auth/AccessDeniedPage';
import NotFoundPage from './pages/Auth/NotFoundPage';

function ProtectedRoute({ children, requireRole }) {
  const { isAuthenticated, loading, hasRole } = useAuth();

  if (loading) {
    return <div className="loading-state auth-loading">Verificando sesión...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (requireRole && !hasRole(requireRole)) {
    return <Navigate to="/acceso-denegado" replace />;
  }

  return children;
}

function PublicRoute({ children }) {
  const { isAuthenticated, loading } = useAuth();

  if (loading) {
    return <div className="loading-state auth-loading">Cargando...</div>;
  }

  if (isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  return children;
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/login"
            element={
              <PublicRoute>
                <LoginPage />
              </PublicRoute>
            }
          />
          <Route
            path="/registro"
            element={
              <PublicRoute>
                <RegisterPage />
              </PublicRoute>
            }
          />
          <Route
            path="/perfil"
            element={
              <ProtectedRoute>
                <ProfilePage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/admin"
            element={
              <ProtectedRoute requireRole="Administrador">
                <AdminUsersPage />
              </ProtectedRoute>
            }
          />
          <Route path="/acceso-denegado" element={<AccessDeniedPage />} />
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
