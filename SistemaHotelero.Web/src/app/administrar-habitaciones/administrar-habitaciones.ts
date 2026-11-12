import { CurrencyPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Room } from '../core/demo/demo.models';
import { HotelDemoService } from '../core/demo/hotel-demo.service';
import { normalizeReservationSearch } from '../core/demo/reservation-demo';
import { AdminNav } from '../shared/admin-nav/admin-nav';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Edita tarifas base, amenidades, fotos y mantenimiento. Usa HotelDemoService para consultar el
 * inventario y guardar los cambios.
 */
@Component({
  selector: 'app-administrar-habitaciones',
  imports: [CurrencyPipe, ReactiveFormsModule, RouterLink, AdminNav, Skeleton],
  templateUrl: './administrar-habitaciones.html',
  styleUrls: [
    '../shared/explorer.css',
    '../shared/recepcion.css',
    '../shared/operacion.css',
    '../shared/administracion.css',
  ],
})
export class AdministrarHabitaciones {
  private readonly hotel = inject(HotelDemoService);
  readonly buildings = this.hotel.buildings;
  readonly loading = signal(true);
  readonly query = signal('');
  readonly building = signal('');
  readonly selectedId = signal('');
  readonly error = signal('');
  readonly notice = signal('');

  /** Habitación que corresponde a la selección del editor. */
  readonly selected = computed(() =>
    this.hotel.rooms.find((room) => room.id === this.selectedId()),
  );

  /** Habitaciones que coinciden con el edificio y el texto buscado. */
  readonly filtered = computed(() => {
    const query = normalizeReservationSearch(this.query().trim());
    return this.hotel.rooms.filter(
      (room) =>
        (!this.building() || this.building() === room.building) &&
        normalizeReservationSearch(room.id + ' ' + room.type).includes(query),
    );
  });

  readonly form = new FormGroup({
    price: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0.01), Validators.max(100000)],
    }),
    amenities: new FormControl('', { nonNullable: true }),
    photos: new FormControl('', { nonNullable: true }),
    maintenance: new FormControl(false, { nonNullable: true }),
  });

  /** Carga el inventario de habitaciones al entrar en la pantalla. */
  async ngOnInit() {
    await this.hotel.loadRooms();
    this.loading.set(false);
  }

  /** Selecciona una habitación y copia sus valores actuales al formulario. */
  edit(room: Room) {
    this.selectedId.set(room.id);
    this.form.reset({
      price: room.price,
      amenities: room.amenities.join('\n'),
      photos: room.photos.join('\n'),
      maintenance: room.maintenance,
    });
    this.error.set('');
    this.notice.set('');
  }

  /** Valida el formulario y envía los cambios a HotelDemoService. */
  save() {
    const room = this.selected();
    if (!room) {
      return;
    }
    this.form.markAllAsTouched();
    this.notice.set('');
    if (this.form.invalid) {
      this.error.set('Revisa la tarifa por noche.');
      return;
    }
    const { price, amenities, photos, maintenance } = this.form.getRawValue();
    const error = this.hotel.updateRoom(room.id, {
      price,
      amenities: this.lines(amenities),
      photos: this.lines(photos),
      maintenance,
    });
    this.error.set(error ?? '');
    if (!error) {
      this.form.markAsPristine();
      this.notice.set('Cambios guardados en la habitación ' + room.id + '.');
    }
  }

  /** Convierte las líneas de texto en una lista sin entradas vacías ni repetidas. */
  private lines(value: string): string[] {
    return [
      ...new Set(
        value
          .split('\n')
          .map((line) => line.trim())
          .filter(Boolean),
      ),
    ];
  }
}
