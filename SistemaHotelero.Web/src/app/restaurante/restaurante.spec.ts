import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { HotelDemoService } from '../core/demo/hotel-demo.service';
import { RestaurantTable } from '../core/demo/demo.models';
import { Restaurante } from './restaurante';

describe('Restaurant attention preview', () => {
  const waiting: RestaurantTable = {
    id: 'R1',
    seats: 4,
    area: 'Restaurante',
    status: 'Por tomar orden',
    guest: 'Ana',
    people: 3,
    minutes: 8,
    order: [],
  };
  beforeEach(() =>
    TestBed.configureTestingModule({ imports: [Restaurante], providers: [provideRouter([])] }),
  );

  it('updates the order state and counters without altering other tables', () => {
    const page = TestBed.createComponent(Restaurante).componentInstance;
    const free: RestaurantTable = {
      ...waiting,
      id: 'P1',
      area: 'Piscina',
      status: 'Libre',
      people: 0,
      guest: '',
    };
    page.tables.set([{ ...waiting }, free]);
    expect(page.needsAttention()).toBe(1);
    const hotel = TestBed.inject(HotelDemoService);
    expect(hotel.advanceTable(waiting.id)).toBeNull();
    page.tables.set([{ ...waiting, order: [{ name: 'Casado con pollo', quantity: 3 }] }, free]);
    expect(hotel.advanceTable(waiting.id)).toBe('En preparación');
    expect(page.count('En preparación')).toBe(1);
    expect(page.needsAttention()).toBe(0);
    expect(page.tables()[1]).toEqual(free);
    expect(page.tables()[0].order[0].quantity).toBe(3);
  });

  it('uses alert cards to filter globally and toggles the active filter off', () => {
    const page = TestBed.createComponent(Restaurante).componentInstance;
    page.tables.set([{ ...waiting }]);
    page.chooseArea('Piscina');
    page.showStatus('Por tomar orden');
    expect(page.area()).toBe('Todas las áreas');
    expect(page.statusFilter()).toBe('Por tomar orden');
    expect(page.filteredTables()).toHaveLength(1);
    page.showStatus('Por tomar orden');
    expect(page.statusFilter()).toBe('Todos');
  });
  it('clears the detail when filters hide the selected table', () => {
    const page = TestBed.createComponent(Restaurante).componentInstance;
    page.tables.set([{ ...waiting }]);
    page.chooseTable(waiting);
    page.chooseArea('Piscina');
    expect(page.filteredTables()).toHaveLength(0);
    expect(page.selected()).toBeNull();
  });
});
