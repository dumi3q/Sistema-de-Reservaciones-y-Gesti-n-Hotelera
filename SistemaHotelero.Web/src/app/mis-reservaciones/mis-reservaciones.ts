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
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CajaDemoService } from '../core/demo/caja-demo.service';
import { GuestBookingDemoService } from '../core/demo/guest-booking-demo.service';
import { DemoReservation } from '../core/demo/reservation-demo';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Presenta las reservas del huésped y coordina su cancelación. Usa GuestBookingDemoService para
 * las reservas y CajaDemoService para enlazar sus comprobantes.
 */
@Component({
  selector: 'app-mis-reservaciones',
  imports: [CurrencyPipe, DatePipe, RouterLink, Skeleton],
  templateUrl: './mis-reservaciones.html',
  styleUrls: [
    '../shared/explorer.css',
    '../shared/recepcion.css',
    '../shared/operacion.css',
    './mis-reservaciones.css',
  ],
})
export class MisReservaciones {
  readonly demo = inject(GuestBookingDemoService);
  private readonly params = inject(ActivatedRoute).snapshot.queryParamMap;
  private readonly detector = inject(ChangeDetectorRef);
  private readonly dialog = viewChild.required<ElementRef<HTMLDialogElement>>('cancelDialog');
  readonly loading = this.demo.loading;
  readonly selectedId = signal(this.params.get('reserva'));

  /** Reserva del huésped cuyo detalle está seleccionado. */
  readonly selected = computed(() =>
    this.demo.reservations().find((item) => item.id === this.selectedId()),
  );
  private readonly caja = inject(CajaDemoService);

  /** Comprobante de hospedaje vinculado a la reserva seleccionada. */
  readonly selectedReceipt = computed(() =>
    this.caja
      .receipts()
      .find((item) => item.account.type === 'Hospedaje' && item.account.id === this.selected()?.id),
  );
  readonly tab = signal<'activas' | 'historial'>('activas');
  readonly pending = signal<DemoReservation | null>(null);
  readonly message = signal('');

  /** Separa las reservas activas de las canceladas o finalizadas. */
  readonly groups = computed(() => {
    const items = [...this.demo.reservations()].sort((first, second) =>
      first.start.localeCompare(second.start),
    );
    return {
      activas: items.filter((item) => item.status !== 'Cancelada' && item.status !== 'Finalizada'),
      historial: items
        .filter((item) => item.status === 'Cancelada' || item.status === 'Finalizada')
        .reverse(),
    };
  });

  /** Carga las reservas y recupera la selección y el aviso indicados en la URL. */
  ngOnInit() {
    void this.demo.load();
    const current = this.selected();
    if (current?.status === 'Confirmada' && this.params.get('nueva') === 'true') {
      this.message.set('Reservación de prueba confirmada. Código ' + current.id + '.');
    }
    if (current?.status === 'Cancelada' || current?.status === 'Finalizada') {
      this.tab.set('historial');
    }
  }

  /**
   * Cambia entre reservas activas e historial y limpia la selección si ya no pertenece a la
   * lista.
   */
  switchTab(tab: 'activas' | 'historial') {
    this.tab.set(tab);
    if (!this.groups()[tab].some((item) => item.id === this.selectedId())) {
      this.selectedId.set(null);
    }
  }

  /** Abre el diálogo de confirmación con la reserva que se desea cancelar. */
  askCancel(item: DemoReservation) {
    this.pending.set(item);
    this.detector.detectChanges();
    this.dialog().nativeElement.showModal();
  }

  /** Solicita la cancelación, cierra el diálogo y muestra el resultado al huésped. */
  cancel() {
    const item = this.pending();
    if (!item) {
      return;
    }
    const cancelled = this.demo.cancel(item.id);
    this.dialog().nativeElement.close();
    if (cancelled) {
      this.tab.set('historial');
    }
    this.message.set(
      cancelled
        ? 'Reservación ' + item.id + ' cancelada.'
        : 'La reservación ya no se puede cancelar.',
    );
  }
}
