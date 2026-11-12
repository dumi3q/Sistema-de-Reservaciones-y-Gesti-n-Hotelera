import { Component, computed, inject, input } from '@angular/core';

import { GuestBookingDemoService } from '../../core/demo/guest-booking-demo.service';
import { PerfilDemoService } from '../../core/demo/perfil-demo.service';

const contacts: Record<string, { phone: string; email: string }> = {
  'HUE-001': { phone: '+12025550100', email: 'ana.martinez@example.com' },
  'HUE-002': { phone: '+12025550101', email: 'luis.castro@example.com' },
  'HUE-003': { phone: '+12025550102', email: 'sofia.ramirez@example.com' },
};

/**
 * Presenta los enlaces de teléfono y correo de un huésped. Para la visita de prueba consulta
 * PerfilDemoService; para los ejemplos usa el directorio local.
 */
@Component({
  selector: 'app-contacto-huesped',
  templateUrl: './contacto-huesped.html',
  styleUrl: './contacto-huesped.css',
})
export class ContactoHuesped {
  readonly guestId = input.required<string>();
  readonly reservationId = input('');
  private readonly guest = inject(GuestBookingDemoService);
  private readonly profile = inject(PerfilDemoService).profile;

  /** Datos de contacto correspondientes al identificador del huésped. */
  readonly contact = computed(() => {
    if (this.guestId() !== this.guest.guestId) {
      return contacts[this.guestId()];
    }
    const profile = this.profile();
    return profile.phone || profile.email ? profile : undefined;
  });

  /** Enlace que abre un correo con un asunto acorde con la reserva. */
  readonly emailLink = computed(() => {
    const email = this.contact()?.email;
    const subject = this.reservationId()
      ? 'Confirmación de reserva ' + this.reservationId()
      : 'Contacto de HotelHub';
    return email ? 'mailto:' + email + '?subject=' + encodeURIComponent(subject) : '';
  });
}
