import { Component, computed, inject, signal } from '@angular/core';
import { HotelDemoService, TariffRule } from '../core/demo/hotel-demo.service';
import { AdminNav } from '../shared/admin-nav/admin-nav';

/**
 * Edita tipos de habitación y reglas de tarifa mediante HotelDemoService. Las reglas se
 * reflejan en las cotizaciones nuevas.
 */
@Component({
  selector: 'app-administrar-tarifas',
  imports: [AdminNav],
  templateUrl: './administrar-tarifas.html',
  styleUrls: [
    '../shared/explorer.css',
    '../shared/recepcion.css',
    '../shared/administracion.css',
    './administrar-tarifas.css',
  ],
})
export class AdministrarTarifas {
  readonly hotel = inject(HotelDemoService);

  /** Lista de tipos sin duplicados a partir del inventario de habitaciones. */
  readonly types = computed(() => [...new Set(this.hotel.rooms.map((room) => room.type))]);
  readonly selected = signal('');
  readonly name = signal('');
  readonly capacity = signal(1);
  readonly rule = signal<TariffRule>({
    id: 0,
    label: '',
    type: '',
    start: '',
    end: '',
    adjustment: 0,
    active: true,
  });
  readonly message = signal('');
  readonly error = signal('');

  /** Carga el nombre y la capacidad del tipo elegido para editarlos. */
  chooseType(type: string) {
    this.selected.set(type);
    this.name.set(type);
    this.capacity.set(this.hotel.rooms.find((room) => room.type === type)?.adults ?? 1);
  }

  /** Pide a HotelDemoService que actualice el tipo sin invalidar reservas vigentes. */
  saveType() {
    const saved = this.hotel.renameRoomType(this.selected(), this.name(), this.capacity());
    this.error.set(
      saved
        ? ''
        : 'Usa un nombre único y una capacidad que permita mantener las reservas vigentes.',
    );
    this.message.set(saved ? 'Tipo de habitación actualizado.' : '');
    if (saved) {
      this.chooseType(this.name().trim());
    }
  }

  /** Cambia únicamente los campos de la regla que se están editando. */
  patchRule(value: Partial<TariffRule>) {
    this.rule.update((rule) => ({ ...rule, ...value }));
  }

  /** Envía la regla al servicio y limpia el editor después de guardarla. */
  saveRule() {
    const saved = this.hotel.saveTariff(this.rule());
    this.error.set(saved ? '' : 'Revisa el nombre, las fechas y el porcentaje de ajuste.');
    this.message.set(saved ? 'Regla de tarifa guardada.' : '');
    if (saved) {
      this.rule.set({
        id: 0,
        label: '',
        type: '',
        start: '',
        end: '',
        adjustment: 0,
        active: true,
      });
    }
  }
}
