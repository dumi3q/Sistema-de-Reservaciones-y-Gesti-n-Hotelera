import { Component, computed, ElementRef, inject, OnInit, signal, viewChild } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HotelDemoService } from '../core/demo/hotel-demo.service';
import { RestaurantTable, TableArea, TableStatus } from '../core/demo/demo.models';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Muestra las mesas y sus estados en un plano. Lee los datos de HotelDemoService y enlaza con
 * pedidos, reservas de mesa y Caja.
 */
@Component({
  selector: 'app-restaurante',
  standalone: true,
  imports: [RouterLink, Skeleton],
  templateUrl: './restaurante.html',
  styleUrls: [
    '../shared/explorer.css',
    './restaurante.css',
    './table-detail.css',
    './restaurant-dialog.css',
  ],
})
export class Restaurante implements OnInit {
  readonly detailDialog = viewChild<ElementRef<HTMLDialogElement>>('detailDialog');
  private readonly demo = inject(HotelDemoService);
  readonly areas = ['Todas las áreas', 'Restaurante', 'Piscina', 'Bar'] as const;
  readonly states: TableStatus[] = [
    'Libre',
    'Por tomar orden',
    'En preparación',
    'Atendida',
    'Por cobrar',
  ];
  readonly area = signal<string>('Todas las áreas');
  readonly statusFilter = signal<string>('Todos');
  readonly tables = this.demo.restaurantTables;
  readonly selectedId = signal<string | null>(null);
  readonly loading = signal(true);
  readonly error = signal('');

  /** Mesa cuyo detalle se muestra actualmente. */
  readonly selected = computed(
    () => this.tables().find((table) => table.id === this.selectedId()) ?? null,
  );

  /** Mesas que coinciden con los filtros de área y estado. */
  readonly filteredTables = computed(() =>
    this.tables().filter(
      (table) =>
        (this.area() === 'Todas las áreas' || table.area === this.area()) &&
        (this.statusFilter() === 'Todos' || table.status === this.statusFilter()),
    ),
  );
  readonly mapAreas: readonly TableArea[] = ['Restaurante', 'Piscina', 'Bar'];

  /** Identificadores que permiten distinguir las mesas visibles en el plano. */
  readonly matchingIds = computed(() => new Set(this.filteredTables().map((table) => table.id)));

  /** Cantidad de mesas que están en uso. */
  readonly occupied = computed(
    () => this.tables().filter((table) => table.status !== 'Libre').length,
  );

  /** Cantidad total de comensales registrados en las mesas. */
  readonly people = computed(() => this.tables().reduce((total, table) => total + table.people, 0));

  /** Cantidad de mesas que necesitan tomar una orden o cobrar. */
  readonly needsAttention = computed(
    () => this.count('Por tomar orden') + this.count('Por cobrar'),
  );

  /** Carga el mapa de mesas y muestra un mensaje si falla la consulta. */
  async ngOnInit() {
    try {
      await this.demo.loadTables();
    } catch {
      this.error.set('No se pudo cargar el mapa de mesas.');
    } finally {
      this.loading.set(false);
    }
  }

  /** Cuenta las mesas que tienen el estado indicado. */
  count(status: TableStatus): number {
    return this.tables().filter((table) => table.status === status).length;
  }

  /** Obtiene las mesas de un área para mantener sus posiciones en el plano. */
  tablesForArea(area: string): RestaurantTable[] {
    return this.tables().filter((table) => table.area === area);
  }

  /** Asigna a cada estado la clase visual que usa el mapa. */
  tone(status: TableStatus): string {
    return {
      Libre: 'free',
      'Por tomar orden': 'waiting',
      'En preparación': 'preparing',
      Atendida: 'served',
      'Por cobrar': 'payment',
    }[status];
  }

  /** Aplica el filtro de área y revisa la selección actual. */
  chooseArea(area: string) {
    this.area.set(area);
    this.clearSelectionIfHidden();
  }

  /** Activa o desactiva el filtro de una tarjeta de estado para todas las áreas. */
  showStatus(status: TableStatus) {
    const next = this.statusFilter() === status ? 'Todos' : status;
    this.area.set('Todas las áreas');
    this.filterStatus(next);
  }

  /** Cambia el filtro de estado y limpia la selección si quedó fuera del resultado. */
  filterStatus(status: string) {
    this.statusFilter.set(status);
    this.clearSelectionIfHidden();
  }

  /** Selecciona una mesa y abre su detalle. */
  chooseTable(table: RestaurantTable) {
    this.selectedId.set(table.id);
    this.detailDialog()?.nativeElement.showModal?.();
  }

  /** Quita la selección cuando un filtro oculta la mesa activa. */
  private clearSelectionIfHidden() {
    if (!this.filteredTables().some((table) => table.id === this.selectedId())) {
      this.selectedId.set(null);
    }
  }
}
