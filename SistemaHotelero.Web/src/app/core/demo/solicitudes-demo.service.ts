import { AuditoriaDemoService } from './auditoria-demo.service';
import { computed, inject, Injectable, signal } from '@angular/core';
import { GuestBookingDemoService } from './guest-booking-demo.service';

/** Estados que indican si una solicitud espera atención, está en curso o ya se cerró. */
type Status = 'Pendiente' | 'Asignada' | 'En curso' | 'Resuelta' | 'Cancelada';

/** Solicitud de servicio con habitación, equipo responsable y estado. */
interface ServiceRequest {
  id: string;
  room: string;
  category: string;
  notes: string;
  assignedTo: string;
  status: Status;
}

/**
 * Mantiene el estado de solicitudes de servicio de la demo. Consulta las estadías del huésped y
 * registra los cambios en AuditoriaDemoService.
 */
@Injectable({ providedIn: 'root' })
export class SolicitudesDemoService {
  private readonly audit = inject(AuditoriaDemoService);
  private readonly guest = inject(GuestBookingDemoService);
  private readonly state = signal<ServiceRequest[]>([]);
  readonly requests = this.state.asReadonly();
  readonly categories = ['Toallas y ropa de cama', 'Limpieza', 'Mantenimiento', 'Otra solicitud'];
  readonly teams = ['Limpieza', 'Mantenimiento', 'Recepción'];

  /** Estadías activas que pueden recibir una solicitud de servicio. */
  readonly stays = computed(() =>
    this.guest.reservations().filter((stay) => stay.status === 'Alojado'),
  );

  /**
   * Carga las estadías necesarias para identificar las habitaciones que pueden pedir servicio.
   */
  load() {
    return this.guest.load();
  }

  /** Crea una solicitud cuando la habitación está alojada y los datos son válidos. */
  create(stayId: string, category: string, notes: string): string | null {
    const stay = this.stays().find((item) => item.id === stayId);
    const detail = notes.trim();
    if (!stay || !this.categories.includes(category) || !detail || detail.length > 300) {
      return null;
    }
    const id = 'SOL-' + String(this.requests().length + 1).padStart(3, '0');
    this.state.update((items) => [
      { id, room: stay.roomId, category, notes: detail, assignedTo: '', status: 'Pendiente' },
      ...items,
    ]);
    this.audit.record(
      'Servicios',
      'Solicitud creada',
      id,
      category + ' · ' + stay.roomId,
      'Huésped de prueba',
    );
    return id;
  }

  /** Asigna una solicitud pendiente a uno de los equipos disponibles. */
  assign(id: string, team: string): boolean {
    const request = this.requests().find((item) => item.id === id);
    if (!request || request.status !== 'Pendiente' || !this.teams.includes(team)) {
      return false;
    }
    this.update(id, 'Asignada', team);
    return true;
  }

  /** Pasa una solicitud asignada a atención, o una en curso a resuelta. */
  advance(id: string): boolean {
    const request = this.requests().find((item) => item.id === id);
    if (request?.status !== 'Asignada' && request?.status !== 'En curso') {
      return false;
    }
    this.update(id, request.status === 'Asignada' ? 'En curso' : 'Resuelta');
    return true;
  }

  /** Cancela una solicitud pendiente desde la vista del huésped. */
  cancel(id: string): boolean {
    if (!this.requests().some((item) => item.id === id && item.status === 'Pendiente')) {
      return false;
    }
    this.update(id, 'Cancelada', undefined, 'Huésped de prueba');
    return true;
  }

  /** Actualiza el estado y el equipo de la solicitud y registra el movimiento. */
  private update(id: string, status: Status, team?: string, actor?: string) {
    this.state.update((items) =>
      items.map((item) =>
        item.id === id ? { ...item, status, assignedTo: team ?? item.assignedTo } : item,
      ),
    );
    this.audit.record(
      'Servicios',
      'Estado actualizado',
      id,
      team ? status + ' · ' + team : status,
      actor,
    );
  }
}
