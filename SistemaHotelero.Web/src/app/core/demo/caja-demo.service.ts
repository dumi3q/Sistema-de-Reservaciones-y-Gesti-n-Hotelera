import { AuditoriaDemoService } from './auditoria-demo.service';
import { computed, inject, Injectable, signal } from '@angular/core';
import { DashboardDemoService } from './dashboard-demo.service';
import { HotelDemoService } from './hotel-demo.service';
import { MenuDemoService } from './menu-demo.service';
import { RestaurantTable } from './demo.models';
import { demoReservations } from './reservation-demo';

/** Métodos de pago de prueba disponibles en Caja. */
export type PaymentMethod = 'Efectivo' | 'Tarjeta' | 'Transferencia';

/** Importe unitario y cantidad de un cargo que se puede incorporar a una cuenta. */
export interface Charge {
  label: string;
  quantity: number;
  unitCents: number;
}

/** Cuenta de hospedaje o restaurante con sus cargos y estado de cobro. */
export interface Account {
  id: string;
  title: string;
  guest: string;
  type: 'Hospedaje' | 'Restaurante';
  charges: Charge[];
  payable: boolean;
}

/** Comprobante que Caja genera al registrar el pago de una cuenta. */
export interface Receipt {
  date: string;
  id: string;
  account: Account;
  method: PaymentMethod;
  totalCents: number;
}

/** Suma los importes de los cargos en centavos para evitar errores al sumar dólares. */
export function totalCents(charges: Charge[]): number {
  return charges.reduce((total, item) => total + item.quantity * item.unitCents, 0);
}

/**
 * Calcula cuentas, registra cobros y conserva sus comprobantes. Usa HotelDemoService para
 * estadías y mesas, y MenuDemoService para los cargos originales del menú.
 */
@Injectable({ providedIn: 'root' })
export class CajaDemoService {
  private readonly audit = inject(AuditoriaDemoService);
  private readonly hotel = inject(HotelDemoService);
  private readonly referenceDate = inject(DashboardDemoService).referenceDate;
  private readonly menu = inject(MenuDemoService);
  private readonly transfers = signal<{ stayId: string; charges: Charge[] }[]>([]);
  private readonly receiptState = signal<Receipt[]>([]);
  private readonly historicalStays = new Set(
    demoReservations.filter((item) => item.status === 'Finalizada').map((item) => item.id),
  );
  readonly receipts = this.receiptState.asReadonly();

  /** Reservas que ya tienen check-in y admiten cargos a habitación. */
  readonly activeStays = computed(() =>
    this.hotel.reservations().filter((item) => item.status === 'Alojado'),
  );

  /** Cuentas de hospedaje y restaurante que todavía se pueden consultar o cobrar. */
  readonly accounts = computed<Account[]>(() => {
    const paidStays = new Set(
      this.receipts()
        .filter((item) => item.account.type === 'Hospedaje')
        .map((item) => item.account.id),
    );
    const hotelAccounts: Account[] = [];
    for (const stay of this.hotel.reservations()) {
      if (
        (stay.status !== 'Alojado' && stay.status !== 'Finalizada') ||
        this.historicalStays.has(stay.id) ||
        paidStays.has(stay.id)
      ) {
        continue;
      }
      const room = this.hotel.rooms.find((item) => item.id === stay.roomId);
      if (!room) {
        continue;
      }
      hotelAccounts.push({
        id: stay.id,
        title: 'Habitación ' + stay.roomId,
        guest: stay.guest,
        type: 'Hospedaje',
        payable: stay.status === 'Finalizada',
        charges: [
          {
            label: 'Hospedaje',
            quantity: (Date.parse(stay.end) - Date.parse(stay.start)) / 86400000,
            unitCents: Math.round(this.hotel.bookedRoomPrice(stay) * 100),
          },
          ...this.transfers()
            .filter((item) => item.stayId === stay.id)
            .flatMap((item) => item.charges),
        ],
      });
    }
    const tableAccounts = this.hotel
      .restaurantTables()
      .filter((table) => table.status === 'Por cobrar')
      .map((table): Account => ({
        id: table.id,
        title: 'Mesa ' + table.id,
        guest: table.guest || 'Consumo en mesa',
        type: 'Restaurante',
        charges: this.tableCharges(table),
        payable: true,
      }));
    return [...tableAccounts, ...hotelAccounts];
  });

  /** Convierte la comanda de una mesa en cargos con cantidad y precio unitario en centavos. */
  tableCharges(table: RestaurantTable): Charge[] {
    return table.order.flatMap((line) => {
      if (line.unitCents !== undefined) {
        return [{ label: line.name, quantity: line.quantity, unitCents: line.unitCents }];
      }
      const item = this.menu.originalItem(line.name);
      return item
        ? [{ label: item.name, quantity: line.quantity, unitCents: Math.round(item.price * 100) }]
        : [];
    });
  }

  /** Registra un cobro válido, crea el comprobante y libera la mesa si corresponde. */
  pay(id: string, method: PaymentMethod): Receipt | null {
    const account = this.accounts().find((item) => item.id === id);
    if (!account?.payable) {
      return null;
    }
    const total = totalCents(account.charges);
    if (total <= 0) {
      return null;
    }
    const receipt = {
      id: 'DEM-' + String(this.receipts().length + 1).padStart(3, '0'),
      account,
      method,
      totalCents: total,
      date: this.referenceDate,
    };
    if (account.type === 'Restaurante') {
      this.releaseTable(id);
    }
    this.receiptState.update((items) => [receipt, ...items]);
    this.audit.record(
      'Caja',
      'Pago registrado',
      receipt.id,
      account.title + ' · ' + method + ' · USD ' + (total / 100).toFixed(2),
    );
    return receipt;
  }

  /** Agrega el consumo a la cuenta de una estadía activa y libera la mesa. */
  transferToStay(tableId: string, stayId: string): boolean {
    const account = this.accounts().find(
      (item) => item.id === tableId && item.type === 'Restaurante',
    );
    if (
      !account ||
      !account.charges.length ||
      !this.activeStays().some((stay) => stay.id === stayId)
    ) {
      return false;
    }
    const charges = account.charges.map((item) => ({
      ...item,
      label: account.title + ' · ' + item.label,
    }));
    if (!this.addStayCharges(stayId, charges)) {
      return false;
    }
    this.releaseTable(tableId);
    this.audit.record('Caja', 'Consumo cargado a habitación', stayId, account.title);
    return true;
  }

  /** Añade cargos a una estadía que sigue activa. */
  addStayCharges(stayId: string, charges: Charge[]): boolean {
    if (!charges.length || !this.activeStays().some((stay) => stay.id === stayId)) {
      return false;
    }
    this.transfers.update((items) => [...items, { stayId, charges }]);
    return true;
  }

  /** Limpia el estado y el pedido de una mesa para dejarla disponible. */
  private releaseTable(id: string) {
    this.hotel.restaurantTables.update((tables) =>
      tables.map((table) =>
        table.id === id
          ? { ...table, status: 'Libre', guest: '', people: 0, minutes: 0, order: [] }
          : table,
      ),
    );
  }
}
