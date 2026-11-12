import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { CajaDemoService } from '../core/demo/caja-demo.service';
import { GuestBookingDemoService } from '../core/demo/guest-booking-demo.service';

/**
 * Muestra los comprobantes de CajaDemoService. En la vista pública solo incluye los de
 * hospedaje vinculados a reservas de GuestBookingDemoService.
 */
@Component({
  selector: 'app-facturas',
  imports: [CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './facturas.html',
  styleUrls: ['../shared/explorer.css', '../shared/recepcion.css', './facturas.css'],
})
export class Facturas {
  private readonly route = inject(ActivatedRoute);
  private readonly caja = inject(CajaDemoService);
  private readonly guest = inject(GuestBookingDemoService);
  private readonly params = toSignal(this.route.paramMap, {
    initialValue: this.route.snapshot.paramMap,
  });
  readonly staff = this.route.snapshot.data['staff'] === true;

  /** Código del comprobante indicado en la ruta. */
  readonly id = computed(() => this.params().get('id'));

  /** Comprobantes visibles según se esté en Caja o en la vista del huésped. */
  readonly receipts = computed(() => {
    const own = new Set(this.guest.reservations().map((item) => item.id));
    return this.caja
      .receipts()
      .filter(
        (item) => this.staff || (item.account.type === 'Hospedaje' && own.has(item.account.id)),
      );
  });

  /** Comprobante visible cuyo código coincide con la ruta. */
  readonly receipt = computed(() => this.receipts().find((item) => item.id === this.id()));

  /** Abre el cuadro de impresión del navegador para el comprobante mostrado. */
  print() {
    window.print();
  }
}
