import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

/**
 * Presenta la portada pública y enlaza con habitaciones, restaurante, servicio a la habitación
 * y registro mediante el enrutador de Angular.
 */
@Component({
  selector: 'app-inicio',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './inicio.html',
  styleUrl: './inicio.css',
})
export class Inicio {}
