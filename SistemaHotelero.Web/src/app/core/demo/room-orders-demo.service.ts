import { computed, inject, Injectable, signal } from '@angular/core';
import { CajaDemoService, Charge } from './caja-demo.service';
import { GuestBookingDemoService } from './guest-booking-demo.service';
import { MenuDemoService } from './menu-demo.service';

/** Estados del pedido a habitación desde que se recibe hasta su cierre. */
type OrderStatus = 'Recibido' | 'En preparación' | 'Listo' | 'Entregado' | 'Cancelado';

/** Pedido a una habitación con la estadía y los cargos que se registraron al enviarlo. */
interface RoomOrder {
  id: string;
  stayId: string;
  room: string;
  status: OrderStatus;
  lines: Charge[];
}

/**
 * Gestiona el carrito y el ciclo de pedidos a habitación. Valida estadías con
 * GuestBookingDemoService, consulta el menú y agrega el cargo en CajaDemoService al entregar.
 */
@Injectable({ providedIn: 'root' })
export class RoomOrdersDemoService {
  private readonly guest = inject(GuestBookingDemoService);
  private readonly menu = inject(MenuDemoService);
  private readonly caja = inject(CajaDemoService);
  private readonly quantities = signal<Record<number, number>>({});
  private readonly orderState = signal<RoomOrder[]>([]);
  readonly orders = this.orderState.asReadonly();

  /** Estadías en las que el huésped puede pedir a la habitación. */
  readonly activeStays = computed(() =>
    this.guest.reservations().filter((stay) => stay.status === 'Alojado'),
  );

  /** Líneas del carrito con el precio y la disponibilidad actuales del menú. */
  readonly cart = computed(() =>
    this.menu
      .items()
      .filter((dish) => !!this.quantities()[dish.id])
      .map((dish) => ({
        id: dish.id,
        label: dish.name,
        quantity: this.quantities()[dish.id],
        unitCents: Math.round(dish.price * 100),
        available: dish.available !== false,
      })),
  );

  /** Cantidad total de unidades agregadas al carrito. */
  readonly count = computed(() => this.cart().reduce((total, item) => total + item.quantity, 0));

  /** Prepara los datos de las estadías para validar pedidos a habitación. */
  load(): Promise<void> {
    return this.guest.load();
  }

  /** Ajusta la cantidad de un platillo dentro de los límites del carrito. */
  adjust(id: number, step: 1 | -1) {
    const dish = this.menu.items().find((item) => item.id === id);
    if (!dish || (step > 0 && dish.available === false)) {
      return;
    }
    this.quantities.update((current) => {
      const quantity = Math.max(0, Math.min(99, (current[id] ?? 0) + step));
      return { ...current, [id]: quantity };
    });
  }

  /** Valida la estadía y los platillos, crea el pedido y vacía el carrito. */
  submit(stayId: string): RoomOrder | null {
    const stay = this.activeStays().find((item) => item.id === stayId);
    const cart = this.cart();
    if (!stay || !cart.length || cart.some((item) => !item.available)) {
      return null;
    }
    const order: RoomOrder = {
      id: 'PED-' + String(this.orders().length + 1).padStart(3, '0'),
      stayId,
      room: stay.roomId,
      status: 'Recibido',
      lines: cart.map(({ label, quantity, unitCents }) => ({ label, quantity, unitCents })),
    };
    this.orderState.update((items) => [order, ...items]);
    this.quantities.set({});
    return order;
  }

  /** Cancela el pedido si su estado permite hacerlo desde la vista actual. */
  cancel(id: string, staff: boolean): boolean {
    const order = this.orders().find((item) => item.id === id);
    if (
      !order ||
      order.status === 'Entregado' ||
      order.status === 'Cancelado' ||
      (!staff && order.status !== 'Recibido')
    ) {
      return false;
    }
    this.setStatus(id, 'Cancelado');
    return true;
  }

  /** Avanza el pedido y agrega el cargo a CajaDemoService al confirmar la entrega. */
  advance(id: string): boolean {
    const order = this.orders().find((item) => item.id === id);
    if (!order || order.status === 'Entregado' || order.status === 'Cancelado') {
      return false;
    }
    const next = {
      Recibido: 'En preparación',
      'En preparación': 'Listo',
      Listo: 'Entregado',
    } as const;
    if (!this.activeStays().some((stay) => stay.id === order.stayId)) {
      return false;
    }
    const status = next[order.status];
    if (
      status === 'Entregado' &&
      !this.caja.addStayCharges(
        order.stayId,
        order.lines.map((line) => ({ ...line, label: order.id + ' · ' + line.label })),
      )
    ) {
      return false;
    }
    this.setStatus(id, status);
    return true;
  }

  /** Reemplaza el estado del pedido indicado en la lista compartida. */
  private setStatus(id: string, status: OrderStatus) {
    this.orderState.update((items) =>
      items.map((item) => (item.id === id ? { ...item, status } : item)),
    );
  }
}
