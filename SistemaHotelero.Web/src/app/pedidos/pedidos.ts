import { CurrencyPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CajaDemoService, totalCents } from '../core/demo/caja-demo.service';
import { HotelDemoService } from '../core/demo/hotel-demo.service';
import { RestaurantOrderLine } from '../core/demo/demo.models';
import { MenuDemoService, MenuItem } from '../core/demo/menu-demo.service';
import { normalizeReservationSearch } from '../core/demo/reservation-demo';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Gestiona la comanda de una mesa. Consulta el menú, actualiza la mesa en HotelDemoService y
 * obtiene los importes de CajaDemoService.
 */
@Component({
  selector: 'app-pedidos',
  imports: [CurrencyPipe, RouterLink, Skeleton],
  templateUrl: './pedidos.html',
  styleUrls: [
    '../shared/explorer.css',
    '../shared/recepcion.css',
    '../shared/operacion.css',
    './pedidos.css',
  ],
})
export class Pedidos {
  private readonly hotel = inject(HotelDemoService);
  private readonly menu = inject(MenuDemoService);
  private readonly caja = inject(CajaDemoService);
  readonly tables = this.hotel.restaurantTables;
  readonly tableId = signal(inject(ActivatedRoute).snapshot.queryParamMap.get('mesa'));

  /** Mesa seleccionada dentro del inventario compartido. */
  readonly selected = computed(() => this.tables().find((table) => table.id === this.tableId()));
  readonly loading = signal(true);
  readonly query = signal('');
  readonly message = signal('');

  /** Platillos del catálogo que coinciden con el texto buscado. */
  readonly dishes = computed(() => {
    const query = normalizeReservationSearch(this.query().trim());
    return this.menu
      .items()
      .filter((item) =>
        normalizeReservationSearch(item.name + ' ' + item.category).includes(query),
      );
  });

  /** Cargos de la comanda de la mesa seleccionada. */
  readonly charges = computed(() => {
    const table = this.selected();
    return table ? this.caja.tableCharges(table) : [];
  });

  /** Líneas de la comanda organizadas por identificador de platillo. */
  readonly orderLines = computed(() => {
    const result: Record<number, RestaurantOrderLine> = {};
    for (const line of this.selected()?.order ?? []) {
      const id = this.lineId(line);
      if (id !== undefined) {
        result[id] = line;
      }
    }
    return result;
  });

  /** Obtiene el precio guardado en la comanda o la tarifa del catálogo si la línea es nueva. */
  unitPrice(dish: MenuItem): number {
    const line = this.orderLines()[dish.id];
    if (!line) {
      return dish.price;
    }
    return line.unitCents !== undefined
      ? line.unitCents / 100
      : (this.menu.originalItem(line.name)?.price ?? dish.price);
  }

  /** Identifica el platillo de una línea actual o precargada para relacionarlo con el menú. */
  private lineId(line: RestaurantOrderLine): number | undefined {
    return line.menuItemId ?? this.menu.originalItem(line.name)?.id;
  }

  /** Total de la comanda expresado en dólares para mostrarlo. */
  readonly total = computed(() => totalCents(this.charges()) / 100);

  /** Carga las mesas y selecciona la que indica la ruta o una opción inicial. */
  async ngOnInit() {
    await this.hotel.loadTables();
    if (!this.selected()) {
      this.tableId.set(
        this.tables().find((table) => table.status === 'Por tomar orden')?.id ??
          this.tables()[0]?.id ??
          null,
      );
    }
    this.loading.set(false);
  }

  /** Selecciona una mesa y reinicia los filtros y mensajes de la comanda. */
  select(id: string) {
    this.tableId.set(id);
    this.query.set('');
    this.message.set('');
  }

  /** Agrega o quita una unidad en la comanda si la mesa admite cambios. */
  adjust(dish: MenuItem, difference: number) {
    const table = this.selected();
    if (
      !table ||
      table.status !== 'Por tomar orden' ||
      (difference > 0 && dish.available === false)
    ) {
      return;
    }
    const current = table.order.find((item) => this.lineId(item) === dish.id);
    const quantity = (current?.quantity ?? 0) + difference;
    if (quantity < 0 || quantity > 99) {
      return;
    }
    const order = current
      ? table.order
          .map((item) => (this.lineId(item) === dish.id ? { ...item, quantity } : item))
          .filter((item) => item.quantity > 0)
      : [
          ...table.order,
          {
            name: dish.name,
            menuItemId: dish.id,
            quantity,
            unitCents: Math.round(dish.price * 100),
          },
        ];
    this.hotel.restaurantTables.update((items) =>
      items.map((item) => (item.id === table.id ? { ...item, order } : item)),
    );
  }

  /** Solicita a HotelDemoService que avance el estado de la mesa. */
  advance() {
    const table = this.selected();
    if (!table) {
      return;
    }
    const status = this.hotel.advanceTable(table.id);
    if (status) {
      this.message.set('Mesa ' + table.id + ' · ' + status + '.');
    }
  }
}
