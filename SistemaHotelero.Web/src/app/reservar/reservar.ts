import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CurrencyPipe } from '@angular/common';
import { Component, computed, ElementRef, inject, OnInit, signal, viewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HotelDemoService } from '../core/demo/hotel-demo.service';
import {
  readReservationDraft,
  reservationParams,
  validStayDates,
} from '../core/demo/reservation-selection';
import { Building, Room } from '../core/demo/demo.models';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Permite comparar disponibilidad por fecha, edificio y piso. Consulta HotelDemoService y envía
 * la selección al resumen público o al formulario del personal.
 */
@Component({
  selector: 'app-reservar',
  standalone: true,
  imports: [CurrencyPipe, ReactiveFormsModule, RouterLink, Skeleton],
  templateUrl: './reservar.html',
  styleUrls: ['../shared/explorer.css', './reservar.css', './room-detail.css'],
})
export class Reservar implements OnInit {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  readonly publicMode = this.router.url.startsWith('/visitar');
  readonly demo = inject(HotelDemoService);
  readonly buildings = this.demo.buildings;
  readonly floors = [1, 2, 3, 4, 5, 6];
  readonly loading = signal(true);
  readonly rooms = signal<Room[]>([]);
  readonly building = signal('A');
  readonly floor = signal(1);
  readonly selectedRoomId = signal<string | null>(null);
  readonly photoIndex = signal(0);
  readonly error = signal('');
  readonly searched = signal(false);
  private readonly roomDetail = viewChild.required<ElementRef<HTMLElement>>('roomDetail');
  readonly dates = signal({ start: '2026-12-10', end: '2026-12-12', guests: 1 });
  readonly filterForm = new FormGroup({
    start: new FormControl('2026-12-10', { nonNullable: true, validators: [Validators.required] }),
    end: new FormControl('2026-12-12', { nonNullable: true, validators: [Validators.required] }),
    guests: new FormControl(1, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(1), Validators.max(4)],
    }),
  });

  /** Edificio que se está mostrando en el selector. */
  readonly currentBuilding = computed(() =>
    this.buildings.find((item) => item.code === this.building())!,
  );

  /** Habitaciones del edificio y piso seleccionados. */
  readonly floorRooms = computed(() =>
    this.rooms().filter((room) => room.building === this.building() && room.floor === this.floor()),
  );

  /** Habitación cuyo detalle se está mostrando. */
  readonly selectedRoom = computed(
    () => this.floorRooms().find((room) => room.id === this.selectedRoomId()) ?? null,
  );

  /** Cantidad de habitaciones disponibles en todo el hotel. */
  readonly totalAvailable = computed(
    () => this.rooms().filter((room) => this.available(room)).length,
  );

  /** Cantidad de noches entre la entrada y la salida elegidas. */
  readonly nights = computed(() =>
    Math.round((Date.parse(this.dates().end) - Date.parse(this.dates().start)) / 86400000),
  );

  /** Carga las habitaciones y recupera la selección anterior si está en la URL. */
  async ngOnInit() {
    try {
      this.rooms.set(await this.demo.loadRooms());
      const draft = readReservationDraft(this.route.snapshot.queryParamMap);
      const room = this.rooms().find((item) => item.id === draft?.roomId);
      if (draft && room) {
        const dates = { start: draft.start, end: draft.end, guests: draft.guests };
        this.dates.set(dates);
        this.filterForm.setValue(dates);
        this.building.set(room.building);
        this.floor.set(room.floor);
        this.chooseRoom(room);
      }
    } catch {
      this.error.set('No se pudieron cargar las habitaciones. Vuelve a intentarlo.');
    } finally {
      this.loading.set(false);
    }
  }

  /** Comprueba la disponibilidad de una habitación para las fechas y los adultos elegidos. */
  available(room: Room): boolean {
    const { start, end, guests } = this.dates();
    return this.demo.isAvailable(room, start, end, guests);
  }

  /** Devuelve la etiqueta de estado de la habitación para la búsqueda actual. */
  status(room: Room): string {
    const { start, end, guests } = this.dates();
    return this.demo.roomStatus(room, start, end, guests);
  }

  /** Cuenta las habitaciones disponibles en el edificio indicado. */
  buildingAvailable(building: Building): number {
    return this.rooms().filter((room) => room.building === building.code && this.available(room))
      .length;
  }

  /** Cuenta las habitaciones disponibles en el piso del edificio seleccionado. */
  floorAvailable(floor: number): number {
    return this.rooms().filter(
      (room) => room.building === this.building() && room.floor === floor && this.available(room),
    ).length;
  }

  /** Selecciona un edificio, vuelve al primer piso y limpia la habitación elegida. */
  chooseBuilding(code: string) {
    this.building.set(code);
    this.floor.set(1);
    this.clearSelection();
  }

  /** Selecciona un piso y limpia la habitación anterior. */
  chooseFloor(floor: number) {
    this.floor.set(floor);
    this.clearSelection();
  }

  /** Actualiza la habitación seleccionada y, si se pidió, acerca el detalle a la vista. */
  chooseRoom(room: Room, reveal = false) {
    this.selectedRoomId.set(room.id);
    this.photoIndex.set(0);
    if (reveal) {
      const detail = this.roomDetail().nativeElement;
      detail.focus({ preventScroll: true });
      detail.scrollIntoView({ block: 'nearest' });
    }
  }

  /**
   * Valida y aplica la búsqueda, y sugiere un piso disponible cuando el actual no tiene
   * opciones.
   */
  search() {
    this.filterForm.markAllAsTouched();
    const values = this.filterForm.getRawValue();
    if (this.filterForm.invalid || !validStayDates(values.start, values.end)) {
      this.error.set('Selecciona entrada anterior a salida y entre 1 y 4 adultos.');
      return;
    }
    this.error.set('');
    this.dates.set(values);
    const selected = this.selectedRoom();
    if (selected && !this.available(selected)) {
      this.clearSelection();
    }
    this.searched.set(true);
    if (this.floorAvailable(this.floor()) === 0) {
      const available = this.rooms().filter((room) => this.available(room));
      const first = available.find((room) => room.building === this.building()) ?? available[0];
      if (first) {
        this.building.set(first.building);
        this.floor.set(first.floor);
      }
    }
  }

  /** Abre el resumen del huésped o el formulario de recepción con la selección actual. */
  selectForReservation(room: Room) {
    if (!this.available(room)) {
      return;
    }
    const path = this.publicMode ? '/visitar/confirmar-reserva' : '/explorar/reservaciones';
    void this.router.navigate([path], {
      queryParams: reservationParams({ roomId: room.id, ...this.dates() }),
    });
  }

  /** Vuelve a mostrar la foto si el navegador logra cargarla. */
  showPhoto(event: Event) {
    (event.target as HTMLImageElement).style.display = 'block';
  }

  /** Oculta la foto rota para que quede visible el fondo de respaldo. */
  hideBrokenPhoto(event: Event) {
    (event.target as HTMLImageElement).style.display = 'none';
  }

  /** Quita la habitación elegida y vuelve a la primera foto. */
  private clearSelection() {
    this.selectedRoomId.set(null);
    this.photoIndex.set(0);
  }
}
