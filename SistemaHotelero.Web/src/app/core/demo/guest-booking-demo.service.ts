import { computed, inject, Injectable } from '@angular/core';
import { DashboardDemoService } from './dashboard-demo.service';
import { PerfilDemoService } from './perfil-demo.service';
import { HotelDemoService } from './hotel-demo.service';
import { DemoReservation, ReservationDraft } from './reservation-demo';

/**
 * Identifica las reservas del huésped de prueba y delega su creación y cancelación en
 * HotelDemoService. Toma el nombre de PerfilDemoService.
 */
@Injectable({ providedIn: 'root' })
export class GuestBookingDemoService {
  private readonly hotel = inject(HotelDemoService);
  readonly guestId = 'HUE-DEMO-VISITA';
  private readonly profile = inject(PerfilDemoService).profile;
  readonly referenceDate = inject(DashboardDemoService).referenceDate;

  /** Reservas que pertenecen al huésped de esta sesión de prueba. */
  readonly reservations = computed(() =>
    this.hotel.reservations().filter((item) => item.guestId === this.guestId),
  );
  readonly loading = this.hotel.reservationsLoading;

  /** Pide a HotelDemoService la carga compartida de las reservas. */
  load(): Promise<void> {
    return this.hotel.loadReservations();
  }

  /**
   * Crea una reserva de prueba con el perfil del huésped si las fechas son válidas para la
   * demo.
   */
  confirm(draft: ReservationDraft): DemoReservation | null {
    if (draft.start < this.referenceDate) {
      return null;
    }
    return this.hotel.createReservation(draft, this.profile().name, this.guestId);
  }

  /** Comprueba que la reserva pertenezca al huésped y siga en un estado cancelable. */
  canCancel(reservation: DemoReservation): boolean {
    return (
      reservation.guestId === this.guestId &&
      reservation.start >= this.referenceDate &&
      (reservation.status === 'Pendiente' || reservation.status === 'Confirmada')
    );
  }

  /**
   * Solicita la cancelación al inventario únicamente si el huésped puede cancelar esa reserva.
   */
  cancel(id: string): boolean {
    const current = this.reservations().find((item) => item.id === id);
    return (
      !!current &&
      this.canCancel(current) &&
      !!this.hotel.updateReservation(id, 'Cancelada', 'Huésped de prueba')
    );
  }

  /** Calcula el subtotal del hospedaje con la tarifa guardada y el número de noches. */
  total(reservation: DemoReservation): number {
    const nights = (Date.parse(reservation.end) - Date.parse(reservation.start)) / 86400000;
    return nights * this.hotel.bookedRoomPrice(reservation);
  }
}
