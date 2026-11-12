import { Component, input } from '@angular/core';

/**
 * Muestra un marcador visual de carga del alto solicitado. Las pantallas lo usan mientras sus
 * servicios preparan los datos.
 */
@Component({
  selector: 'app-skeleton',
  standalone: true,
  templateUrl: './skeleton.html',
  styleUrl: './skeleton.css',
})
export class Skeleton {
  readonly height = input(120);
}
