import { DatePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { AuditoriaDemoService } from '../core/demo/auditoria-demo.service';
import { normalizeReservationSearch } from '../core/demo/reservation-demo';
import { AdminNav } from '../shared/admin-nav/admin-nav';

/**
 * Muestra la bitácora de AuditoriaDemoService y permite filtrar los eventos por área o texto.
 */
@Component({
  selector: 'app-auditoria',
  imports: [AdminNav, DatePipe],
  templateUrl: './auditoria.html',
  styleUrls: [
    '../shared/explorer.css',
    '../shared/recepcion.css',
    '../shared/administracion.css',
    './auditoria.css',
  ],
})
export class Auditoria {
  readonly audit = inject(AuditoriaDemoService);
  readonly query = signal('');
  readonly area = signal('');

  /** Áreas que tienen al menos un evento en la sesión. */
  readonly areas = computed(() =>
    [...new Set(this.audit.entries().map((item) => item.area))].sort(),
  );

  /** Eventos que coinciden con el área y el texto buscado. */
  readonly filtered = computed(() => {
    const query = normalizeReservationSearch(this.query().trim());
    return this.audit
      .entries()
      .filter(
        (item) =>
          (!this.area() || item.area === this.area()) &&
          normalizeReservationSearch(
            [item.action, item.target, item.detail, item.actor].join(' '),
          ).includes(query),
      );
  });
}
