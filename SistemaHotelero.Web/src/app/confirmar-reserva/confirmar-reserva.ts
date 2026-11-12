import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ConfiguracionDemoService } from '../core/demo/configuracion-demo.service';
import { GuestBookingDemoService } from '../core/demo/guest-booking-demo.service';
import { HotelDemoService } from '../core/demo/hotel-demo.service';
import { readReservationDraft, reservationParams } from '../core/demo/reservation-selection';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Resume la habitación, las fechas y los importes antes del pago. Consulta HotelDemoService y
 * ConfiguracionDemoService, y conserva la selección al navegar.
 */
@Component({
  selector: 'app-confirmar-reserva',
  imports: [CurrencyPipe, DatePipe, RouterLink, Skeleton],
  templateUrl: './confirmar-reserva.html',
  styleUrls: ['../shared/explorer.css', '../shared/recepcion.css', './confirmar-reserva.css'],
})
export class ConfirmarReserva {
  private readonly configuration = inject(ConfiguracionDemoService);
  readonly settings = this.configuration.settings;

  /** Anticipo estimado según la tarifa cotizada y la configuración. */
  readonly deposit = computed(() =>
    this.configuration.estimateDeposit(this.nightly(), this.nights),
  );
  private readonly hotel = inject(HotelDemoService);
  private readonly guest = inject(GuestBookingDemoService);
  readonly draft = readReservationDraft(inject(ActivatedRoute).snapshot.queryParamMap);
  readonly params = this.draft ? reservationParams(this.draft) : {};
  readonly loading = this.guest.loading;

  /** Habitación indicada en los datos de la reserva. */
  readonly room = computed(() => this.hotel.rooms.find((item) => item.id === this.draft?.roomId));
  readonly nights = this.draft
    ? (Date.parse(this.draft.end) - Date.parse(this.draft.start)) / 86400000
    : 0;

  /** Enlace de correo con la selección incluida en el borrador. */
  readonly advisorLink = computed(() => {
    const room = this.room();
    const draft = this.draft;
    if (!room || !draft) {
      return '';
    }

    const subject = 'Consulta de reserva · HotelHub';
    const body = [
      'Hola, quisiera ayuda para reservar esta estadía:',
      '',
      'Habitación: ' + room.id + ' · ' + room.type,
      'Entrada: ' + draft.start.split('-').reverse().join('/'),
      'Salida: ' + draft.end.split('-').reverse().join('/'),
      'Adultos: ' + draft.guests,
      '',
      '¿Podrían confirmarme la disponibilidad y los pasos para reservar? Gracias.',
    ].join('\r\n');

    return (
      'mailto:reservas@hotelhub.example?subject=' +
      encodeURIComponent(subject) +
      '&body=' +
      encodeURIComponent(body)
    );
  });

  /** Precio cotizado por noche para la fecha de entrada. */
  readonly nightly = computed(() => {
    const room = this.room();
    return room && this.draft ? this.hotel.quoteRoomPrice(room, this.draft.start) : 0;
  });

  /** Indica si la habitación se puede reservar con la selección actual. */
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

  /** Solicita la carga de reservas para mostrar la disponibilidad de la selección. */
  ngOnInit() {
    void this.guest.load();
  }
}
