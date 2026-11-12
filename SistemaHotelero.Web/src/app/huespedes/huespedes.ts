import { DatePipe } from '@angular/common';
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
import { HotelDemoService } from '../core/demo/hotel-demo.service';
import { DemoReservation, normalizeReservationSearch } from '../core/demo/reservation-demo';

/** Ficha que agrupa las reservas asociadas al mismo huésped. */
interface GuestProfile {
  id: string;
  name: string;
  reservations: DemoReservation[];
}

import { ContactoHuesped } from '../shared/contacto-huesped/contacto-huesped';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Agrupa las reservas de HotelDemoService por huésped para mostrar un directorio y su
 * historial. La ficha delega los datos de contacto en ContactoHuesped.
 */
@Component({
  selector: 'app-huespedes',
  imports: [DatePipe, RouterLink, Skeleton, ContactoHuesped],
  templateUrl: './huespedes.html',
  styleUrls: ['../shared/explorer.css', '../shared/recepcion.css', './huespedes.css'],
})
export class Huespedes {
  private readonly hotel = inject(HotelDemoService);
  readonly loading = this.hotel.reservationsLoading;

  /** Carga las reservas a partir de las cuales se forma el directorio de huéspedes. */
  ngOnInit() {
    void this.hotel.loadReservations();
  }

  private readonly detector = inject(ChangeDetectorRef);
  private readonly profileHeading = viewChild<ElementRef<HTMLElement>>('profileHeading');
  private readonly searchInput = viewChild.required<ElementRef<HTMLInputElement>>('search');

  /** Abre la ficha elegida y dirige el foco a su encabezado. */
  selectGuest(id: string) {
    this.selectedId.set(id);
    this.detector.detectChanges();
    this.profileHeading()?.nativeElement.focus({ preventScroll: true });
  }

  /** Cierra la ficha y devuelve el foco al buscador del directorio. */
  closeProfile() {
    this.selectedId.set(null);
    this.detector.detectChanges();
    this.searchInput().nativeElement.focus({ preventScroll: true });
  }

  readonly selectedId = signal(inject(ActivatedRoute).snapshot.queryParamMap.get('huesped'));
  readonly query = signal('');

  /** Directorio formado al agrupar el historial por identificador de huésped. */
  readonly guests = computed(() => {
    const profiles = new Map<string, GuestProfile>();
    const reservations = [...this.hotel.reservations()].sort((first, second) =>
      second.start.localeCompare(first.start),
    );
    for (const item of reservations) {
      const profile = profiles.get(item.guestId);
      if (profile) {
        profile.reservations.push(item);
      } else {
        profiles.set(item.guestId, { id: item.guestId, name: item.guest, reservations: [item] });
      }
    }
    return [...profiles.values()].sort((first, second) =>
      first.name.localeCompare(second.name, 'es'),
    );
  });

  /** Fichas que coinciden con el nombre, la habitación o la reserva buscados. */
  readonly filtered = computed(() => {
    const query = normalizeReservationSearch(this.query().trim());
    return this.guests().filter(
      (guest) =>
        normalizeReservationSearch(guest.name).includes(query) ||
        guest.reservations.some((item) =>
          normalizeReservationSearch(item.roomId + ' ' + item.id).includes(query),
        ),
    );
  });

  /** Ficha que corresponde a la selección actual. */
  readonly selected = computed(() => this.guests().find((guest) => guest.id === this.selectedId()));
}
