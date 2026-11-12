import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PerfilDemoService } from '../core/demo/perfil-demo.service';

/**
 * Edita el perfil de prueba indicado por la ruta. PerfilDemoService separa el estado del
 * huésped y el del personal.
 */
@Component({
  selector: 'app-mi-perfil',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './mi-perfil.html',
  styleUrls: [
    '../shared/explorer.css',
    '../shared/recepcion.css',
    '../shared/administracion.css',
    './mi-perfil.css',
  ],
})
export class MiPerfil {
  private readonly demo = inject(PerfilDemoService);
  readonly staff = inject(ActivatedRoute).snapshot.data['staff'] === true;
  readonly message = signal('');
  readonly error = signal('');
  readonly form = new FormGroup({
    name: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(100)],
    }),
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.email, Validators.maxLength(150)],
    }),
    phone: new FormControl('', {
      nonNullable: true,
      validators: [Validators.pattern(/^[+\d ().-]{7,25}$/)],
    }),
  });

  /** Rellena el formulario con el perfil correspondiente a la ruta. */
  ngOnInit() {
    this.reset();
  }

  /** Recupera los valores guardados y limpia los avisos del formulario. */
  reset() {
    this.form.reset(this.staff ? this.demo.staffProfile() : this.demo.profile());
    this.error.set('');
    this.message.set('');
  }

  /** Valida y guarda los datos en el perfil del huésped o del personal. */
  save() {
    this.message.set('');
    const values = this.form.getRawValue();
    if (
      this.form.invalid ||
      !values.name.trim() ||
      (values.phone && values.phone.replace(/\D/g, '').length < 7)
    ) {
      this.error.set('Revisa el nombre, el correo y el número de teléfono.');
      return;
    }
    const profile = {
      name: values.name.trim(),
      email: values.email.trim(),
      phone: values.phone.trim(),
    };
    if (this.staff) {
      this.demo.saveStaff(profile);
    } else {
      this.demo.save(profile);
    }
    this.reset();
    this.message.set('Perfil de prueba guardado.');
  }
}
