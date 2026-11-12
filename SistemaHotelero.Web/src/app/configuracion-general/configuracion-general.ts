import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  ConfiguracionDemoService,
  defaultHotelSettings,
  DepositMode,
} from '../core/demo/configuracion-demo.service';
import { AdminNav } from '../shared/admin-nav/admin-nav';

/**
 * Edita horarios, reglas de anticipo e impuestos de referencia. ConfiguracionDemoService
 * comparte los valores guardados con el flujo de reserva.
 */
@Component({
  selector: 'app-configuracion-general',
  imports: [AdminNav, ReactiveFormsModule],
  templateUrl: './configuracion-general.html',
  styleUrls: [
    '../shared/explorer.css',
    '../shared/recepcion.css',
    '../shared/administracion.css',
    './configuracion-general.css',
  ],
})
export class ConfiguracionGeneral {
  private readonly demo = inject(ConfiguracionDemoService);
  private readonly time = [Validators.required, Validators.pattern(/^(?:[01]\d|2[0-3]):[0-5]\d$/)];
  readonly message = signal('');
  readonly form = new FormGroup({
    checkIn: new FormControl('', { nonNullable: true, validators: this.time }),
    checkOut: new FormControl('', { nonNullable: true, validators: this.time }),
    depositMode: new FormControl<DepositMode>('Porcentaje', { nonNullable: true }),
    lodgingTax: new FormControl(13, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0), Validators.max(100)],
    }),
    restaurantTax: new FormControl(13, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0), Validators.max(100)],
    }),
    deposit: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0), Validators.max(100)],
    }),
  });

  /** Rellena el formulario con los valores de ConfiguracionDemoService. */
  ngOnInit() {
    this.form.reset(this.demo.settings());
  }

  /** Valida horarios y porcentajes antes de guardar la configuración compartida. */
  save() {
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      this.message.set('Revisa los horarios y los porcentajes ingresados.');
      return;
    }
    this.demo.save(this.form.getRawValue());
    this.message.set('Configuración guardada en la demo.');
  }

  /** Carga los valores originales en el formulario para revisarlos antes de guardar. */
  reset() {
    this.form.reset(defaultHotelSettings);
    this.message.set('Valores originales cargados. Guarda los cambios para aplicarlos.');
  }
}
