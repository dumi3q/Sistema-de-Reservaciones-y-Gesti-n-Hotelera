import { AuditoriaDemoService } from './auditoria-demo.service';
import { Injectable, signal } from '@angular/core';
import { Building, RestaurantTable, Room } from './demo.models';
import { validStayDates } from './reservation-selection';
import {
  demoReservations,
  DemoReservation,
  ReservationDraft,
  ReservationStatus,
  reservationTransitions,
} from './reservation-demo';

/**
 * Ajuste porcentual que se puede aplicar a una cotización según la fecha y el tipo de
 * habitación.
 */
export interface TariffRule {
  id: number;
  label: string;
  type: string;
  start: string;
  end: string;
  adjustment: number;
  active: boolean;
}

/**
 * Centraliza habitaciones, tarifas, reservas y mesas de la demo. Las pantallas de huésped y
 * personal consultan este mismo estado y los cambios relevantes se registran en auditoría.
 */
@Injectable({ providedIn: 'root' })
export class HotelDemoService {
  /**
   * Recibe la bitácora de la demo. Las pruebas también pueden crear el servicio de forma
   * aislada.
   */
  constructor(private readonly audit: AuditoriaDemoService = new AuditoriaDemoService()) {}
  private readonly reservationState = signal<DemoReservation[]>(demoReservations);
  readonly reservations = this.reservationState.asReadonly();
  private readonly reservationLoading = signal(true);
  readonly reservationsLoading = this.reservationLoading.asReadonly();
  private reservationLoad?: Promise<void>;

  /** Simula la carga de reservas una sola vez y comparte su estado entre pantallas. */
  loadReservations(): Promise<void> {
    this.reservationLoad ??= this.pause().then(() => this.reservationLoading.set(false));
    return this.reservationLoad;
  }
  private readonly seededRooms = new Set(demoReservations.map((item) => item.roomId));
  private nextReservation = demoReservations.length + 1;

  /** Comprueba la disponibilidad y crea una reserva con su tarifa y su identificador. */
  createReservation(
    draft: ReservationDraft,
    guest: string,
    guestId?: string,
  ): DemoReservation | null {
    const room = this.rooms.find((item) => item.id === draft.roomId);
    if (!guest.trim() || !room || !this.isAvailable(room, draft.start, draft.end, draft.guests)) {
      return null;
    }
    const id = 'RES-' + String(this.nextReservation++).padStart(3, '0');
    const created: DemoReservation = {
      ...draft,
      id,
      guest: guest.trim(),
      nightlyPrice: this.quoteRoomPrice(room, draft.start),
      guestId: guestId ?? id.replace('RES-', 'HUE-'),
      status: 'Confirmada',
    };
    this.reservationState.update((items) => [...items, created]);
    this.audit.record(
      'Reservaciones',
      'Reserva creada',
      id,
      room.id + ' · ' + draft.start + ' a ' + draft.end,
      guestId ? 'Huésped de prueba' : 'Personal de prueba',
    );
    return created;
  }

  /** Aplica una transición permitida y registra el estado anterior y el nuevo. */
  updateReservation(id: string, status: ReservationStatus, actor?: string): DemoReservation | null {
    const current = this.reservations().find((item) => item.id === id);
    if (!current || !reservationTransitions[current.status].includes(status)) {
      return null;
    }
    const updated = { ...current, status };
    this.reservationState.update((items) => items.map((item) => (item.id === id ? updated : item)));
    this.audit.record(
      'Reservaciones',
      'Estado actualizado',
      id,
      current.status + ' → ' + status,
      actor,
    );
    return updated;
  }

  readonly buildings: Building[] = [
    { code: 'A', name: 'Edificio A', description: 'La calma de llegar a casa.', floors: 6 },
    { code: 'B', name: 'Edificio B', description: 'Un espacio para desconectar.', floors: 6 },
    { code: 'C', name: 'Edificio C', description: 'Momentos para compartir.', floors: 6 },
    { code: 'D', name: 'Edificio D', description: 'Tu próximo lugar favorito.', floors: 6 },
  ];

