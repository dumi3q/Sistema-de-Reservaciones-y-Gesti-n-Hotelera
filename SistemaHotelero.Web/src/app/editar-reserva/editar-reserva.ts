import { CurrencyPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { HotelDemoService } from '../core/demo/hotel-demo.service';

/**
 * Permite al personal cambiar fechas, ocupantes y habitación. Pide a HotelDemoService que
 * verifique la disponibilidad y aplique la edición.
 */
@Component({
  selector: 'app-editar-reserva',
  imports: [CurrencyPipe, RouterLink],
  templateUrl: './editar-reserva.html',
  styleUrls: ['../shared/explorer.css', '../shared/recepcion.css', '../shared/administracion.css'],
})
export class EditarReserva {
  private readonly hotel = inject(HotelDemoService);
  readonly id = inject(ActivatedRoute).snapshot.paramMap.get('id') ?? '';

  /** Reserva cuyo código viene en la ruta. */
  readonly reservation = computed(() =>
    this.hotel.reservations().find((item) => item.id === this.id),
  );

  /** Indica si el estado permite cambiar la reserva. */
  readonly editable = computed(() =>
    ['Pendiente', 'Confirmada'].includes(this.reservation()?.status ?? ''),
  );
  readonly start = signal(this.reservation()?.start ?? '');
  readonly end = signal(this.reservation()?.end ?? '');
  readonly guests = signal(this.reservation()?.guests ?? 1);
  readonly roomId = signal(this.reservation()?.roomId ?? '');
  readonly message = signal('');
  readonly error = signal('');

  /** Habitaciones que aceptan las fechas y los adultos ingresados. */
  readonly available = computed(() =>
    this.hotel.rooms.filter((room) =>
      this.hotel.isAvailable(room, this.start(), this.end(), this.guests(), this.id),
    ),
  );

  /** Precio por noche para la selección de edición. */
  readonly price = computed(() => {
    const booking = this.reservation();
    const room = this.available().find((item) => item.id === this.roomId());
    return booking && room
      ? this.hotel.rebookingPrice(booking, room, this.start(), this.end())
      : null;
  });

  /** Subtotal estimado con la tarifa y las noches seleccionadas. */
  readonly total = computed(() => {
    const nights = (Date.parse(this.end()) - Date.parse(this.start())) / 86400000;
    return (this.price() ?? 0) * nights;
  });

  /**
   * Envía las fechas, la ocupación y la habitación a HotelDemoService y muestra el resultado.
   */
  save() {
    this.message.set('');
    const updated = this.hotel.editReservation(this.id, {
      start: this.start(),
      end: this.end(),
      guests: this.guests(),
      roomId: this.roomId(),
    });
    this.error.set(
      updated ? '' : 'Revisa las fechas, la capacidad y la disponibilidad de la habitación.',
    );
    if (updated) {
      this.message.set('Cambios guardados en la reserva ' + updated.id + '.');
    }
  }
}
