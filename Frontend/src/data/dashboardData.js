export const navItems = [
  { label: 'Dashboard', active: true, icon: 'home' },
  { label: 'Inventario', icon: 'box' },
  { label: 'Pedidos', icon: 'basket' },
  { label: 'Flota', icon: 'fleet' },
  { label: 'Entregas', icon: 'delivery' },
  { label: 'Reports', icon: 'report' },
  { label: 'LogiTrack AI', icon: 'spark', new: true },
]

export const summaryCards = [
  { label: 'Total de Pedidos', value: '1,248', delta: '+12.5%', tone: 'blue' },
  { label: 'Inventario Disponible', value: '85,320', delta: '+8.3%', tone: 'green' },
  { label: 'Vehículos Activos', value: '24 / 28', delta: '+3.7%', tone: 'purple' },
  { label: 'Entregas del Día', value: '892', delta: '+15.2%', tone: 'orange' },
]

export const products = [
  { name: 'Laptop Pro 15"', value: 342, icon: 'laptop' },
  { name: 'Mouse Inalámbrico', value: 289, icon: 'mouse' },
  { name: 'Teclado Mecánico', value: 245, icon: 'keyboard' },
]

export const activities = [
  { text: 'Pedido #LT-7842 entregado', time: 'Hace 5 min', status: 'success' },
  { text: 'Nuevo pedido recibido #LT-7843', time: 'Hace 12 min', status: 'info' },
  { text: 'Vehículo V-124 en ruta', time: 'Hace 18 min', status: 'warning' },
  { text: 'Inventario actualizado', time: 'Hace 25 min', status: 'success' },
]

export const iconMap = {
  home: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 11.5 12 5l8 6.5V19a1 1 0 0 1-1 1h-4v-7H9v7H5a1 1 0 0 1-1-1v-7.5Z"/></svg>',
  box: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 2.8 4 7v10l8 4.2 8-4.2V7l-8-4.2Zm0 2.4 5.4 2.8L12 10.8 6.6 8 12 5.2Zm-6 4.2 5 2.6v7.2l-5-2.6V9.4Zm7 9.8v-7.2l5-2.6v7.2l-5 2.6Z"/></svg>',
  basket: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M6.5 9h11l-.8 9.1a2 2 0 0 1-2 1.9H9.3a2 2 0 0 1-2-1.9L6.5 9Zm2.5-2.2A3 3 0 0 1 12 4a3 3 0 0 1 3 2.8V9h-6V6.8ZM9 9h6V7.2a2.2 2.2 0 1 0-4.4 0V9Z"/></svg>',
  fleet: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M3 9.5V7.2A1.2 1.2 0 0 1 4.2 6h7.3l2.1 3.5H18a2 2 0 0 1 2 2v2.8h-1.3a2.7 2.7 0 1 0-5.4 0H9.7a2.7 2.7 0 1 0-5.4 0H3V9.5Zm2.4 5.2a1.3 1.3 0 1 1 0 2.6 1.3 1.3 0 0 1 0-2.6Zm12 0a1.3 1.3 0 1 1 0 2.6 1.3 1.3 0 0 1 0-2.6Z"/></svg>',
  delivery: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M3 7.5A2.5 2.5 0 0 1 5.5 5h8.1a2.5 2.5 0 0 1 2.1 1.1L17.8 9H20a2 2 0 0 1 2 2v3.2a2.5 2.5 0 0 1-2 2.5h-.8a3 3 0 1 1-6 0H9.7a3 3 0 1 1-6 0H3V7.5Zm2 0v9.5h1.1a3 3 0 0 1 5.7 0h4.4a3 3 0 0 1 5.7 0H20v-4h-2.2a2 2 0 0 1-1.7-1l-1.5-3.5H5.5Z"/></svg>',
  report: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M7 3.5h7l5 5V18a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V5.5a2 2 0 0 1 2-2Zm6 2.1v4h4.5v.8H13v-4.8Zm-4 8.4h8v1.4H9v-1.4Zm0 3h8v1.4H9v-1.4Z"/></svg>',
  search: '<svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="11" cy="11" r="5.3"></circle><path d="m15.5 15.5 4.5 4.5"></path></svg>',
  bell: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 3a5 5 0 0 1 5 5v3.5l1.7 4.3H5.3L7 11.5V8a5 5 0 0 1 5-5Zm0 18a2.5 2.5 0 0 1-2.5-2h5A2.5 2.5 0 0 1 12 21Z"/></svg>',
  cog: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 8.8A3.2 3.2 0 1 1 12 15.2 3.2 3.2 0 0 1 12 8.8Zm9 3.2-1.4-.7a7.5 7.5 0 0 0-.7-1.7L19.7 9l-1.4-2.4-1.8.6c-.5-.4-1-.7-1.6-.9L14 5.5h-4l-.2 1.8c-.6.2-1.1.5-1.6.9l-1.8-.6L4.3 9l1.1 1.6c-.3.5-.6 1.1-.7 1.7L3.3 13l.8 1.9 1.8-.5c.5.4 1 .7 1.6.9L8 17.5h4l.2-1.8c.6-.2 1.1-.5 1.6-.9l1.8.5 1.4-2.4-.9-1.5Z"/></svg>',
  spark: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="m12 2 2.1 6.9L21 11l-6.9 2.1L12 20l-2.1-6.9L3 11l6.9-2.1L12 2Z"/></svg>',
  chevron: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="m8 6 6 6-6 6"/></svg>',
  truck: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M3 8.5V7a2 2 0 0 1 2-2h8.7l2.2 3.5H19a2 2 0 0 1 2 2v3.5h-1.2a3 3 0 0 0-6 0H9.2a3 3 0 0 0-6 0H3V8.5Zm2.6 8.5a1.6 1.6 0 1 0 0 3.2 1.6 1.6 0 0 0 0-3.2Zm12.8 0a1.6 1.6 0 1 0 0 3.2 1.6 1.6 0 0 0 0-3.2Z"/></svg>',
  laptop: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M5 6.5A2.5 2.5 0 0 1 7.5 4h9A2.5 2.5 0 0 1 19 6.5v6A2.5 2.5 0 0 1 16.5 15h-9A2.5 2.5 0 0 1 5 12.5v-6Zm2.5 2h9v5h-9v-5Zm-2.5 8h14l1.5 2.5H3.5L5 16.5Z"/></svg>',
  mouse: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 2.5A6 6 0 0 1 18 8.5v7A6 6 0 0 1 6 15.5v-7A6 6 0 0 1 12 2.5Zm0 2.5a3.5 3.5 0 0 0-3.5 3.5v2h7V8.5A3.5 3.5 0 0 0 12 5Z"/></svg>',
  keyboard: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 8.5A2.5 2.5 0 0 1 6.5 6h11A2.5 2.5 0 0 1 20 8.5v7A2.5 2.5 0 0 1 17.5 18h-11A2.5 2.5 0 0 1 4 15.5v-7Zm2.5 1h3v2h-3v-2Zm4 0h3v2h-3v-2Zm4 0h3v2h-3v-2Zm-8.5 4h15v1.5h-15V13.5Z"/></svg>',
}
