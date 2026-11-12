import { DatePipe, PercentPipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Arrival, DashboardDemoService } from '../core/demo/dashboard-demo.service';
import { HotelDemoService } from '../core/demo/hotel-demo.service';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Presenta la agenda, el calendario y los indicadores del personal. Combina los movimientos de
 * DashboardDemoService con el inventario de HotelDemoService.
 */
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink, DatePipe, PercentPipe, Skeleton],
  templateUrl: './dashboard.html',
  styleUrls: ['../shared/explorer.css', './dashboard.css', './calendar.css'],
})
export class Dashboard implements OnInit {
  private readonly source = inject(DashboardDemoService);
  readonly hotel = inject(HotelDemoService);
  readonly loading = signal(true);
  readonly error = signal('');
  readonly events = signal<Arrival[]>([]);
  readonly day = signal(this.source.referenceDate);
  readonly month = signal(new Date('2026-12-01T12:00:00Z'));
  readonly selectedKind = signal<'Llegada' | 'Salida'>('Llegada');
  readonly weekdayNames = ['L', 'M', 'X', 'J', 'V', 'S', 'D'];

  /** Nombre del mes que se muestra en el calendario. */
  readonly monthLabel = computed(() =>
    new Intl.DateTimeFormat('es', { month: 'long', year: 'numeric', timeZone: 'UTC' }).format(
      this.month(),
    ),
  );

  /** Movimientos programados para el día seleccionado. */
  readonly dayEvents = computed(() => this.events().filter((event) => event.date === this.day()));

  /** Movimientos del día que coinciden con el filtro de llegadas o salidas. */
  readonly visibleEvents = computed(() =>
    this.dayEvents()
      .filter((event) => event.kind === this.selectedKind())
      .sort((a, b) => a.time.localeCompare(b.time)),
  );

  /** Indicadores de disponibilidad y ocupación estimados para la fecha. */
  readonly availability = computed(() => {
    const start = this.day();
    const date = new Date(start + 'T12:00:00Z');
    date.setUTCDate(date.getUTCDate() + 1);
    const end = date.toISOString().slice(0, 10);
    const maintenance = this.hotel.rooms.filter((room) => room.maintenance).length;
    const unavailable = this.hotel.rooms.filter(
      (room) => !room.maintenance && !this.hotel.isAvailable(room, start, end, 1),
    );
    // Estimación de la demo: parte de las habitaciones no disponibles ya tiene ingreso registrado.
    const occupied = unavailable.filter((_, index) => index % 2 === 0);
    return {
      maintenance,
      reserved: unavailable.length - occupied.length,
      occupied: occupied.length,
      available: this.hotel.rooms.length - maintenance - unavailable.length,
      guests: occupied.reduce((total, room) => total + Math.min(room.adults, 2), 0),
      rate: occupied.length / (this.hotel.rooms.length - maintenance),
    };
  });

  /** Celdas del calendario, incluidas las fechas contiguas y los días con movimientos. */
  readonly calendarDays = computed(() => {
    const month = this.month();
    const first = new Date(Date.UTC(month.getUTCFullYear(), month.getUTCMonth(), 1));
    const offset = (first.getUTCDay() + 6) % 7;
    return Array.from({ length: 42 }, (_, index) => {
      const date = new Date(
        Date.UTC(month.getUTCFullYear(), month.getUTCMonth(), 1 - offset + index),
      );
      const key = date.toISOString().slice(0, 10);
      return {
        key,
        number: date.getUTCDate(),
        inMonth: date.getUTCMonth() === month.getUTCMonth(),
        hasEvents: this.events().some((event) => event.date === key),
      };
    });
  });

  /** Carga la agenda de prueba y muestra un aviso si falla la consulta. */
  async ngOnInit() {
    try {
      this.events.set(await this.source.load());
    } catch {
      this.error.set('No se pudo cargar el resumen. Intenta recargar la página.');
    } finally {
      this.loading.set(false);
    }
  }

  /** Cuenta las llegadas o salidas programadas para el día seleccionado. */
  count(kind: 'Llegada' | 'Salida'): number {
    return this.dayEvents().filter((event) => event.kind === kind).length;
  }

  /** Avanza o retrocede el mes que muestra el calendario. */
  shiftMonth(change: number) {
    const current = this.month();
    this.month.set(new Date(Date.UTC(current.getUTCFullYear(), current.getUTCMonth() + change, 1)));
  }

  /** Selecciona una fecha y actualiza el período mostrado en la agenda. */
  selectDay(key: string) {
    this.day.set(key);
    this.month.set(new Date(key.slice(0, 7) + '-01T12:00:00Z'));
  }

  /** Vuelve a la fecha de referencia de la demo. */
  resetDate() {
    this.day.set(this.source.referenceDate);
    this.month.set(new Date('2026-12-01T12:00:00Z'));
  }

  /** Cuenta las habitaciones disponibles de un edificio para el día seleccionado. */
  availableInBuilding(code: string): number {
    const date = new Date(this.day() + 'T12:00:00Z');
    date.setUTCDate(date.getUTCDate() + 1);
    return this.hotel.rooms.filter(
      (room) =>
        room.building === code &&
        this.hotel.isAvailable(room, this.day(), date.toISOString().slice(0, 10), 1),
    ).length;
  }
}
