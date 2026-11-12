/** Datos del edificio que muestra el selector de habitaciones. */
export interface Building {
  code: string;
  name: string;
  description: string;
  floors: number;
}

/** Inventario, capacidad, tarifa y períodos ocupados de una habitación de prueba. */
export interface Room {
  id: string;
  building: string;
  floor: number;
  number: number;
  type: string;
  adults: number;
  children: number;
  capacity: number;
  price: number;
  area: number;
  amenities: string[];
  photos: string[];
  maintenance: boolean;
  stays: { start: string; end: string }[];
}

/** Estados de una mesa durante el servicio del restaurante. */
export type TableStatus =
  'Libre' | 'Por tomar orden' | 'En preparación' | 'Atendida' | 'Por cobrar';

/** Zonas donde se muestran las mesas en el mapa del restaurante. */
export type TableArea = 'Restaurante' | 'Piscina' | 'Bar';

/** Datos y comanda de una mesa que comparten el mapa, Pedidos y Caja. */
export interface RestaurantTable {
  id: string;
  seats: number;
  area: TableArea;
  status: TableStatus;
  guest: string;
  people: number;
  minutes: number;
  order: RestaurantOrderLine[];
}

/**
 * Cantidad y precio de un platillo en la comanda. Las líneas antiguas pueden omitir los datos
 * del catálogo.
 */
export interface RestaurantOrderLine {
  name: string;
  quantity: number;
  menuItemId?: number;
  unitCents?: number;
}
