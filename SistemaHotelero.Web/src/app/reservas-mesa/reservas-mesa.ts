import { DatePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MesaReservaDemoService } from '../core/demo/mesa-reserva-demo.service';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Presenta la búsqueda y el historial de mesas para el huésped, o la agenda para el personal.
 * MesaReservaDemoService controla la disponibilidad y los estados.
 */
@Component({
  selector: 'app-reservas-mesa',
  imports: [DatePipe, RouterLink, Skeleton],
  templateUrl: './reservas-mesa.html',
  styleUrls: [
    '../shared/explorer.css',
    '../shared/recepcion.css',
    '../shared/operacion.css',
    '../shared/administracion.css',
    './reservas-mesa.css',
  ],
})
export class ReservasMesa {
  readonly demo = inject(MesaReservaDemoService);
  readonly staff = inject(ActivatedRoute).snapshot.data['staff'] === true;
  readonly date = signal(this.demo.referenceDate);
  readonly time = signal('18:00');
  readonly people = signal(2);
  readonly searched = signal(false);
  readonly loading = signal(true);
  readonly message = signal('');
  readonly error = signal('');
  readonly selected = signal('');

  /** Mesas que admiten el turno y el tamaño del grupo. */
  readonly available = computed(() => this.demo.available(this.date(), this.time(), this.people()));

  /** Reservas visibles en el historial o en la agenda filtrada por fecha. */
  readonly visible = computed(() =>
    this.demo
      .bookings()
      .filter((item) => !this.staff || !this.date() || item.date === this.date())
      .sort((first, second) => (first.date + first.time).localeCompare(second.date + second.time)),
  );

  /** Carga el inventario de mesas antes de habilitar la búsqueda. */
  async ngOnInit() {
    await this.demo.load();
    this.loading.set(false);
  }

  /** Limpia los resultados, la mesa elegida y los avisos al cambiar la búsqueda. */
  resetSearch() {
    this.searched.set(false);
    this.selected.set('');
    this.message.set('');
    this.error.set('');
  }

  /** Envía la selección a MesaReservaDemoService y muestra el código si se guardó. */
  reserve() {
    if (!this.searched()) {
      return;
    }
    const id = this.demo.create(this.date(), this.time(), this.people(), this.selected());
    if (!id) {
      this.error.set('Selecciona una mesa disponible.');
      return;
    }
    this.resetSearch();
    this.message.set('Mesa reservada. Código ' + id + '.');
  }

  /** Solicita el cambio de estado de una reserva de mesa y anuncia el resultado. */
  update(id: string, status: 'Atendida' | 'Cancelada') {
    this.error.set('');
    if (this.demo.update(id, status, this.staff)) {
      this.message.set('Reserva ' + id + ' · ' + status.toLowerCase() + '.');
    } else {
      this.error.set('La reserva ya no admite ese cambio.');
    }
  }
}
