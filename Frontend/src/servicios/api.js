import { variablesGlobales } from '../configuracion/entorno';

const construirCabeceras = (token, extraCabeceras = {}) => {
  const cabeceras = {
    Accept: 'application/json',
    ...extraCabeceras,
  };

  if (token) {
    cabeceras.Authorization = `Bearer ${token}`;
  }

  if (!(extraCabeceras.body instanceof FormData)) {
    cabeceras['Content-Type'] = 'application/json';
  }

  return cabeceras;
};

const parsearRespuesta = async (respuesta) => {
  const texto = await respuesta.text();

  if (!texto) {
    return null;
  }

  try {
    return JSON.parse(texto);
  } catch {
    return texto;
  }
};

export async function apiFetch(endpoint, { method = 'GET', body, token } = {}) {
  const opciones = {
    method,
    headers: construirCabeceras(token, body instanceof FormData ? { body } : {}),
  };

  if (body !== undefined && !(body instanceof FormData)) {
    opciones.body = JSON.stringify(body);
  }

  if (body instanceof FormData) {
    opciones.body = body;
  }

  const respuesta = await fetch(`${variablesGlobales.apiBaseUrl}${endpoint}`, opciones);
  const payload = await parsearRespuesta(respuesta);

  if (!respuesta.ok) {
    const mensaje = payload?.mensaje || payload?.title || 'La solicitud falló.';
    throw new Error(mensaje);
  }

  return payload;
}
