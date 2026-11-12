import { provideRouter, Router } from '@angular/router';
import { TestBed } from '@angular/core/testing';
import { HotelDemoService } from '../core/demo/hotel-demo.service';
import { Reservar } from './reservar';

describe('Reservation explorer', () => {
  beforeEach(() =>
    TestBed.configureTestingModule({ imports: [Reservar], providers: [provideRouter([])] }),
  );

  it('keeps building and floor counts consistent and resets a room from another building', () => {
    const page = TestBed.createComponent(Reservar).componentInstance;
    page.rooms.set(TestBed.inject(HotelDemoService).rooms);
    expect(
      page.buildings.reduce((sum, building) => sum + page.buildingAvailable(building), 0),
    ).toBe(page.totalAvailable());
    expect(page.floors.reduce((sum, floor) => sum + page.floorAvailable(floor), 0)).toBe(
      page.buildingAvailable(page.buildings[0]),
    );
    page.chooseRoom(page.floorRooms()[0]);
    page.chooseBuilding('B');
    expect(page.selectedRoom()).toBeNull();
    expect(page.floor()).toBe(1);
    expect(page.floorRooms().every((room) => room.building === 'B')).toBe(true);
  });

  it('does not apply invalid dates and never selects an unavailable room for booking', () => {
    const page = TestBed.createComponent(Reservar).componentInstance;
    page.rooms.set(TestBed.inject(HotelDemoService).rooms);
    const original = page.dates();
    page.filterForm.patchValue({ start: '2026-12-15', end: '2026-12-10' });
    page.search();
    expect(page.dates()).toEqual(original);
    expect(page.error()).not.toBe('');
    const unavailable = page.rooms().find((room) => room.maintenance)!;
    const navigate = vi.spyOn(TestBed.inject(Router), 'navigate');
    page.selectForReservation(unavailable);
    expect(navigate).not.toHaveBeenCalled();
  });
});
