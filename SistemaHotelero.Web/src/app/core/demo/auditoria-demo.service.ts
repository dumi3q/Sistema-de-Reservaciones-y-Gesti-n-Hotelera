import { Injectable, signal } from '@angular/core';

/**
 * Mantiene en memoria la bitácora de la sesión. Los servicios de operación registran cambios y
 * Auditoria muestra el historial.
 */
@Injectable({ providedIn: 'root' })
export class AuditoriaDemoService {
  private readonly state = signal<
    {
      id: number;
      date: Date;
      area: string;
      action: string;
      target: string;
      detail: string;
      actor: string;
    }[]
  >([]);
  readonly entries = this.state.asReadonly();

  /** Agrega un evento con fecha, actor y descripción al inicio de la bitácora de la sesión. */
  record(
    area: string,
    action: string,
    target: string,
    detail: string,
    actor = 'Personal de prueba',
  ) {
    this.state.update((entries) => [
      { id: entries.length + 1, date: new Date(), area, action, target, detail, actor },
      ...entries,
    ]);
  }
}
