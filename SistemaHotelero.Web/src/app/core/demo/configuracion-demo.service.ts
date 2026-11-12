import { AuditoriaDemoService } from './auditoria-demo.service';
import { inject, Injectable, signal } from '@angular/core';

/** Modalidades de anticipo admitidas por la configuración de la demo. */
export type DepositMode = 'Porcentaje' | 'PrimeraNoche' | 'SinAnticipo';

/** Valores que Configuración general carga al restaurar las opciones de prueba. */
export const defaultHotelSettings = {
  checkIn: '15:00',
  checkOut: '12:00',
  deposit: 30,
  depositMode: 'Porcentaje' as DepositMode,
  lodgingTax: 13,
  restaurantTax: 13,
};

/**
 * Comparte la configuración general entre Administración y las vistas de reserva y pago.
 * Registra los cambios en AuditoriaDemoService.
 */
@Injectable({ providedIn: 'root' })
export class ConfiguracionDemoService {
  private readonly audit = inject(AuditoriaDemoService);
  private readonly state = signal({ ...defaultHotelSettings });
  readonly settings = this.state.asReadonly();

  /** Calcula el anticipo según la modalidad guardada y redondea el resultado a centavos. */
  estimateDeposit(nightly: number, nights: number): number {
    const settings = this.settings();
    if (settings.depositMode === 'SinAnticipo') {
      return 0;
    }
    if (settings.depositMode === 'PrimeraNoche') {
      return nightly;
    }
    return Math.round(nightly * nights * settings.deposit) / 100;
  }

  /** Guarda la configuración de la sesión y registra el cambio en auditoría. */
  save(settings: typeof defaultHotelSettings) {
    this.state.set({ ...settings });
    this.audit.record(
      'Administración',
      'Configuración guardada',
      'Hotel',
      'Check-in ' +
        settings.checkIn +
        ' · Check-out ' +
        settings.checkOut +
        ' · Anticipo ' +
        settings.depositMode +
        ' · ' +
        settings.deposit +
        ' % · Impuestos de referencia ' +
        settings.lodgingTax +
        ' % / ' +
        settings.restaurantTax +
        ' %',
    );
  }
}
