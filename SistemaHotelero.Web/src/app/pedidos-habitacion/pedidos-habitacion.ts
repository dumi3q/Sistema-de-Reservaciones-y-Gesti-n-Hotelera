import { CurrencyPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { totalCents } from '../core/demo/caja-demo.service';
import { RoomOrdersDemoService } from '../core/demo/room-orders-demo.service';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Muestra el carrito y el historial al huésped, o la cola de trabajo al personal.
 * RoomOrdersDemoService realiza las operaciones de pedido.
 */
@Component({
  selector: 'app-pedidos-habitacion',
  imports: [CurrencyPipe, RouterLink, Skeleton],
  templateUrl: './pedidos-habitacion.html',
  styleUrls: [
    '../shared/explorer.css',
    '../shared/recepcion.css',
    '../shared/operacion.css',
    './pedidos-habitacion.css',
  ],
})
export class PedidosHabitacion {
  readonly demo = inject(RoomOrdersDemoService);
  readonly staff = inject(ActivatedRoute).snapshot.data['staff'] === true;
  readonly loading = signal(true);
  readonly stayId = signal('');
  readonly message = signal('');
  readonly total = totalCents;

  /** Subtotal del carrito de servicio a la habitación. */
  readonly cartTotal = computed(() => totalCents(this.demo.cart()) / 100);

  /** Indica si un producto del carrito dejó de estar disponible. */
  readonly unavailable = computed(() => this.demo.cart().some((item) => !item.available));

  /** Carga los pedidos y selecciona la primera estadía activa para el formulario. */
  async ngOnInit() {
    await this.demo.load();
    this.stayId.set(this.demo.activeStays()[0]?.id ?? '');
    this.loading.set(false);
  }

  /** Envía el carrito de la estadía elegida y muestra el resultado. */
  submit() {
    const order = this.demo.submit(this.stayId());
    this.message.set(
      order
        ? 'Pedido ' + order.id + ' enviado al restaurante.'
        : 'Revisa la estadía y la disponibilidad del carrito.',
    );
  }

  /** Avanza el estado del pedido desde la vista del personal. */
  advance(id: string) {
    this.message.set(
      this.demo.advance(id)
        ? 'Estado actualizado.'
        : 'La estadía debe estar activa para continuar el pedido.',
    );
  }

  /** Solicita la cancelación con las reglas de la vista actual. */
  cancel(id: string) {
    this.message.set(
      this.demo.cancel(id, this.staff)
        ? 'Pedido ' + id + ' cancelado.'
        : 'El pedido ya no se puede cancelar.',
    );
  }
}
