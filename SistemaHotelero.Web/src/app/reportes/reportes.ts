import { CurrencyPipe, DatePipe, PercentPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { CajaDemoService } from '../core/demo/caja-demo.service';
import { DashboardDemoService } from '../core/demo/dashboard-demo.service';
import { HotelDemoService } from '../core/demo/hotel-demo.service';
import { validStayDates } from '../core/demo/reservation-selection';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Resume ocupación, reservas y cobros por período. Combina HotelDemoService y CajaDemoService y
 * ofrece exportación e impresión.
 */
@Component({
  selector: 'app-reportes',
  imports: [CurrencyPipe, DatePipe, PercentPipe, Skeleton],
  templateUrl: './reportes.html',
  styleUrls: [
    '../shared/explorer.css',
    '../shared/recepcion.css',
    '../shared/administracion.css',
    './reportes.css',
  ],
})
export class Reportes {
  private readonly hotel = inject(HotelDemoService);
  private readonly caja = inject(CajaDemoService);
  readonly referenceDate = inject(DashboardDemoService).referenceDate;
  readonly loading = signal(true);
  readonly error = signal('');
  readonly start = signal(this.referenceDate);
  readonly end = signal(this.nextDay(this.referenceDate));
  readonly range = signal({ start: this.start(), end: this.end() });

  /** Reservas cuyas fechas cruzan el período seleccionado. */
  readonly reservations = computed(() => {
    const { start, end } = this.range();
    return this.hotel.reservations().filter((item) => item.start < end && item.end > start);
  });

  /** Reservas del período que no están canceladas. */
  readonly active = computed(() =>
    this.reservations().filter((item) => item.status !== 'Cancelada'),
  );

  /** Cantidad de reservas canceladas que cruzan el período. */
  readonly cancelled = computed(() => this.reservations().length - this.active().length);

  /** Desglose diario de habitaciones, movimientos programados y cobros. */
  readonly daily = computed(() => {
    const { start, end } = this.range();
    const rooms = this.hotel.rooms.filter((room) => !room.maintenance);
    const reservations = this.hotel.reservations().filter((item) => item.status !== 'Cancelada');
    const rows = [];
    for (let date = start; date < end; date = this.nextDay(date)) {
      const tomorrow = this.nextDay(date);
      const used = rooms.filter((room) => !this.hotel.isAvailable(room, date, tomorrow, 1)).length;
      rows.push({
        date,
        used,
        capacity: rooms.length,
        arrivals: reservations.filter((item) => item.start === date).length,
        departures: reservations.filter((item) => item.end === date).length,
        collected:
          this.receipts()
            .filter((receipt) => receipt.date === date)
            .reduce((sum, receipt) => sum + receipt.totalCents, 0) / 100,
      });
    }
    return rows;
  });

  /** Ocupación prevista calculada a partir de las noches operativas del período. */
  readonly occupancy = computed(() => {
    const used = this.daily().reduce((sum, row) => sum + row.used, 0);
    const capacity = this.daily().reduce((sum, row) => sum + row.capacity, 0);
    return { used, capacity, rate: capacity ? used / capacity : 0 };
  });

  /** Número e importe de cobros agrupados por método de pago. */
  readonly paymentMethods = computed(() =>
    (['Efectivo', 'Tarjeta', 'Transferencia'] as const).map((method) => {
      const receipts = this.receipts().filter((receipt) => receipt.method === method);
      return {
        method,
        count: receipts.length,
        amount: receipts.reduce((sum, receipt) => sum + receipt.totalCents, 0) / 100,
      };
    }),
  );

  /** Comprobantes cuya fecha de emisión está dentro del período. */
  readonly receipts = computed(() => {
    const { start, end } = this.range();
    return this.caja.receipts().filter((receipt) => receipt.date >= start && receipt.date < end);
  });

  /** Importe total cobrado en el período. */
  readonly collected = computed(
    () => this.receipts().reduce((sum, receipt) => sum + receipt.totalCents, 0) / 100,
  );

  /** Parte cobrada que corresponde a mesas y pedidos de restaurante. */
  readonly restaurant = computed(
    () =>
      this.receipts().reduce((sum, receipt) => {
        if (receipt.account.type === 'Restaurante') {
          return sum + receipt.totalCents;
        }
        return (
          sum +
          receipt.account.charges
            .filter((line) => line.label.startsWith('Mesa ') || /^PED-\d+ · /.test(line.label))
            .reduce((total, line) => total + line.unitCents * line.quantity, 0)
        );
      }, 0) / 100,
  );

  /** Carga los datos compartidos que se necesitan para calcular los indicadores. */
  async ngOnInit() {
    await Promise.all([this.hotel.loadReservations(), this.hotel.loadTables()]);
    this.loading.set(false);
  }

  /** Valida el rango de fechas y actualiza el período que usan los cálculos. */
  apply() {
    const start = this.start();
    const end = this.end();
    const days = (Date.parse(end) - Date.parse(start)) / 86400000;
    if (!validStayDates(start, end) || days > 366) {
      this.error.set('Elige un período de 1 a 366 días con la fecha final posterior a la inicial.');
      return;
    }
    this.error.set('');
    this.range.set({ start, end });
  }

  /** Abre la impresión del reporte con el período aplicado. */
  print() {
    window.print();
  }

  /** Descarga el desglose diario y los métodos de pago en un archivo CSV. */
  exportCsv() {
    const { start, end } = this.range();
    const rows = [
      'Fecha,Habitaciones comprometidas,Habitaciones operativas,Llegadas,Salidas,Cobrado USD',
      ...this.daily().map((row) =>
        [
          row.date,
          row.used,
          row.capacity,
          row.arrivals,
          row.departures,
          row.collected.toFixed(2),
        ].join(','),
      ),
      '',
      'Metodo,Cantidad,Monto USD',
      ...this.paymentMethods().map((row) =>
        [row.method, row.count, row.amount.toFixed(2)].join(','),
      ),
    ];
    const url = URL.createObjectURL(
      new Blob(['\uFEFF', rows.join('\r\n')], { type: 'text/csv;charset=utf-8' }),
    );
    const link = document.createElement('a');
    link.href = url;
    link.download = 'HotelHub-reportes-' + start + '-a-' + end + '.csv';
    link.click();
    setTimeout(() => URL.revokeObjectURL(url), 1000);
  }

  /** Obtiene el día siguiente en UTC para recorrer el período sin cambios de zona horaria. */
  private nextDay(date: string): string {
    return new Date(Date.parse(date) + 86400000).toISOString().slice(0, 10);
  }
}
