import { DatePipe } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  computed,
  ElementRef,
  inject,
  signal,
  viewChild,
} from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DashboardDemoService } from '../core/demo/dashboard-demo.service';
import { HotelDemoService } from '../core/demo/hotel-demo.service';
import { DemoReservation, normalizeReservationSearch } from '../core/demo/reservation-demo';

/** Listas entre las que recepción puede alternar en la pantalla de estadías. */
type StayTab = 'llegadas' | 'alojados' | 'salidas';

import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Organiza llegadas, huéspedes alojados y salidas. Actualiza los estados de las reservas a
 * través de HotelDemoService.
 */
@Component({
  selector: 'app-estadias',
  imports: [DatePipe, RouterLink, Skeleton],
  templateUrl: './estadias.html',
  styleUrls: ['../shared/explorer.css', '../shared/recepcion.css', './estadias.css'],
})
export class Estadias {
  private readonly hotel = inject(HotelDemoService);
  readonly loading = this.hotel.reservationsLoading;

  /** Carga las reservas que alimentan las listas de recepción. */
  ngOnInit() {
    void this.hotel.loadReservations();
  }

  private readonly detector = inject(ChangeDetectorRef);
  private readonly checkoutDialog =
    viewChild.required<ElementRef<HTMLDialogElement>>('checkoutDialog');
  readonly query = signal('');
  readonly referenceDate = inject(DashboardDemoService).referenceDate;
  private readonly params = inject(ActivatedRoute).snapshot.queryParamMap;

  readonly date = signal(this.params.get('fecha') || this.referenceDate);
  readonly tab = signal<StayTab>(this.params.get('vista') === 'salidas' ? 'salidas' : 'llegadas');
  readonly message = signal('');
  readonly pending = signal<DemoReservation | null>(null);

  /** Distribuye las reservas entre llegadas, alojados y salidas según la fecha. */
  readonly groups = computed(() => {
    const reservations = this.hotel.reservations();
    const date = this.date();
    return {
      llegadas: reservations.filter(
        (item) =>
          item.start === date && (item.status === 'Confirmada' || item.status === 'Pendiente'),
      ),
      alojados: reservations.filter((item) => item.status === 'Alojado'),
      salidas: reservations.filter(
        (item) => item.end === date && (item.status === 'Alojado' || item.status === 'Finalizada'),
      ),
    };
  });

  /** Filas de la pestaña que coinciden con la búsqueda. */
  readonly rows = computed(() => {
    const query = normalizeReservationSearch(this.query().trim());
    return this.groups()[this.tab()].filter((item) =>
      normalizeReservationSearch(item.guest + ' ' + item.roomId + ' ' + item.id).includes(query),
    );
  });

  /** Aplica la fecha de consulta o recupera la fecha de referencia si se borra. */
  changeDate(date: string) {
    this.date.set(date || this.referenceDate);
  }

  /** Abre la confirmación de salida con los datos de la reserva seleccionada. */
  confirmCheckout(item: DemoReservation) {
    this.pending.set(item);
    this.detector.detectChanges();
    this.checkoutDialog().nativeElement.showModal();
  }

  /** Cambia la lista activa y limpia una posible selección de salida. */
  select(tab: StayTab) {
    this.tab.set(tab);
    this.pending.set(null);
  }

  /** Registra el ingreso o la salida en HotelDemoService y actualiza el aviso. */
  register(item: DemoReservation) {
    const next = item.status === 'Confirmada' ? 'Alojado' : 'Finalizada';
    const updated = this.hotel.updateReservation(item.id, next);
    const dialog = this.checkoutDialog().nativeElement;
    if (dialog.open) {
      dialog.close();
    }
    this.pending.set(null);
    this.message.set(
      updated
        ? (next === 'Alojado' ? 'Check-in' : 'Check-out') + ' registrado para ' + item.guest + '.'
        : 'El estado de la reservación cambió. Revisa la lista e inténtalo de nuevo.',
    );
  }
}
