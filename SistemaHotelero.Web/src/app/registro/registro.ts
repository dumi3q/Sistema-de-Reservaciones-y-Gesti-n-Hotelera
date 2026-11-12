import { Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AuthCarousel } from '../shared/auth-carousel/auth-carousel';

/** Comprueba que la contraseña y su confirmación coincidan en el formulario de registro. */
export function passwordsMatch(control: AbstractControl) {
  return control.get('password')?.value === control.get('confirm')?.value
    ? null
    : { passwordMismatch: true };
}

/**
 * Valida los datos del formulario de registro de la demo, incluida la coincidencia de
 * contraseñas. La vista conserva la ruta de reserva al volver al acceso.
 */
@Component({
  selector: 'app-registro',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, AuthCarousel],
  templateUrl: './registro.html',
  styleUrls: ['../login/login.css', '../login/auth-access.css', './registro.css'],
})
export class Registro {
  readonly reservationContext = inject(ActivatedRoute).snapshot.queryParamMap.has('habitacion');
  readonly visible = signal(false);
  readonly loading = signal(false);
  readonly message = signal('');
  readonly form = new FormGroup(
    {
      names: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.maxLength(100)],
      }),
      surnames: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.maxLength(100)],
      }),
      email: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.email],
      }),
      password: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.minLength(8)],
      }),
      confirm: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    },
    { validators: passwordsMatch },
  );

  /** Valida el formulario y muestra el resultado del registro simulado. */
  async submit() {
    this.form.markAllAsTouched();
    if (this.form.invalid || this.loading()) {
      return;
    }
    this.loading.set(true);
    this.message.set('');
    await new Promise((resolve) => setTimeout(resolve, 600));
    this.loading.set(false);
    this.message.set(
      'El formulario está listo para conectarse al servicio de registro. Por ahora no se creó una cuenta ni se guardaron tus datos.',
    );
    this.form.controls.password.reset();
    this.form.controls.confirm.reset();
  }
}
