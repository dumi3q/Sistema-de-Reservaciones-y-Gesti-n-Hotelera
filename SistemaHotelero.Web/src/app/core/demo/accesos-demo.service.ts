import { AuditoriaDemoService } from './auditoria-demo.service';
import { inject, Injectable, signal } from '@angular/core';
import { permissionSeed, roleSeed } from './accesos-demo.data';
import { normalizeReservationSearch } from './reservation-demo';

/** Datos de una cuenta del personal que se usa en el directorio de prueba. */
export interface StaffUser {
  id: number;
  username: string;
  email: string;
  active: boolean;
  roleIds: number[];
}

/** Rol que agrupa los permisos asignables a usuarios de la demo. */
export interface StaffRole {
  id: number;
  name: string;
  description: string;
  permissions: string[];
}

/**
 * Gestiona los usuarios, roles y permisos de la demo. AdministrarAccesos utiliza su estado y
 * AuditoriaDemoService recibe los cambios guardados.
 */
@Injectable({ providedIn: 'root' })
export class AccesosDemoService {
  private readonly audit = inject(AuditoriaDemoService);
  readonly administratorId = 1;
  readonly guestRoleId = 7;
  readonly permissions = permissionSeed;
  private readonly roleState = signal<StaffRole[]>(roleSeed);
  readonly roles = this.roleState.asReadonly();
  private readonly userState = signal<StaffUser[]>([
    { id: 1, username: 'admin.hotel', email: 'admin@hotelhub.example', active: true, roleIds: [1] },
    {
      id: 2,
      username: 'recepcion.norte',
      email: 'recepcion@hotelhub.example',
      active: true,
      roleIds: [3],
    },
    { id: 3, username: 'caja.turno', email: 'caja@hotelhub.example', active: true, roleIds: [4] },
    {
      id: 4,
      username: 'restaurante.salon',
      email: 'restaurante@hotelhub.example',
      active: true,
      roleIds: [5],
    },
    {
      id: 5,
      username: 'supervision',
      email: 'supervision@hotelhub.example',
      active: true,
      roleIds: [2, 4],
    },
    {
      id: 6,
      username: 'servicios.turno',
      email: 'servicios@hotelhub.example',
      active: false,
      roleIds: [6],
    },
  ]);
  readonly users = this.userState.asReadonly();

  /** Simula el tiempo de carga del directorio sin consultar una API. */
  async load(): Promise<void> {
    await new Promise((resolve) => setTimeout(resolve, 450));
  }

  /**
   * Crea o actualiza un usuario y comprueba sus roles y la existencia de un administrador
   * activo.
   */
  saveUser(values: Omit<StaffUser, 'id'>, id: number | null): string | null {
    const username = values.username.trim();
    const email = values.email.trim();
    const roleIds = [...new Set(values.roleIds)];
    if (
      !username ||
      username.length > 50 ||
      email.length > 150 ||
      !/^\S+@[^\s@]+\.[^\s@]+$/.test(email)
    ) {
      return 'Ingresa un nombre de usuario y un correo válido.';
    }
    if (
      !roleIds.length ||
      roleIds.some(
        (roleId) => roleId === this.guestRoleId || !this.roles().some((role) => role.id === roleId),
      )
    ) {
      return 'Selecciona al menos un rol del personal.';
    }
    const users = this.users();
    if (
      users.some(
        (user) =>
          user.id !== id &&
          (normalizeReservationSearch(user.username) === normalizeReservationSearch(username) ||
            user.email.toLowerCase() === email.toLowerCase()),
      )
    ) {
      return 'Ese nombre de usuario o correo ya está registrado.';
    }
    if (id !== null && !users.some((user) => user.id === id)) {
      return 'Ese usuario ya no existe.';
    }
    const user = { ...values, id: id ?? users.length + 1, username, email, roleIds };
    const next =
      id === null ? [...users, user] : users.map((item) => (item.id === id ? user : item));
    if (!next.some((item) => item.active && item.roleIds.includes(this.administratorId))) {
      return 'Debe quedar al menos una cuenta de administrador activa.';
    }
    this.userState.set(next);
    this.audit.record(
      'Accesos',
      id === null ? 'Usuario creado' : 'Usuario actualizado',
      'Usuario ' + user.id,
      (user.active ? 'Activo' : 'Inactivo') + ' · ' + roleIds.length + ' roles asignados',
    );
    return null;
  }

  /** Crea o actualiza un rol después de comprobar su nombre y sus permisos. */
  saveRole(values: Omit<StaffRole, 'id'>, id: number | null): string | null {
    const name = values.name.trim();
    const description = values.description.trim();
    const permissions = [...new Set(values.permissions)];
    const roles = this.roles();
    if (!name || name.length > 50 || description.length > 200) {
      return 'Revisa el nombre y la descripción.';
    }
    if (id === this.administratorId) {
      return 'El rol Administrador conserva todos los permisos de la demo.';
    }
    if (id !== null && !roles.some((role) => role.id === id)) {
      return 'Ese rol ya no existe.';
    }
    if (
      roles.some(
        (role) =>
          role.id !== id &&
          normalizeReservationSearch(role.name) === normalizeReservationSearch(name),
      )
    ) {
      return 'Ya existe un rol con ese nombre.';
    }
    if (
      !permissions.length ||
      permissions.some((code) => !this.permissions.some((item) => item.code === code))
    ) {
      return 'Selecciona al menos un permiso válido.';
    }
    const role = { id: id ?? roles.length + 1, name, description, permissions };
    this.roleState.set(
      id === null ? [...roles, role] : roles.map((item) => (item.id === id ? role : item)),
    );
    this.audit.record(
      'Accesos',
      id === null ? 'Rol creado' : 'Rol actualizado',
      'Rol ' + role.id,
      permissions.length + ' permisos asignados',
    );
    return null;
  }
}
