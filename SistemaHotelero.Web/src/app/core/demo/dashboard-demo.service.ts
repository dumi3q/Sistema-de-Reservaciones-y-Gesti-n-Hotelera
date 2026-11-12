import { Injectable } from '@angular/core';
import { HotelDemoService } from './hotel-demo.service';

/** Movimiento de entrada o salida mostrado en la agenda de ejemplo. */
export interface Arrival {
  id: string;
  date: string;
  guest: string;
  room: string;
  time: string;
  people: number;
  kind: 'Llegada' | 'Salida';
}

/**
 * Proporciona la fecha de referencia y los movimientos ficticios que utiliza el panel del
 * personal.
 */
@Injectable({ providedIn: 'root' })
export class DashboardDemoService {
  readonly referenceDate = '2026-12-10';
  readonly events: Arrival[] = [
    {
      id: '1',
      date: '2026-12-10',
      guest: 'Ana Martínez',
      room: 'A-102',
      time: '14:00',
      people: 2,
      kind: 'Llegada',
    },
    {
      id: '2',
      date: '2026-12-10',
      guest: 'Luis Castro',
      room: 'B-204',
      time: '14:30',
      people: 2,
      kind: 'Llegada',
    },
    {
      id: '3',
      date: '2026-12-10',
      guest: 'Sofía Ramírez',
      room: 'C-103',
      time: '15:00',
      people: 4,
      kind: 'Llegada',
    },
    {
      id: '4',
      date: '2026-12-10',
      guest: 'Carlos Vega',
      room: 'D-401',
      time: '16:30',
      people: 1,
      kind: 'Llegada',
    },
    {
      id: '5',
      date: '2026-12-10',
      guest: 'Lucía Rojas',
      room: 'A-304',
      time: '11:00',
      people: 2,
      kind: 'Salida',
    },
    {
      id: '6',
      date: '2026-12-10',
      guest: 'Diego Mora',
      room: 'C-202',
      time: '12:00',
      people: 2,
      kind: 'Salida',
    },
    {
      id: '7',
      date: '2026-12-11',
      guest: 'María Solís',
      room: 'B-103',
      time: '14:00',
      people: 3,
      kind: 'Llegada',
    },
    {
      id: '8',
      date: '2026-12-11',
      guest: 'Pablo Jiménez',
      room: 'D-304',
      time: '15:00',
      people: 2,
      kind: 'Llegada',
    },
    {
      id: '9',
      date: '2026-12-12',
      guest: 'Elena Vargas',
      room: 'A-604',
      time: '15:00',
      people: 4,
      kind: 'Llegada',
    },
    {
      id: '10',
      date: '2026-12-15',
      guest: 'Andrés Soto',
      room: 'C-203',
      time: '14:00',
      people: 3,
      kind: 'Llegada',
    },
  ];

  /** Entrega la agenda ficticia tras una pausa que simula la carga inicial. */
  async load(): Promise<Arrival[]> {
    await new Promise((resolve) => setTimeout(resolve, 600));
    return this.events;
  }
}
