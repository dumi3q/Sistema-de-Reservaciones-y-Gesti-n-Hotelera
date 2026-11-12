import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

/**
 * Es el componente raíz de la web. RouterOutlet muestra la pantalla que Angular selecciona para
 * la ruta actual.
 */
@Component({
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
  imports: [RouterOutlet],
})
export class App {
  protected readonly title = signal('SistemaHotelero.Web');
}
