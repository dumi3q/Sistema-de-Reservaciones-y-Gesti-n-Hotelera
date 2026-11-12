import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { Registro } from './registro';

describe('Guest registration preview', () => {
  beforeEach(() =>
    TestBed.configureTestingModule({ imports: [Registro], providers: [provideRouter([])] }),
  );
  it('rejects mismatched passwords and does not claim to create an account', async () => {
    const page = TestBed.createComponent(Registro).componentInstance;
    page.form.setValue({
      names: 'Ana',
      surnames: 'Mora',
      email: 'ana@example.com',
      password: 'example-pass',
      confirm: 'different-pass',
    });
    await page.submit();
    expect(page.form.hasError('passwordMismatch')).toBe(true);
    expect(page.loading()).toBe(false);
    expect(page.message()).toBe('');
  });
  it('validates email and minimum password length', () => {
    const page = TestBed.createComponent(Registro).componentInstance;
    page.form.setValue({
      names: 'Ana',
      surnames: 'Mora',
      email: 'not-an-email',
      password: 'short',
      confirm: 'short',
    });
    expect(page.form.valid).toBe(false);
    page.form.patchValue({
      email: 'ana@example.com',
      password: 'example-pass',
      confirm: 'example-pass',
    });
    expect(page.form.valid).toBe(true);
  });
});
