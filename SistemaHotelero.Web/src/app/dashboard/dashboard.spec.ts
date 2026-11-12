import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { Dashboard } from './dashboard';
import { DashboardDemoService } from '../core/demo/dashboard-demo.service';

describe('Hotel dashboard', () => {
  beforeEach(() =>
    TestBed.configureTestingModule({ imports: [Dashboard], providers: [provideRouter([])] }),
  );
  it('reconciles the operational snapshot with all 96 rooms', () => {
    const page = TestBed.createComponent(Dashboard).componentInstance;
    const snapshot = page.availability();
    expect(snapshot.occupied + snapshot.reserved + snapshot.available + snapshot.maintenance).toBe(
      96,
    );
    expect(
      page.hotel.buildings.reduce(
        (sum, building) => sum + page.availableInBuilding(building.code),
        0,
      ),
    ).toBe(snapshot.available);
  });
  it('changes the arrivals with the selected calendar date and handles empty days', () => {
    const page = TestBed.createComponent(Dashboard).componentInstance;
    page.events.set(TestBed.inject(DashboardDemoService).events);
    expect(page.count('Llegada')).toBe(4);
    page.selectDay('2026-12-11');
    expect(page.count('Llegada')).toBe(2);
    page.selectDay('2027-01-01');
    expect(page.dayEvents()).toHaveLength(0);
    expect(page.calendarDays()).toHaveLength(42);
    expect(page.calendarDays().find((day) => day.key === '2027-01-01')?.inMonth).toBe(true);
  });
});
