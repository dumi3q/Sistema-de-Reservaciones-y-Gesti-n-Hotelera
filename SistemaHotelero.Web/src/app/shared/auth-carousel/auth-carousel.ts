import { Component, OnDestroy, OnInit, signal } from '@angular/core';

/**
 * Presenta el carrusel de Login y Registro. Coordina la rotación automática con el foco, la
 * interacción y la preferencia de movimiento reducido.
 */
@Component({
  selector: 'app-auth-carousel',
  standalone: true,
  templateUrl: './auth-carousel.html',
  styleUrls: ['./auth-carousel.css', './carousel-controls.css'],
})
export class AuthCarousel implements OnInit, OnDestroy {
  readonly active = signal(0);
  readonly paused = signal(false);
  readonly hovered = signal(false);
  readonly focused = signal(false);
  readonly slides = [
    {
      label: 'El hotel',
      eyebrow: 'UN LUGAR PARA SENTIRTE BIEN',
      title: 'Tu próxima historia empieza aquí.',
      description:
        'Un refugio entre el verde, la calma y los pequeños detalles que hacen una gran estancia.',
    },
    {
      label: 'La experiencia HotelHub',
      eyebrow: 'MENOS GESTIÓN. MÁS HOSPITALIDAD.',
      title: 'El arte de recibir. La tranquilidad de tenerlo todo.',
      description:
        'Reservaciones, huéspedes y experiencias. Un solo lugar para cuidar lo que importa.',
    },
    {
      label: 'Jardín y descanso',
      eyebrow: 'HAZ UNA PAUSA. QUÉDATE UN POCO MÁS.',
      title: 'Hay momentos que merecen disfrutarse sin prisa.',
      description:
        'Espacios para desconectar, sabores para compartir y una bienvenida hecha para ti.',
    },
  ];
  private timer?: ReturnType<typeof setInterval>;

  /**
   * Inicia la rotación y respeta el movimiento reducido, el foco y la visibilidad de la
   * pestaña.
   */
  ngOnInit() {
    this.paused.set(window.matchMedia?.('(prefers-reduced-motion: reduce)').matches ?? false);
    this.timer = setInterval(() => {
      if (!this.paused() && !this.hovered() && !this.focused() && !document.hidden) {
        this.active.update((value) => (value + 1) % this.slides.length);
      }
    }, 6500);
  }

  /** Muestra la diapositiva elegida y pausa la rotación automática. */
  select(index: number) {
    this.active.set(index);
    this.paused.set(true);
  }

  /** Deja de considerar enfocado el carrusel cuando el foco sale de sus controles. */
  onFocusOut(event: FocusEvent) {
    if (!(event.currentTarget as HTMLElement).contains(event.relatedTarget as Node | null)) {
      this.focused.set(false);
    }
  }

  /** Detiene el temporizador cuando la pantalla se destruye. */
  ngOnDestroy() {
    if (this.timer) {
      clearInterval(this.timer);
    }
  }
}
