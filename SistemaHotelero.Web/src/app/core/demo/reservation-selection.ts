import { ParamMap } from '@angular/router';
import { ReservationDraft } from './reservation-demo';

/** Valida el formato y comprueba que la fecha de salida sea posterior a la de entrada. */
export function validStayDates(start: string, end: string): boolean {
  /** Comprueba que el texto represente una fecha real con el formato año, mes y día. */
  const valid = (date: string) =>
    /^\d{4}-\d{2}-\d{2}$/.test(date) &&
    Number.isFinite(Date.parse(date)) &&
    new Date(date).toISOString().slice(0, 10) === date;
  return valid(start) && valid(end) && start < end;
}

/**
 * Extrae la habitación, las fechas y los adultos de la URL y descarta selecciones inválidas.
 */
export function readReservationDraft(params: ParamMap): ReservationDraft | null {
  const roomId = params.get('habitacion');
  const start = params.get('entrada');
  const end = params.get('salida');
  const guests = Number(params.get('adultos'));
  if (
    !roomId ||
    !start ||
    !end ||
    !validStayDates(start, end) ||
    !Number.isInteger(guests) ||
    guests < 1 ||
    guests > 4
  ) {
    return null;
  }
  return { roomId, start, end, guests };
}

/** Convierte el borrador en parámetros de navegación que comparten las pantallas de reserva. */
export function reservationParams(draft: ReservationDraft) {
  return {
    habitacion: draft.roomId,
    entrada: draft.start,
    salida: draft.end,
    adultos: draft.guests,
  };
}
