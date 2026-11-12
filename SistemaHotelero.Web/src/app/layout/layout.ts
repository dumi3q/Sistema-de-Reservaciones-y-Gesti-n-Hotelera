import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map } from 'rxjs';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

/**
 * Es el marco común de las pantallas públicas y del personal. Observa la ruta de Angular para
 * mostrar el menú correspondiente y aloja la pantalla activa.
 */
@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './layout.html',
  styleUrl: './layout.css',
})
export class Layout {
  private readonly router = inject(Router);
  private readonly url = toSignal(
    this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd),
      map((event) => event.urlAfterRedirects),
    ),
    { initialValue: this.router.url },
  );
  readonly menuOpen = signal(false);

  /** Indica si la ruta corresponde a la web pública. */
  readonly publicMode = computed(() => this.url().startsWith('/visitar'));

  /** Mantiene activa la sección de reservas al abrir el selector del personal. */
  readonly roomPickerActive = computed(() => this.url().split('?')[0] === '/explorar/reservar');
}
