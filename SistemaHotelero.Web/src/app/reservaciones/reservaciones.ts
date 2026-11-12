import { CurrencyPipe, DatePipe } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  computed,
  ElementRef,
  inject,
  signal,
  viewChild,
} from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HotelDemoService } from '../core/demo/hotel-demo.service';
import {
  DemoReservation,
  ReservationDraft,
  ReservationStatus,
  normalizeReservationSearch,
} from '../core/demo/reservation-demo';

import { readReservationDraft } from '../core/demo/reservation-selection';
import { ContactoHuesped } from '../shared/contacto-huesped/contacto-huesped';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Permite buscar reservas, preparar una nueva desde el selector y actualizar su estado.
 * HotelDemoService conserva los datos de la operación.
 */
@Component({
  selector: 'app-reservaciones',
  imports: [CurrencyPipe, DatePipe, ReactiveFormsModule, RouterLink, Skeleton, ContactoHuesped],
  templateUrl: './reservaciones.html',
  styleUrls: ['../shared/explorer.css', './reservaciones.css'],
})
export class Reservaciones {
  private readonly changeDetector = inject(ChangeDetectorRef);
  private readonly demo = inject(HotelDemoService);
  readonly loading = this.demo.reservationsLoading;

  /** Carga las reservas que se muestran en recepción. */
  ngOnInit() {
    void this.demo.loadReservations();
  }

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly detail = viewChild.required<ElementRef<HTMLDialogElement>>('detail');

  readonly statuses: ReservationStatus[] = [
    'Pendiente',
    'Confirmada',
    'Alojado',
    'Finalizada',
    'Cancelada',
  ];
  readonly summary = [
    { label: 'Total registradas', status: '' },
    { label: 'Por confirmar', status: 'Pendiente' },
    { label: 'Confirmadas', status: 'Confirmada' },
    { label: 'En estadía', status: 'Alojado' },
  ];
  readonly reservations = this.demo.reservations;
  readonly query = signal(this.route.snapshot.queryParamMap.get('buscar') ?? '');
  readonly status = signal('');
  readonly selected = signal<DemoReservation | null>(null);
  readonly cancelRequested = signal(false);
  readonly feedback = signal('');
  readonly error = signal('');
  readonly guest = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.maxLength(100)],
  });

  readonly draft = signal(this.readDraft());

  /** Habitación que corresponde a la selección del borrador. */
  readonly draftRoom = computed(() =>
    this.demo.rooms.find((room) => room.id === this.draft()?.roomId),
  );

  /** Subtotal del borrador con la tarifa cotizada y las noches elegidas. */
  readonly draftTotal = computed(() => {
    const draft = this.draft();
    const room = this.draftRoom();
    if (!draft || !room) {
      return 0;
    }
    return (
      (this.demo.quoteRoomPrice(room, draft.start) *
        (Date.parse(draft.end) - Date.parse(draft.start))) /
      86400000
    );
  });

  /** Reservas que coinciden con el estado y el texto buscado. */
  readonly filtered = computed(() => {
    const query = normalizeReservationSearch(this.query().trim());
    return this.reservations()
      .filter((item) => !this.status() || item.status === this.status())
      .filter((item) =>
        normalizeReservationSearch(`${item.id} ${item.guest} ${item.roomId}`).includes(query),
      )
      .sort(
        (first, second) =>
          first.start.localeCompare(second.start) || first.id.localeCompare(second.id),
      );
  });

  /** Cuenta las reservas de un estado para los indicadores del listado. */
  count(status: string): number {
    return status
      ? this.reservations().filter((item) => item.status === status).length
      : this.reservations().length;
  }

  /** Selecciona una reserva y abre su diálogo de detalle. */
  showDetail(item: DemoReservation) {
    this.selected.set(item);
    this.cancelRequested.set(false);
    this.error.set('');
    this.changeDetector.detectChanges();
    this.detail().nativeElement.showModal();
  }

  /** Cierra el diálogo de la reserva seleccionada. */
  closeDetail() {
    this.detail().nativeElement.close();
  }

  /** Solicita al servicio un cambio de estado y actualiza el detalle mostrado. */
  updateStatus(status: ReservationStatus) {
    const item = this.selected();
    if (!item) {
      return;
    }
    const updated = this.demo.updateReservation(item.id, status);
    if (!updated) {
      this.error.set('Este cambio de estado ya no está disponible.');
      return;
    }
    this.selected.set(updated);
    this.cancelRequested.set(false);
    this.error.set('');
    this.feedback.set(`Se actualizó la reservación ${updated.id} en la demo.`);
  }

  /** Valida el nombre del huésped y guarda la reserva del borrador en HotelDemoService. */
  createReservation() {
    const draft = this.draft();
    const guest = this.guest.value.trim();
    if (!guest || this.guest.invalid) {
      this.guest.markAsTouched();
      return;
    }
    if (!draft) {
      return;
    }
    const created = this.demo.createReservation(draft, guest);
    if (!created) {
      this.error.set('La habitación ya no está disponible para esa selección. Vuelve al selector.');
      return;
    }
    this.query.set('');
    this.status.set('');
    this.feedback.set(`Reservación ${created.id} creada en la demo.`);
    this.clearDraft();
    this.showDetail(created);
  }

  /** Limpia el borrador y retira sus datos de la URL sin saltar al inicio de la página. */
  clearDraft() {
    this.draft.set(null);
    this.error.set('');
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {},
      replaceUrl: true,
      scroll: 'manual',
    });
  }

  /** Lee y valida la selección enviada por el selector de habitaciones. */
  private readDraft(): ReservationDraft | null {
    const draft = readReservationDraft(this.route.snapshot.queryParamMap);
    if (!draft) {
      return null;
    }
    const room = this.demo.rooms.find((item) => item.id === draft.roomId);
    return room && this.demo.isAvailable(room, draft.start, draft.end, draft.guests) ? draft : null;
  }
}
