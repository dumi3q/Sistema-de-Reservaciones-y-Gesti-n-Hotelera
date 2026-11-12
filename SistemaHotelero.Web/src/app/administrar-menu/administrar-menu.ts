import { CurrencyPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MenuDemoService, MenuItem } from '../core/demo/menu-demo.service';
import { normalizeReservationSearch } from '../core/demo/reservation-demo';
import { AdminNav } from '../shared/admin-nav/admin-nav';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Edita el catálogo del restaurante con MenuDemoService, que comparte los platillos con la
 * carta y las comandas.
 */
@Component({
  selector: 'app-administrar-menu',
  imports: [CurrencyPipe, ReactiveFormsModule, RouterLink, AdminNav, Skeleton],
  templateUrl: './administrar-menu.html',
  styleUrls: [
    '../shared/explorer.css',
    '../shared/recepcion.css',
    '../shared/operacion.css',
    '../shared/administracion.css',
  ],
})
export class AdministrarMenu {
  private readonly menu = inject(MenuDemoService);
  readonly categories = this.menu.categories;
  readonly loading = signal(true);
  readonly query = signal('');
  readonly category = signal('');
  readonly selectedId = signal<number | null>(null);
  readonly creating = signal(false);
  readonly error = signal('');
  readonly notice = signal('');

  /** Platillo correspondiente a la selección del editor. */
  readonly selected = computed(() =>
    this.menu.items().find((item) => item.id === this.selectedId()),
  );

  /** Platillos que coinciden con la categoría y el texto buscado. */
  readonly filtered = computed(() => {
    const query = normalizeReservationSearch(this.query().trim());
    return this.menu
      .items()
      .filter(
        (item) =>
          (!this.category() || this.category() === item.category) &&
          normalizeReservationSearch(item.name + ' ' + item.category).includes(query),
      );
  });

  readonly form = new FormGroup({
    name: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(80)],
    }),
    description: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(300)],
    }),
    category: new FormControl(this.categories[0], {
      nonNullable: true,
      validators: [Validators.required],
    }),
    price: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0.01), Validators.max(100000)],
    }),
    minutes: new FormControl(15, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(1), Validators.max(300)],
    }),
    available: new FormControl(true, { nonNullable: true }),
  });

  /** Espera la carga inicial del catálogo antes de habilitar la edición. */
  async ngOnInit() {
    await this.menu.load();
    this.loading.set(false);
  }

  /** Carga un platillo existente o prepara el formulario para crear uno. */
  edit(item?: MenuItem) {
    this.selectedId.set(item?.id ?? null);
    this.creating.set(!item);
    this.form.reset({
      name: item?.name ?? '',
      description: item?.description ?? '',
      category: item?.category ?? this.categories[0],
      price: item?.price ?? 0,
      minutes: item?.minutes ?? 15,
      available: item?.available !== false,
    });
    this.error.set('');
    this.notice.set('');
  }

  /** Valida el formulario, guarda el platillo y actualiza la selección. */
  save() {
    if (!this.selected() && !this.creating()) {
      return;
    }
    this.form.markAllAsTouched();
    this.notice.set('');
    if (this.form.invalid) {
      this.error.set('Completa el nombre, descripción, precio y tiempo de preparación.');
      return;
    }
    const values = this.form.getRawValue();
    const error = this.menu.save(values, this.selectedId() ?? undefined);
    this.error.set(error ?? '');
    if (error) {
      return;
    }
    const saved = this.menu.items().find((item) => item.name === values.name.trim());
    if (saved) {
      this.edit(saved);
    }
    this.notice.set('Platillo guardado en el menú de prueba.');
  }
}
