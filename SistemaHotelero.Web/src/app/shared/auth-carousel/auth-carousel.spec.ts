import { TestBed } from '@angular/core/testing';
import { AuthCarousel } from './auth-carousel';
import { vi } from 'vitest';

describe('Hotel login carousel', () => {
  beforeEach(() => TestBed.configureTestingModule({ imports: [AuthCarousel] }));
  it('starts with the hotel image and lets the visitor choose all three slides', () => {
    const fixture = TestBed.createComponent(AuthCarousel);
    fixture.detectChanges();
    expect(fixture.componentInstance.active()).toBe(0);
    fixture.componentInstance.select(1);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.mini-dashboard')).toBeTruthy();
    fixture.componentInstance.select(2);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.hotel-photo').getAttribute('src')).toContain(
      'hotel-garden',
    );
    expect(fixture.componentInstance.paused()).toBe(true);
    fixture.destroy();
  });
  it('advances automatically, respects pause, and clears its timer when destroyed', () => {
    vi.useFakeTimers();
    const previous = Object.getOwnPropertyDescriptor(document, 'hidden');
    Object.defineProperty(document, 'hidden', { configurable: true, value: false });
    const fixture = TestBed.createComponent(AuthCarousel);
    try {
      fixture.detectChanges();
      fixture.componentInstance.paused.set(false);
      vi.advanceTimersByTime(6500);
      expect(fixture.componentInstance.active()).toBe(1);
      fixture.componentInstance.paused.set(true);
      vi.advanceTimersByTime(6500);
      expect(fixture.componentInstance.active()).toBe(1);
      fixture.destroy();
      vi.advanceTimersByTime(6500);
      expect(fixture.componentInstance.active()).toBe(1);
    } finally {
      if (previous) Object.defineProperty(document, 'hidden', previous);
      else delete (document as unknown as Record<string, unknown>)['hidden'];
      vi.useRealTimers();
    }
  });
});
