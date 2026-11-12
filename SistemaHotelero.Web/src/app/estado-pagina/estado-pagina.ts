import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

/**
 * Presenta el estado de página no encontrada o acceso denegado según los datos de la ruta de
 * Angular.
 */
@Component({
  selector: 'app-estado-pagina',
  imports: [RouterLink],
  styleUrls: ['../shared/explorer.css', '../shared/recepcion.css'],
  templateUrl: './estado-pagina.html',
})
export class EstadoPagina {
  readonly forbidden = inject(ActivatedRoute).snapshot.data['forbidden'] === true;
}
