import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import { apiFetch } from '../servicios/api';
import { variablesGlobales } from '../configuracion/entorno';

const STORAGE_KEY = 'logitrack_session_v1';
const AuthContext = createContext(null);

const normalizarUsuario = (usuario = {}) => ({
  id: usuario.id ?? usuario.Id ?? null,
  nombres: usuario.nombres ?? usuario.Nombres ?? '',
  apellidos: usuario.apellidos ?? usuario.Apellidos ?? '',
  email: usuario.email ?? usuario.Email ?? '',
  userName: usuario.userName ?? usuario.UserName ?? '',
  phoneNumber: usuario.phoneNumber ?? usuario.PhoneNumber ?? '',
  estaActivo: usuario.estaActivo ?? usuario.EstaActivo ?? true,
  fechaRegistro: usuario.fechaRegistro ?? usuario.FechaRegistro ?? null,
  ultimoAcceso: usuario.ultimoAcceso ?? usuario.UltimoAcceso ?? null,
});

const normalizarSesion = (payload) => {
  const datos = payload?.datos ?? payload ?? {};
  const usuario = normalizarUsuario(datos.usuario ?? datos.Usuario ?? datos);
  const roles = Array.isArray(datos.roles) ? datos.roles : Array.isArray(datos.Roles) ? datos.Roles : [];

  return {
    token: datos.token ?? datos.Token ?? null,
    expiresAt: datos.expiraEnUtc ?? datos.ExpiraEnUtc ?? null,
    user: usuario,
    roles,
  };
};

const leerSesionGuardada = () => {
  try {
    const dato = localStorage.getItem(STORAGE_KEY);
    if (!dato) {
      return null;
    }

    const sesion = JSON.parse(dato);
    if (!sesion?.token) {
      return null;
    }

    return sesion;
  } catch {
    return null;
  }
};

const crearSesionDemo = (credenciales = {}) => {
  const emailNormalizado = `${credenciales.email ?? ''}`.trim().toLowerCase();
  const nombres = `${credenciales.nombres ?? ''}`.trim() || 'Usuario';
  const apellidos = `${credenciales.apellidos ?? ''}`.trim() || 'Demo';

  if (!emailNormalizado) {
    throw new Error('Debes ingresar un correo para continuar.');
  }

  return {
    datos: {
      token: `demo-token-${Date.now()}`,
      expiraEnUtc: null,
      usuario: {
        id: `demo-${emailNormalizado}`,
        nombres,
        apellidos,
        email: emailNormalizado,
        userName: emailNormalizado,
        phoneNumber: `${credenciales.phoneNumber ?? ''}`.trim(),
        estaActivo: true,
      },
      roles: ['Usuario'],
    },
  };
};

export function AuthProvider({ children }) {
  const [session, setSession] = useState(() => leerSesionGuardada());
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(false);
  }, []);

  useEffect(() => {
    if (session?.token) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
    } else {
      localStorage.removeItem(STORAGE_KEY);
    }
  }, [session]);

  const guardarSesion = (payload) => {
    const sesionNormalizada = normalizarSesion(payload);
    if (!sesionNormalizada.token) {
      return null;
    }

    setSession(sesionNormalizada);
    return sesionNormalizada;
  };

  const login = async (credenciales) => {
    if (variablesGlobales.modoSoloFrontend) {
      const payloadDemo = crearSesionDemo(credenciales);
      guardarSesion(payloadDemo);
      return payloadDemo;
    }

    const payload = await apiFetch('/auth/login', {
      method: 'POST',
      body: credenciales,
    });

    const sesion = guardarSesion(payload);
    if (!sesion) {
      throw new Error('La respuesta del servidor no incluye token válido.');
    }

    return payload;
  };

  const register = async (credenciales) => {
    if (variablesGlobales.modoSoloFrontend) {
      const payloadDemo = crearSesionDemo(credenciales);
      guardarSesion(payloadDemo);
      return payloadDemo;
    }

    const payload = await apiFetch('/auth/registro', {
      method: 'POST',
      body: credenciales,
    });

    const sesion = guardarSesion(payload);
    if (!sesion) {
      throw new Error('No se pudo completar el registro.');
    }

    return payload;
  };

  const logout = async () => {
    if (variablesGlobales.modoSoloFrontend) {
      setSession(null);
      return;
    }

    try {
      if (session?.token) {
        await apiFetch('/auth/logout', {
          method: 'POST',
          token: session.token,
        });
      }
    } catch {
      // ignoramos errores del logout para cerrar sesión localmente
    } finally {
      setSession(null);
    }
  };

  const refreshProfile = async () => {
    if (!session?.token) {
      return null;
    }

    const payload = await apiFetch('/auth/perfil', {
      method: 'GET',
      token: session.token,
    });

    const perfil = payload?.datos ?? payload ?? {};
    const usuario = normalizarUsuario(perfil);
    const roles = Array.isArray(perfil.roles) ? perfil.roles : Array.isArray(perfil.Roles) ? perfil.Roles : session.roles;

    setSession((actual) => ({
      ...actual,
      user: usuario,
      roles,
    }));

    return payload;
  };

  const updateProfile = async (datos) => {
    if (!session?.token) {
      throw new Error('Debes iniciar sesión para actualizar el perfil.');
    }

    const payload = await apiFetch('/auth/perfil', {
      method: 'PUT',
      body: datos,
      token: session.token,
    });

    if (payload?.datos) {
      const usuario = normalizarUsuario(payload.datos);
      setSession((actual) => ({
        ...actual,
        user: usuario,
      }));
    }

    return payload;
  };

  const listUsers = async () => {
    if (!session?.token) {
      throw new Error('Debes iniciar sesión para consultar usuarios.');
    }

    const payload = await apiFetch('/usuarios', {
      method: 'GET',
      token: session.token,
    });

    return payload?.datos ?? [];
  };

  const toggleUserStatus = async (id, estaActivo) => {
    if (!session?.token) {
      throw new Error('Debes iniciar sesión para cambiar el estado del usuario.');
    }

    const payload = await apiFetch(`/usuarios/${id}/estado`, {
      method: 'PATCH',
      body: { estaActivo },
      token: session.token,
    });

    return payload;
  };

  const updateUserRole = async (id, rol) => {
    if (!session?.token) {
      throw new Error('Debes iniciar sesión para cambiar el rol del usuario.');
    }

    const payload = await apiFetch(`/usuarios/${id}/rol`, {
      method: 'PUT',
      body: { rol },
      token: session.token,
    });

    return payload;
  };

  const hasRole = (rol) => (session?.roles ?? []).includes(rol);

  const value = useMemo(() => ({
    session,
    user: session?.user ?? null,
    token: session?.token ?? null,
    roles: session?.roles ?? [],
    isAuthenticated: Boolean(session?.token),
    isAdmin: hasRole('Administrador'),
    loading,
    login,
    register,
    logout,
    refreshProfile,
    updateProfile,
    listUsers,
    toggleUserStatus,
    updateUserRole,
    hasRole,
  }), [session, loading]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const contexto = useContext(AuthContext);

  if (!contexto) {
    throw new Error('useAuth debe usarse dentro de AuthProvider.');
  }

  return contexto;
}
