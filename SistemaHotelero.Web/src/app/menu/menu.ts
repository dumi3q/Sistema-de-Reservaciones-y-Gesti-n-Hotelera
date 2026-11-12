import { CurrencyPipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { totalCents } from '../core/demo/caja-demo.service';
import { MenuDemoService, MenuItem } from '../core/demo/menu-demo.service';
import { RoomOrdersDemoService } from '../core/demo/room-orders-demo.service';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Muestra el catálogo de MenuDemoService. En servicio a la habitación permite agregar productos
 * al carrito de RoomOrdersDemoService.
 */
@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CurrencyPipe, RouterLink, Skeleton],
  templateUrl: './menu.html',
  styleUrls: ['../shared/explorer.css', '../shared/recepcion.css', './menu.css'],
})
export class Menu implements OnInit {
  private readonly demo = inject(MenuDemoService);
  readonly orders = inject(RoomOrdersDemoService);
  readonly roomService = inject(ActivatedRoute).snapshot.data['roomService'] === true;
  readonly lastAdded = signal('');

  /** Subtotal de los productos agregados al carrito. */
  readonly subtotal = computed(() => totalCents(this.orders.cart()) / 100);

  /** Cantidad agregada por platillo para mostrarla en cada tarjeta. */
  readonly quantities = computed(
    () => new Map(this.orders.cart().map((item) => [item.id, item.quantity])),
  );
  readonly categories = ['Todo', ...this.demo.categories];
  readonly category = signal('Todo');
  readonly items = signal<MenuItem[]>([]);
  readonly loading = signal(true);

  /** Platillos disponibles que coinciden con la categoría activa. */
  readonly filtered = computed(() =>
    this.items().filter(
      (item) =>
        item.available !== false &&
        (this.category() === 'Todo' || item.category === this.category()),
    ),
  );

  /** Agrega una unidad al carrito y anuncia el cambio si se pudo aplicar. */
  add(item: MenuItem) {
    const before = this.orders.count();
    this.orders.adjust(item.id, 1);
    if (this.orders.count() > before) {
      this.lastAdded.set(item.name);
    }
  }

  /** Carga los platillos de MenuDemoService antes de mostrar la carta. */
  async ngOnInit() {
    try {
      this.items.set(await this.demo.load());
    } finally {
      this.loading.set(false);
    }
  }
}