  private readonly initialRooms: Room[] = this.buildings.flatMap((building, buildingIndex) =>
    Array.from({ length: 6 }, (_, index) => index + 1).flatMap((floor) =>
      Array.from({ length: 4 }, (_, index) => {
        const position = index + 1;
        const presidential = floor === 6 && position === 4;
        const types = ['Simple', 'Doble Estándar', 'Familiar', 'Suite Ejecutiva'];
        const prices = [60, 90, 140, 160];
        const adultCapacity = [1, 2, 4, 2];
        const childCapacity = [0, 2, 3, 1];
        const capacity = [1, 4, 6, 3];
        const suite = position === 4;
        const sequence = buildingIndex * 24 + (floor - 1) * 4 + index;
        return {
          id: `${building.code}-${floor * 100 + position}`,
          building: building.code,
          floor,
          number: floor * 100 + position,
          type: presidential ? 'Suite Presidencial' : types[index],
          adults: presidential ? 4 : adultCapacity[index],
          children: presidential ? 2 : childCapacity[index],
          capacity: presidential ? 6 : capacity[index],
          price: presidential ? 350 : prices[index],
          area: presidential ? 86 : [22, 34, 52, 61][index],
          amenities: [
            'Wi-Fi',
            'Aire acondicionado',
            'Caja fuerte',
            ...(suite || position === 3 ? ['Cama king', 'Balcón'] : []),
            ...(suite ? ['Minibar'] : []),
            ...(presidential ? ['Jacuzzi', 'Vista al mar'] : []),
          ],
          photos: [
            'https://images.unsplash.com/photo-1611892440504-42a792e24d32?auto=format&fit=crop&w=1200&q=85',
            'https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=1200&q=85',
            'https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=1200&q=85',
          ],
          maintenance: sequence % 17 === 16,
          stays:
            sequence % 4 === 1
              ? [{ start: '2026-12-10', end: '2026-12-13' }]
              : sequence % 7 === 3
                ? [{ start: '2026-12-14', end: '2026-12-17' }]
                : [],
        } satisfies Room;
      }),
    ),
  );

  private readonly roomState = signal(this.initialRooms);
  private readonly tariffState = signal<TariffRule[]>([]);
  readonly tariffs = this.tariffState.asReadonly();

  /**
   * Calcula la tarifa por noche aplicando la última regla que corresponda a la fecha y al tipo.
   */
  quoteRoomPrice(room: Room, start: string): number {
    const rule = [...this.tariffs()]
      .reverse()
      .find(
        (item) =>
          item.active &&
          (!item.type || item.type === room.type) &&
          item.start <= start &&
          start < item.end,
      );
    return Math.round(room.price * (100 + (rule?.adjustment ?? 0))) / 100;
  }

  /** Valida y guarda una regla de tarifa para futuras cotizaciones. */
  saveTariff(rule: TariffRule): boolean {
    const label = rule.label.trim();
    if (
      !label ||
      label.length > 60 ||
      !validStayDates(rule.start, rule.end) ||
      !Number.isFinite(rule.adjustment) ||
      rule.adjustment < -90 ||
      rule.adjustment > 300 ||
      (rule.type && !this.rooms.some((room) => room.type === rule.type))
    ) {
      return false;
    }
    const items = this.tariffs();
    if (rule.id && !items.some((item) => item.id === rule.id)) {
      return false;
    }
    const saved = { ...rule, label, id: rule.id || items.length + 1 };
    this.tariffState.set(
      rule.id ? items.map((item) => (item.id === rule.id ? saved : item)) : [...items, saved],
    );
    this.audit.record('Habitaciones', 'Regla de tarifa guardada', String(saved.id), saved.label);
    return true;
  }

  /** Actualiza el nombre y la capacidad del tipo sin invalidar las reservas vigentes. */
  renameRoomType(type: string, name: string, adults: number): boolean {
    const label = name.trim();
    const matching = this.rooms.filter((room) => room.type === type);
    if (
      !matching.length ||
      !label ||
      label.length > 60 ||
      !Number.isInteger(adults) ||
      adults < 1 ||
      adults > 4 ||
      matching.some((room) => room.capacity < adults) ||
      this.reservations().some(
        (item) =>
          (item.status === 'Pendiente' ||
            item.status === 'Confirmada' ||
            item.status === 'Alojado') &&
          item.guests > adults &&
          matching.some((room) => room.id === item.roomId),
      ) ||
      this.rooms.some(
        (room) => room.type !== type && room.type.toLowerCase() === label.toLowerCase(),
      )
    ) {
      return false;
    }
    this.roomState.update((items) =>
      items.map((room) => (room.type === type ? { ...room, type: label, adults } : room)),
    );
    this.tariffState.update((items) =>
      items.map((rule) => (rule.type === type ? { ...rule, type: label } : rule)),
    );
    this.audit.record('Habitaciones', 'Tipo de habitación actualizado', label, adults + ' adultos');
    return true;
  }

