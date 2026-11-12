import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthCarousel } from '../shared/auth-carousel/auth-carousel';

/**
 * Valida el formulario de acceso de la demo y permite entrar a la vista pública o al panel de
 * prueba. Usa el enrutador para conservar el recorrido de reserva.
 */
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, AuthCarousel],
  templateUrl: './login.html',
  styleUrls: ['./login.css', './auth-access.css'],
})
export class Login {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  readonly visible = signal(false);
  readonly loading = signal(false);
  readonly message = signal('');
  readonly reservationContext = this.route.snapshot.queryParamMap.has('habitacion');
  readonly form = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  });

  /** Valida el formulario y muestra el resultado de la simulación de acceso. */
  async submit() {
    this.form.markAllAsTouched();
    if (this.form.invalid || this.loading()) {
      return;
    }
    this.loading.set(true);
    this.message.set('');
    await new Promise((resolve) => setTimeout(resolve, 650));
    this.loading.set(false);
    this.message.set(
      'La autenticación aún no está conectada. No se inició ninguna sesión. Puedes continuar como invitado o abrir la demostración del personal.',
    );
  }

  /** Abre la portada para continuar como invitado cuando no hay una operación pendiente. */
  async explore() {
    if (!this.loading()) {
      await this.router.navigateByUrl('/visitar');
    }
  }
}
