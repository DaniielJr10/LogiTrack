export const variablesGlobales = {
  nombreAplicacion: 'LogiTrack',
  version: '0.1.0',
  apiBaseUrl: import.meta.env.VITE_API_URL || 'https://localhost:7153/api',
  modoSoloFrontend: import.meta.env.VITE_FRONTEND_ONLY_AUTH !== 'false',
};