  /**
   * Conserva el precio reservado si la habitación y las fechas no cambian, o calcula una tarifa
   * nueva.
   */
  rebookingPrice(booking: DemoReservation, room: Room, start: string, end: string): number {
    return booking.roomId === room.id && booking.start === start && booking.end === end
      ? this.bookedRoomPrice(booking)
      : this.quoteRoomPrice(room, start);
  }

  /**
   * Valida la nueva selección sin contar la propia reserva como un conflicto y guarda la
   * edición.
   */
  editReservation(id: string, draft: ReservationDraft): DemoReservation | null {
    const booking = this.reservations().find((item) => item.id === id);
    const room = this.rooms.find((item) => item.id === draft.roomId);
    if (
      !booking ||
      !room ||
      !['Pendiente', 'Confirmada'].includes(booking.status) ||
      !this.isAvailable(room, draft.start, draft.end, draft.guests, id)
    ) {
      return null;
    }
    const updated = {
      ...booking,
      ...draft,
      nightlyPrice: this.rebookingPrice(booking, room, draft.start, draft.end),
    };
    this.reservationState.update((items) => items.map((item) => (item.id === id ? updated : item)));
    this.audit.record(
      'Reservaciones',
      'Reserva editada',
      id,
      booking.roomId + ' → ' + draft.roomId + ' · ' + draft.start + ' a ' + draft.end,
    );
    return updated;
  }

  /** Devuelve el inventario actual con los cambios guardados durante la sesión. */
  get rooms(): Room[] {
    return this.roomState();
  }

  /** Devuelve la tarifa pactada o la tarifa original para una reserva precargada. */
  bookedRoomPrice(reservation: DemoReservation): number {
    return (
      reservation.nightlyPrice ??
      this.initialRooms.find((room) => room.id === reservation.roomId)?.price ??
      0
    );
  }

  /** Actualiza la tarifa base, las amenidades, las fotos y el estado de mantenimiento. */
  updateRoom(
    id: string,
    changes: Pick<Room, 'price' | 'amenities' | 'photos' | 'maintenance'>,
  ): string | null {
    const room = this.rooms.find((item) => item.id === id);
    if (!room) {
      return 'La habitación ya no está disponible.';
    }
    if (!Number.isFinite(changes.price) || changes.price < 0.01 || changes.price > 100000) {
      return 'Ingresa una tarifa entre $0.01 y $100,000.';
    }
    if (
      changes.photos.some((photo) => !URL.canParse(photo) || new URL(photo).protocol !== 'https:')
    ) {
      return 'Usa enlaces HTTPS válidos para las fotos.';
    }
    if (
      !room.maintenance &&
      changes.maintenance &&
      this.reservations().some(
        (item) =>
          item.roomId === id && ['Pendiente', 'Confirmada', 'Alojado'].includes(item.status),
      )
    ) {
      return 'Esta habitación tiene una reserva vigente. Revisa la reserva antes de bloquearla.';
    }
    const price = Math.round(changes.price * 100) / 100;
    this.roomState.update((items) =>
      items.map((item) => (item.id === id ? { ...item, ...changes, price } : item)),
    );
    this.audit.record(
      'Habitaciones',
      'Habitación actualizada',
      id,
      'Tarifa USD ' +
        room.price +
        ' → ' +
        price +
        ' · Mantenimiento: ' +
        (changes.maintenance ? 'sí' : 'no'),
    );
    return null;
  }

  /** Simula la carga inicial y devuelve el inventario de habitaciones. */
  async loadRooms(): Promise<Room[]> {
    await this.pause();
    return this.rooms;
  }

  readonly restaurantTables = signal<RestaurantTable[]>([]);
  private tableLoad?: Promise<void>;

  /** Carga las mesas una sola vez y devuelve el estado compartido del restaurante. */
  async loadTables(): Promise<RestaurantTable[]> {
    this.tableLoad ??= this.pause().then(() => this.restaurantTables.set(this.demoTables));
    await this.tableLoad;
    return this.restaurantTables();
  }

