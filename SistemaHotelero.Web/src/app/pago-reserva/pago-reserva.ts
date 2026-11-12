import { CurrencyPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ConfiguracionDemoService } from '../core/demo/configuracion-demo.service';
import { GuestBookingDemoService } from '../core/demo/guest-booking-demo.service';
import { HotelDemoService } from '../core/demo/hotel-demo.service';
import { readReservationDraft, reservationParams } from '../core/demo/reservation-selection';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Simula el paso de pago con tarjetas de ejemplo. Consulta tarifas y anticipos, y confirma la
 * reserva a través de GuestBookingDemoService.
 */
@Component({
  selector: 'app-pago-reserva',
  imports: [CurrencyPipe, RouterLink, Skeleton],
  templateUrl: './pago-reserva.html',
  styleUrls: ['../shared/explorer.css', '../shared/recepcion.css', './pago-reserva.css'],
})
export class PagoReserva {
  private readonly hotel = inject(HotelDemoService);
  private readonly guest = inject(GuestBookingDemoService);
  private readonly router = inject(Router);
  private readonly configuration = inject(ConfiguracionDemoService);
  readonly draft = readReservationDraft(inject(ActivatedRoute).snapshot.queryParamMap);
  readonly params = this.draft ? reservationParams(this.draft) : {};
  readonly loading = this.guest.loading;

  /** Habitación indicada en la selección recibida desde el resumen. */
  readonly room = computed(() => this.hotel.rooms.find((room) => room.id === this.draft?.roomId));

  /** Indica si la habitación sigue disponible para las fechas elegidas. */
  readonly available = computed(() => {
    const room = this.room();
    const draft = this.draft;
    return (
      !!room &&
      !!draft &&
      draft.start >= this.guest.referenceDate &&
      this.hotel.isAvailable(room, draft.start, draft.end, draft.guests)
    );
  });

  /** Subtotal cotizado de hospedaje para todas las noches. */
  readonly total = computed(() => {
    if (!this.draft) {
      return 0;
    }
    const nights = (Date.parse(this.draft.end) - Date.parse(this.draft.start)) / 86400000;
    const room = this.room();
    return room ? this.hotel.quoteRoomPrice(room, this.draft.start) * nights : 0;
  });

  /** Anticipo simulado según la regla y la tarifa vigentes. */
  readonly deposit = computed(() => {
    const room = this.room();
    if (!room || !this.draft) {
      return 0;
    }
    const nights = (Date.parse(this.draft.end) - Date.parse(this.draft.start)) / 86400000;
    return this.configuration.estimateDeposit(
      this.hotel.quoteRoomPrice(room, this.draft.start),
      nights,
    );
  });

  readonly examples = [
    { brand: 'Visa', number: '4242424242424242' },
    { brand: 'Mastercard', number: '5555555555554444' },
    { brand: 'American Express', number: '378282246310005' },
  ];
  readonly number = signal('');
  readonly holder = signal('');
  readonly expiry = signal('');
  readonly code = signal('');
  readonly saving = signal(false);
  readonly error = signal('');

  /** Marca de tarjeta estimada por los primeros dígitos ingresados. */
  readonly brand = computed(() => {
    const digits = this.number();
    if (/^4/.test(digits)) {
      return 'Visa';
    }
    if (/^3[47]/.test(digits)) {
      return 'American Express';
    }
    const prefix = Number(digits.slice(0, 4));
    if (/^5[1-5]/.test(digits) || (digits.length >= 4 && prefix >= 2221 && prefix <= 2720)) {
      return 'Mastercard';
    }
    return 'Tarjeta';
  });

  /** Carga las reservas para comprobar que la selección aún esté disponible. */
  ngOnInit() {
    void this.guest.load();
  }

  /** Extrae los dígitos ingresados y actualiza el formato mostrado en el campo. */
  setNumber(input: HTMLInputElement) {
    const digits = input.value.replace(/\D/g, '').slice(0, 19);
    this.number.set(digits);
    input.value = this.formattedNumber();
  }

  /** Agrupa el número según la marca de tarjeta detectada. */
  formattedNumber(): string {
    const digits = this.number();
    return this.brand() === 'American Express'
      ? [digits.slice(0, 4), digits.slice(4, 10), digits.slice(10)].filter(Boolean).join(' ')
      : (digits.match(/.{1,4}/g) ?? []).join(' ');
  }

  /** Rellena el formulario con los datos ficticios de la tarjeta elegida. */
  useExample(number: string) {
    this.number.set(number);
    this.holder.set('Huésped de prueba');
    this.expiry.set('12/30');
    this.code.set(this.brand() === 'American Express' ? '1234' : '123');
    this.error.set('');
  }

  /** Valida los datos de prueba, confirma la reserva y abre el historial del huésped. */
  async submit() {
    if (!this.draft || !this.available() || this.saving()) {
      return;
    }
    if (
      !this.examples.some((item) => item.number === this.number()) ||
      !this.holder().trim() ||
      !/^(0[1-9]|1[0-2])\/\d{2}$/.test(this.expiry()) ||
      !(this.brand() === 'American Express' ? /^\d{4}$/ : /^\d{3}$/).test(this.code())
    ) {
      this.error.set('Completa los campos con una tarjeta de ejemplo para continuar.');
      return;
    }

    this.saving.set(true);
    const reservation = this.guest.confirm(this.draft);
    if (!reservation) {
      this.error.set('La habitación ya no está disponible para esas fechas.');
      this.saving.set(false);
      return;
    }

    this.number.set('');
    this.holder.set('');
    this.expiry.set('');
    this.code.set('');
    await this.router.navigate(['/visitar/mis-reservaciones'], {
      queryParams: { reserva: reservation.id, nueva: true },
      replaceUrl: true,
    });
  }
}
