import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { SolicitudesDemoService } from '../core/demo/solicitudes-demo.service';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Presenta el formulario y el historial del huésped o la cola del personal.
 * SolicitudesDemoService valida y guarda cada cambio de estado.
 */
@Component({
  selector: 'app-solicitudes',
  imports: [RouterLink, Skeleton],
  templateUrl: './solicitudes.html',
  styleUrls: [
    '../shared/explorer.css',
    '../shared/recepcion.css',
    '../shared/operacion.css',
    './solicitudes.css',
  ],
})
export class Solicitudes {
  readonly demo = inject(SolicitudesDemoService);
  readonly staff = inject(ActivatedRoute).snapshot.data['staff'] === true;
  readonly loading = signal(true);
  readonly stay = signal('');
  readonly category = signal('');
  readonly notes = signal('');
  readonly message = signal('');
  readonly filter = signal(this.staff ? 'Abiertas' : 'Todas');
  readonly filters = ['Abiertas', 'Resueltas', 'Todas'];

  /** Solicitudes que corresponden al estado elegido en el filtro. */
  readonly visible = computed(() =>
    this.demo.requests().filter((item) => {
      if (this.filter() === 'Todas') {
        return true;
      }
      if (this.filter() === 'Resueltas') {
        return item.status === 'Resuelta';
      }
      return item.status !== 'Resuelta' && item.status !== 'Cancelada';
    }),
  );

  /** Carga las estadías y elige la primera habitación activa para el formulario. */
  async ngOnInit() {
    await this.demo.load();
    this.stay.set(this.demo.stays()[0]?.id ?? '');
    this.loading.set(false);
  }

  /** Envía la solicitud al servicio y limpia el formulario cuando se guarda. */
  submit() {
    const id = this.demo.create(this.stay(), this.category(), this.notes());
    this.message.set(
      id
        ? 'Solicitud ' + id + ' enviada a recepción.'
        : 'Elige una habitación, un tipo de solicitud y escribe qué necesitas.',
    );
    if (id) {
      this.category.set('');
      this.notes.set('');
      this.filter.set('Todas');
    }
  }

  /** Anuncia si el cambio solicitado se aplicó correctamente. */
  feedback(updated: boolean) {
    this.message.set(
      updated
        ? 'Solicitud actualizada.'
        : 'No se pudo aplicar ese cambio. Revisa el estado y vuelve a intentarlo.',
    );
  }
}
