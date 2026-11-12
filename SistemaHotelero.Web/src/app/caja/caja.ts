import { CurrencyPipe, NgTemplateOutlet } from '@angular/common';
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
import {
  Account,
  CajaDemoService,
  PaymentMethod,
  Receipt,
  totalCents,
} from '../core/demo/caja-demo.service';
import { HotelDemoService } from '../core/demo/hotel-demo.service';
import { normalizeReservationSearch } from '../core/demo/reservation-demo';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Presenta cuentas abiertas y comprobantes de CajaDemoService. Coordina los diálogos de cobro y
 * de traslado a una habitación.
 */
@Component({
  selector: 'app-caja',
  imports: [CurrencyPipe, NgTemplateOutlet, RouterLink, Skeleton],
  templateUrl: './caja.html',
  styleUrls: [
    '../shared/explorer.css',
    '../shared/recepcion.css',
    '../shared/operacion.css',
    './caja.css',
  ],
})
export class Caja {
  private readonly hotel = inject(HotelDemoService);
  private readonly service = inject(CajaDemoService);
  private readonly detector = inject(ChangeDetectorRef);
  private readonly dialog = viewChild.required<ElementRef<HTMLDialogElement>>('paymentDialog');
  readonly accounts = this.service.accounts;
  readonly receipts = this.service.receipts;
  readonly activeStays = this.service.activeStays;
  readonly selectedId = signal(inject(ActivatedRoute).snapshot.queryParamMap.get('cuenta'));

  /** Cuenta abierta que corresponde a la selección actual. */
  readonly selected = computed(() => this.accounts().find((item) => item.id === this.selectedId()));
  readonly receipt = signal<Receipt | null>(null);
  readonly tab = signal<'cuentas' | 'comprobantes'>('cuentas');
  readonly query = signal('');
  readonly loading = signal(true);
  readonly message = signal('');
  readonly pending = signal<Account | null>(null);
  readonly method = signal<PaymentMethod>('Efectivo');
  readonly stayId = signal('');
  readonly modes = ['Efectivo', 'Tarjeta', 'Transferencia'] as const;
  readonly operation = signal<'pago' | 'cargo'>('pago');
  readonly totalCents = totalCents;

  /** Cuentas que coinciden con el nombre, código o título buscado. */
  readonly filtered = computed(() => {
    const query = normalizeReservationSearch(this.query().trim());
    return this.accounts().filter((item) =>
      normalizeReservationSearch(item.title + ' ' + item.guest + ' ' + item.id).includes(query),
    );
  });

  /** Carga las mesas y reservas, y abre la cuenta o el comprobante indicado en la URL. */
  async ngOnInit() {
    await Promise.all([this.hotel.loadTables(), this.hotel.loadReservations()]);
    const requestedId = this.selectedId();
    if (requestedId && !this.selected()) {
      const receipt = this.receipts().find((item) => item.account.id === requestedId);
      if (receipt) {
        this.receipt.set(receipt);
        this.tab.set('comprobantes');
      } else {
        this.message.set('No hay una cuenta abierta ni un comprobante para esa referencia.');
      }
    } else if (!requestedId) {
      this.selectedId.set(this.accounts()[0]?.id ?? null);
    }
    this.loading.set(false);
  }

  /** Cambia entre cuentas y comprobantes y selecciona un comprobante si hace falta. */
  setTab(tab: 'cuentas' | 'comprobantes') {
    this.tab.set(tab);
    if (tab === 'comprobantes' && !this.receipt()) {
      this.receipt.set(this.receipts()[0] ?? null);
    }
  }

  /** Prepara y abre el diálogo de pago o de traslado para la cuenta elegida. */
  open(account: Account, operation: 'pago' | 'cargo') {
    this.pending.set(account);
    this.operation.set(operation);
    this.method.set('Efectivo');
    this.stayId.set('');
    this.detector.detectChanges();
    this.dialog().nativeElement.showModal();
  }

  /** Cambia el método de pago si corresponde a una opción permitida. */
  selectMethod(value: string) {
    const method = this.modes.find((option) => option === value);
    if (method) {
      this.method.set(method);
    }
  }

  /** Registra el pago o el cargo en CajaDemoService y muestra el resultado. */
  confirm() {
    const account = this.pending();
    if (!account) {
      return;
    }
    if (this.operation() === 'pago') {
      const receipt = this.service.pay(account.id, this.method());
      if (!receipt) {
        this.message.set('La cuenta ya no está disponible para cobrar.');
      } else {
        this.receipt.set(receipt);
        this.tab.set('comprobantes');
        this.message.set('Pago de prueba registrado. Comprobante ' + receipt.id + '.');
      }
    } else {
      const transferred = this.service.transferToStay(account.id, this.stayId());
      this.message.set(
        transferred
          ? 'Consumo cargado a la cuenta de la habitación.'
          : 'No se pudo cargar el consumo. Revisa la cuenta y la estadía.',
      );
      if (transferred) {
        this.selectedId.set(this.stayId());
      }
    }
    this.dialog().nativeElement.close();
  }
}
