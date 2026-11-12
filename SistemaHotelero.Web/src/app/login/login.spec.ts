import { provideRouter } from '@angular/router';
import { TestBed } from '@angular/core/testing';
import { Login } from './login';
describe('Login', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Login],
      providers: [provideRouter([])],
    }).compileComponents();
  });
  it('rejects invalid credentials without entering the loading state', async () => {
    const fixture = TestBed.createComponent(Login);
    await fixture.componentInstance.submit();
    expect(fixture.componentInstance.form.touched).toBe(true);
    expect(fixture.componentInstance.loading()).toBe(false);
    expect(fixture.componentInstance.message()).toBe('');
  });
  it('keeps the password masked until the visibility button is selected', () => {
    const fixture = TestBed.createComponent(Login);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('#password').type).toBe('password');
    fixture.nativeElement.querySelector('.reveal').click();
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('#password').type).toBe('text');
  });
});
