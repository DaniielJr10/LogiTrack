export const variablesGlobales = {
  nombreAplicacion: 'LogiTrack',
  version: '0.1.0',
  apiBaseUrl: (import.meta.env.VITE_API_URL || 'http://localhost:5116/api').replace(/\/$/, ''),
  modoSoloFrontend: import.meta.env.VITE_FRONTEND_ONLY_AUTH === 'true',
};