  /** Mueve una mesa al siguiente estado permitido del servicio. */
  advanceTable(id: string): RestaurantTable['status'] | null {
    const table = this.restaurantTables().find((item) => item.id === id);
    if (!table) {
      return null;
    }
    const next = {
      Libre: 'Por tomar orden',
      'Por tomar orden': 'En preparación',
      'En preparación': 'Atendida',
      Atendida: 'Por cobrar',
      'Por cobrar': null,
    } as const;
    const status = next[table.status];
    if (!status || (table.status === 'Por tomar orden' && !table.order.length)) {
      return null;
    }
    this.restaurantTables.update((items) =>
      items.map((item) =>
        item.id === id ? { ...item, status, people: item.people || Math.min(item.seats, 2) } : item,
      ),
    );
    return status;
  }

  private readonly demoTables: RestaurantTable[] = [
    {
      id: 'R1',
      seats: 4,
      area: 'Restaurante',
      status: 'Libre',
      guest: '',
      people: 0,
      minutes: 0,
      order: [],
    },
    {
      id: 'R2',
      seats: 4,
      area: 'Restaurante',
      status: 'Por tomar orden',
      guest: 'Ana Martínez',
      people: 3,
      minutes: 8,
      order: [],
    },
    {
      id: 'R3',
      seats: 2,
      area: 'Restaurante',
      status: 'En preparación',
      guest: 'Luis Castro',
      people: 2,
      minutes: 14,
      order: [
        { name: 'Casado con pollo', quantity: 2 },
        { name: 'Batido de fruta', quantity: 2 },
      ],
    },
    {
      id: 'R4',
      seats: 6,
      area: 'Restaurante',
      status: 'Atendida',
      guest: 'Sofía Ramírez',
      people: 5,
      minutes: 26,
      order: [
        { name: 'Pasta al pesto', quantity: 3 },
        { name: 'Filete de res', quantity: 2 },
      ],
    },
    {
      id: 'P1',
      seats: 4,
      area: 'Piscina',
      status: 'Por cobrar',
      guest: 'Carlos Vega',
      people: 4,
      minutes: 42,
      order: [
        { name: 'Patacones con frijoles', quantity: 2 },
        { name: 'Batido de fruta', quantity: 4 },
      ],
    },
    {
      id: 'P2',
      seats: 4,
      area: 'Piscina',
      status: 'Libre',
      guest: '',
      people: 0,
      minutes: 0,
      order: [],
    },
    {
      id: 'B1',
      seats: 2,
      area: 'Bar',
      status: 'Por tomar orden',
      guest: 'Lucía Rojas',
      people: 2,
      minutes: 6,
      order: [],
    },
    {
      id: 'B2',
      seats: 2,
      area: 'Bar',
      status: 'Libre',
      guest: '',
      people: 0,
      minutes: 0,
      order: [],
    },
  ];

  /** Comprueba fechas, capacidad, mantenimiento y cruces con otras reservas. */
  isAvailable(room: Room, start: string, end: string, guests: number, excludeId?: string): boolean {
    /**
     * Comprueba si una estadía cruza el intervalo consultado. La salida puede coincidir con
     * otra entrada.
     */
    const overlaps = (stay: { start: string; end: string }) => start < stay.end && end > stay.start;
    return (
      validStayDates(start, end) &&
      !room.maintenance &&
      Number.isInteger(guests) &&
      guests > 0 &&
      room.adults >= guests &&
      (this.seededRooms.has(room.id) || !room.stays.some(overlaps)) &&
      !this.reservations().some(
        (item) =>
          item.id !== excludeId &&
          item.roomId === room.id &&
          item.status !== 'Cancelada' &&
          overlaps(item),
      )
    );
  }

  /** Traduce la disponibilidad y sus restricciones en una etiqueta para la pantalla. */
  roomStatus(room: Room, start: string, end: string, guests: number): string {
    if (room.maintenance) {
      return 'Mantenimiento';
    }
    if (room.adults < guests) {
      return 'Capacidad insuficiente';
    }
    return this.isAvailable(room, start, end, guests) ? 'Disponible' : 'Reservada';
  }

  /** Simula la espera de la fuente de datos en las cargas de prueba. */
  private pause(): Promise<void> {
    return new Promise((resolve) => setTimeout(resolve, 650));
  }
}
