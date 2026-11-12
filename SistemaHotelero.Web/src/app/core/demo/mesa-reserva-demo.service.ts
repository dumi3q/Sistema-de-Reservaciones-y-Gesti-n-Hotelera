import { inject, Injectable, signal } from '@angular/core';
import { AuditoriaDemoService } from './auditoria-demo.service';
import { DashboardDemoService } from './dashboard-demo.service';
import { HotelDemoService } from './hotel-demo.service';
import { PerfilDemoService } from './perfil-demo.service';
import { validStayDates } from './reservation-selection';

/** Estados que puede tener una reserva de mesa. */
type TableBookingStatus = 'Confirmada' | 'Atendida' | 'Cancelada';

/** Reserva de mesa para un turno, con el tamaño del grupo y el estado de la reserva. */
interface TableBooking {
  id: string;
  table: string;
  area: string;
  date: string;
  time: string;
  people: number;
  guest: string;
  status: TableBookingStatus;
}

/**
 * Gestiona las reservas de mesa y valida capacidad y turnos. Consulta las mesas de
 * HotelDemoService, el nombre de PerfilDemoService y registra cambios en auditoría.
 */
@Injectable({ providedIn: 'root' })
export class MesaReservaDemoService {
  private readonly hotel = inject(HotelDemoService);
  private readonly audit = inject(AuditoriaDemoService);
  private readonly profile = inject(PerfilDemoService).profile;
  private readonly state = signal<TableBooking[]>([]);
  readonly bookings = this.state.asReadonly();
  readonly referenceDate = inject(DashboardDemoService).referenceDate;
  readonly times = ['12:00', '13:30', '15:00', '16:30', '18:00', '19:30', '21:00'];

  /** Carga las mesas del inventario compartido con Restaurante. */
  load() {
    return this.hotel.loadTables();
  }

  /** Filtra las mesas por capacidad y por reservas que ocupan el mismo turno. */
  available(date: string, time: string, people: number) {
    if (
      date < this.referenceDate ||
      !validStayDates(date, '9999-12-31') ||
      !this.times.includes(time) ||
      !Number.isInteger(people) ||
      people < 1
    ) {
      return [];
    }
    return this.hotel
      .restaurantTables()
      .filter(
        (table) =>
          table.seats >= people &&
          !this.bookings().some(
            (booking) =>
              booking.date === date &&
              booking.time === time &&
              booking.table === table.id &&
              booking.status !== 'Cancelada',
          ),
      );
  }

  /** Crea una reserva de mesa si todavía está libre y registra el evento. */
  create(date: string, time: string, people: number, id: string): string | null {
    const table = this.available(date, time, people).find((item) => item.id === id);
    if (!table) {
      return null;
    }
    const code = 'MESA-' + String(this.bookings().length + 1).padStart(3, '0');
    this.state.update((items) => [
      ...items,
      {
        id: code,
        table: id,
        area: table.area,
        date,
        time,
        people,
        guest: this.profile().name,
        status: 'Confirmada',
      },
    ]);
    this.audit.record(
      'Restaurante',
      'Mesa reservada',
      code,
      id + ' · ' + date + ' ' + time,
      'Huésped de prueba',
    );
    return code;
  }

  /**
   * Marca una reserva como atendida o cancelada según su estado y la vista que realiza el
   * cambio.
   */
  update(id: string, status: 'Atendida' | 'Cancelada', staff: boolean): boolean {
    if (status === 'Atendida' && !staff) {
      return false;
    }
    const booking = this.bookings().find((item) => item.id === id);
    if (!booking || booking.status !== 'Confirmada') {
      return false;
    }
    this.state.update((items) =>
      items.map((item) => (item.id === id ? { ...item, status } : item)),
    );
    this.audit.record(
      'Restaurante',
      'Reserva de mesa actualizada',
      id,
      status,
      staff ? 'Personal de prueba' : 'Huésped de prueba',
    );
    return true;
  }
}
