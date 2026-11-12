import { NgTemplateOutlet } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AccesosDemoService, StaffRole, StaffUser } from '../core/demo/accesos-demo.service';
import { normalizeReservationSearch } from '../core/demo/reservation-demo';
import { AdminNav } from '../shared/admin-nav/admin-nav';
import { Skeleton } from '../shared/skeleton/skeleton';

/**
 * Administra usuarios y roles de prueba mediante AccesosDemoService. Su vista alterna entre el
 * directorio y el editor de permisos.
 */
@Component({
  selector: 'app-administrar-accesos',
  imports: [AdminNav, NgTemplateOutlet, ReactiveFormsModule, Skeleton],
  templateUrl: './administrar-accesos.html',
  styleUrls: [
    '../shared/explorer.css',
    '../shared/recepcion.css',
    '../shared/operacion.css',
    '../shared/administracion.css',
    './administrar-accesos.css',
  ],
})
export class AdministrarAccesos {
  readonly demo = inject(AccesosDemoService);
  readonly loading = signal(true);
  readonly tab = signal<'users' | 'roles'>('users');
  readonly query = signal('');
  readonly permissionQuery = signal('');
  readonly userId = signal<number | null>(null);
  readonly roleId = signal<number | null>(null);
  readonly editing = signal(false);
  readonly error = signal('');
  readonly notice = signal('');
  readonly activeOnly = signal(false);

  /** Roles que pueden asignarse a una cuenta del personal. */
  readonly staffRoles = computed(() =>
    this.demo.roles().filter((role) => role.id !== this.demo.guestRoleId),
  );

  /** Usuarios que coinciden con el filtro de estado y la búsqueda. */
  readonly filteredUsers = computed(() =>
    this.demo
      .users()
      .filter(
        (user) =>
          (!this.activeOnly() || user.active) &&
          this.matches(user.username + ' ' + user.email + ' ' + this.roleNames(user), this.query()),
      ),
  );

  /** Roles que coinciden con el texto ingresado. */
  readonly filteredRoles = computed(() =>
    this.demo
      .roles()
      .filter((role) => this.matches(role.name + ' ' + role.description, this.query())),
  );

  /** Permisos visibles agrupados por el prefijo de su código. */
  readonly visiblePermissions = computed(() => {
    const groups = new Map<string, typeof this.demo.permissions>();
    for (const permission of this.demo.permissions) {
      if (!this.matches(permission.code + ' ' + permission.description, this.permissionQuery())) {
        continue;
      }
      const category = permission.code.split('.')[0];
      const items = groups.get(category) ?? [];
      items.push(permission);
      groups.set(category, items);
    }
    return [...groups.entries()];
  });

  /** Indica cuándo se consulta el rol fijo de administrador. */
  readonly locked = computed(() => this.roleId() === this.demo.administratorId);

  readonly userForm = new FormGroup({
    username: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(50)],
    }),
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email, Validators.maxLength(150)],
    }),
    active: new FormControl(true, { nonNullable: true }),
    roleIds: new FormControl<number[]>([], { nonNullable: true }),
  });

  readonly roleForm = new FormGroup({
    name: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(50)],
    }),
    description: new FormControl('', {
      nonNullable: true,
      validators: [Validators.maxLength(200)],
    }),
    permissions: new FormControl<string[]>([], { nonNullable: true }),
  });

  /** Carga los usuarios, roles y permisos de prueba al abrir la pantalla. */
  async ngOnInit() {
    await this.demo.load();
    this.loading.set(false);
  }

  /** Cambia de pestaña y limpia la búsqueda y el editor anterior. */
  switchTab(tab: 'users' | 'roles') {
    if (tab === this.tab()) {
      return;
    }
    this.tab.set(tab);
    this.query.set('');
    this.permissionQuery.set('');
    this.editing.set(false);
    this.clearMessages();
  }

  /** Abre el formulario de alta correspondiente a la pestaña activa. */
  newRecord() {
    if (this.tab() === 'users') {
      this.editUser();
    } else {
      this.editRole();
    }
  }

  /** Carga un usuario en el editor o prepara un formulario vacío para crearlo. */
  editUser(user?: StaffUser) {
    this.userId.set(user?.id ?? null);
    this.userForm.reset({
      username: user?.username ?? '',
      email: user?.email ?? '',
      active: user?.active ?? true,
      roleIds: user?.roleIds ?? [],
    });
    this.editing.set(true);
    this.clearMessages();
  }

  /** Carga los permisos de un rol o prepara un formulario vacío para un rol nuevo. */
  editRole(role?: StaffRole) {
    this.roleId.set(role?.id ?? null);
    this.roleForm.reset({
      name: role?.name ?? '',
      description: role?.description ?? '',
      permissions: role?.permissions ?? [],
    });
    this.permissionQuery.set('');
    this.editing.set(true);
    this.clearMessages();
  }

  /** Valida el usuario y guarda sus datos y roles con AccesosDemoService. */
  saveUser() {
    this.clearMessages();
    const values = this.userForm.getRawValue();
    const error = this.userForm.invalid
      ? 'Revisa el nombre de usuario y el correo.'
      : this.demo.saveUser(values, this.userId());
    if (error) {
      this.error.set(error);
      return;
    }
    const saved = this.demo.users().find((user) => user.username === values.username.trim());
    this.editUser(saved);
    this.notice.set('Usuario guardado en la demo.');
  }

  /** Valida el rol y guarda sus permisos con AccesosDemoService. */
  saveRole() {
    this.clearMessages();
    if (this.locked()) {
      return;
    }
    const values = this.roleForm.getRawValue();
    const error = this.roleForm.invalid
      ? 'Revisa el nombre del rol.'
      : this.demo.saveRole(values, this.roleId());
    if (error) {
      this.error.set(error);
      return;
    }
    const saved = this.demo.roles().find((role) => role.name === values.name.trim());
    this.editRole(saved);
    this.notice.set('Rol guardado en la demo.');
  }

  /** Agrega o retira una opción en un control que admite varias selecciones. */
  toggle<T>(control: FormControl<T[]>, value: T, checked: boolean) {
    control.setValue(
      checked
        ? [...new Set([...control.value, value])]
        : control.value.filter((item) => item !== value),
    );
  }

  /** Obtiene los nombres de los roles asignados al usuario para mostrarlos en el directorio. */
  roleNames(user: StaffUser): string {
    return this.demo
      .roles()
      .filter((role) => user.roleIds.includes(role.id))
      .map((role) => role.name)
      .join(', ');
  }

  /** Cierra el editor y borra sus mensajes. */
  closeEditor() {
    this.editing.set(false);
    this.clearMessages();
  }

  /** Compara el texto y la consulta sin distinguir mayúsculas ni acentos. */
  private matches(text: string, query: string): boolean {
    return normalizeReservationSearch(text).includes(normalizeReservationSearch(query.trim()));
  }

  /** Limpia los mensajes de error y confirmación del formulario. */
  private clearMessages() {
    this.error.set('');
    this.notice.set('');
  }
}
