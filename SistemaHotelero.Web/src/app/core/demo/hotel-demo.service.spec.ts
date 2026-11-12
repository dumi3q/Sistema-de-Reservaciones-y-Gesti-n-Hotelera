import { HotelDemoService } from './hotel-demo.service';

describe('Hotel demonstration inventory', () => {
  const service = new HotelDemoService();

  it('matches the database distribution: four buildings, six floors and four rooms per floor', () => {
    expect(service.rooms).toHaveLength(96);
    for (const building of service.buildings) {
      for (let floor = 1; floor <= 6; floor++) {
        expect(
          service.rooms.filter((room) => room.building === building.code && room.floor === floor),
        ).toHaveLength(4);
      }
    }
  });

  it('allows checkout and check-in on the same day, but rejects overlapping stays', () => {
    const room = service.rooms.find((room) => !room.maintenance && room.stays.length > 0)!;
    const stay = room.stays[0];
    expect(service.isAvailable(room, stay.start, stay.end, 1)).toBe(false);
    expect(service.isAvailable(room, stay.end, '2026-12-20', 1)).toBe(true);
  });

  it('rejects maintenance, insufficient adult capacity and invalid intervals', () => {
    const maintenance = service.rooms.find((room) => room.maintenance)!;
    const single = service.rooms.find(
      (room) => room.adults === 1 && !room.maintenance && !room.stays.length,
    )!;
    expect(service.isAvailable(maintenance, '2026-12-10', '2026-12-12', 1)).toBe(false);
    expect(service.isAvailable(single, '2026-12-10', '2026-12-12', 2)).toBe(false);
    expect(service.isAvailable(single, '2026-12-12', '2026-12-10', 1)).toBe(false);
  });
});
