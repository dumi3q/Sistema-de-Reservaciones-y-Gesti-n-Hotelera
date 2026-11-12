import { Injectable, signal } from '@angular/core';

/**
 * Mantiene separados los datos de contacto del huésped y del personal. MiPerfil edita ambos
 * perfiles y las reservas de huésped consultan el primero.
 */
@Injectable({ providedIn: 'root' })
export class PerfilDemoService {
  private readonly state = signal({ name: 'Huésped de prueba', email: '', phone: '' });
  readonly profile = this.state.asReadonly();

  private readonly staffState = signal({ name: 'Administrador de prueba', email: '', phone: '' });
  readonly staffProfile = this.staffState.asReadonly();

  /** Guarda los datos del perfil del personal en la sesión de prueba. */
  saveStaff(profile: ReturnType<typeof this.staffProfile>) {
    this.staffState.set({ ...profile });
  }

  /** Guarda los datos del huésped que usan las vistas públicas y la ficha de contacto. */
  save(profile: ReturnType<typeof this.profile>) {
    this.state.set({ ...profile });
  }
}
