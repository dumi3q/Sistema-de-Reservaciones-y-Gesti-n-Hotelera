/** Estados que puede tener una reserva durante su paso por recepción. */
export type ReservationStatus = 'Pendiente' | 'Confirmada' | 'Alojado' | 'Finalizada' | 'Cancelada';

/** Selección de habitación, fechas y ocupantes antes de guardar una reserva. */
export interface ReservationDraft {
  roomId: string;
  start: string;
  end: string;
  guests: number;
}

/** Reserva guardada que comparten las vistas del huésped y del personal. */
export interface DemoReservation extends ReservationDraft {
  id: string;
  guest: string;
  guestId: string;
  nightlyPrice?: number;
  status: ReservationStatus;
}

/** Reservas iniciales que comparten el panel del personal y el cálculo de disponibilidad. */
export const demoReservations: DemoReservation[] = [
  {
    id: 'RES-001',
    guestId: 'HUE-001',
    guest: 'Ana Martínez',
    roomId: 'A-102',
    start: '2026-12-10',
    end: '2026-12-13',
    guests: 2,
    status: 'Confirmada',
  },
  {
    id: 'RES-002',
    guestId: 'HUE-002',
    guest: 'Luis Castro',
    roomId: 'B-204',
    start: '2026-12-10',
    end: '2026-12-14',
    guests: 2,
    status: 'Confirmada',
  },
  {
    id: 'RES-003',
    guestId: 'HUE-003',
    guest: 'Sofía Ramírez',
    roomId: 'C-103',
    start: '2026-12-10',
    end: '2026-12-12',
    guests: 4,
    status: 'Confirmada',
  },
  {
    id: 'RES-004',
    guestId: 'HUE-004',
    guest: 'Carlos Vega',
    roomId: 'D-401',
    start: '2026-12-10',
    end: '2026-12-12',
    guests: 1,
    status: 'Pendiente',
  },
  {
    id: 'RES-005',
    guestId: 'HUE-005',
    guest: 'Lucía Rojas',
    roomId: 'A-304',
    start: '2026-12-08',
    end: '2026-12-10',
    guests: 2,
    status: 'Alojado',
  },
  {
    id: 'RES-006',
    guestId: 'HUE-006',
    guest: 'Diego Mora',
    roomId: 'C-202',
    start: '2026-12-08',
    end: '2026-12-10',
    guests: 2,
    status: 'Alojado',
  },
  {
    id: 'RES-007',
    guestId: 'HUE-007',
    guest: 'María Solís',
    roomId: 'B-103',
    start: '2026-12-11',
    end: '2026-12-14',
    guests: 3,
    status: 'Pendiente',
  },
  {
    id: 'RES-008',
    guestId: 'HUE-008',
    guest: 'Pablo Jiménez',
    roomId: 'D-304',
    start: '2026-12-11',
    end: '2026-12-13',
    guests: 2,
    status: 'Confirmada',
  },
  {
    id: 'RES-009',
    guestId: 'HUE-009',
    guest: 'Elena Vargas',
    roomId: 'A-604',
    start: '2026-12-12',
    end: '2026-12-15',
    guests: 4,
    status: 'Confirmada',
  },
  {
    id: 'RES-010',
    guestId: 'HUE-010',
    guest: 'Andrés Soto',
    roomId: 'C-203',
    start: '2026-12-15',
    end: '2026-12-17',
    guests: 3,
    status: 'Confirmada',
  },
  {
    id: 'RES-011',
    guestId: 'HUE-011',
    guest: 'Daniela Ruiz',
    roomId: 'A-203',
    start: '2026-12-12',
    end: '2026-12-14',
    guests: 2,
    status: 'Cancelada',
  },
  {
    id: 'RES-012',
    guestId: 'HUE-001',
    guest: 'Ana Martínez',
    roomId: 'A-102',
    start: '2026-12-01',
    end: '2026-12-04',
    guests: 2,
    status: 'Finalizada',
  },
];

/** Normaliza texto para comparar nombres y códigos sin distinguir mayúsculas ni acentos. */
export function normalizeReservationSearch(text: string): string {
  return text
    .toLowerCase()
    .normalize('NFD')
    .replace(/\p{Diacritic}/gu, '');
}

/** Cambios de estado que HotelDemoService permite aplicar a una reserva. */
export const reservationTransitions: Record<ReservationStatus, readonly ReservationStatus[]> = {
  Pendiente: ['Confirmada', 'Cancelada'],
  Confirmada: ['Alojado', 'Cancelada'],
  Alojado: ['Finalizada'],
  Finalizada: [],
  Cancelada: [],
};
