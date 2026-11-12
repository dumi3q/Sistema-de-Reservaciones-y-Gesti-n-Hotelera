import { AuditoriaDemoService } from './auditoria-demo.service';
import { inject, Injectable, signal } from '@angular/core';

/** Datos del platillo que comparten el catálogo, la carta pública y las comandas. */
export interface MenuItem {
  id: number;
  category: string;
  name: string;
  description: string;
  price: number;
  minutes: number;
  available?: boolean;
}

/**
 * Mantiene el catálogo de platillos que usan Administración, la carta y las comandas. Conserva
 * el catálogo original para valorar líneas antiguas.
 */
@Injectable({ providedIn: 'root' })
export class MenuDemoService {
  private readonly audit = inject(AuditoriaDemoService);
  readonly categories = ['Desayunos', 'Entradas', 'Platos fuertes', 'Postres', 'Bebidas'];
  private readonly initialItems: MenuItem[] = [
    {
      id: 1,
      category: 'Desayunos',
      name: 'Gallo pinto completo',
      description: 'Gallo pinto, huevos, plátano, natilla y tortilla.',
      price: 9.5,
      minutes: 15,
    },
    {
      id: 2,
      category: 'Desayunos',
      name: 'Pancakes con frutas',
      description: 'Tres pancakes con frutas de temporada y miel.',
      price: 8,
      minutes: 12,
    },
    {
      id: 3,
      category: 'Entradas',
      name: 'Patacones con frijoles',
      description: 'Patacones con frijol molido y pico de gallo.',
      price: 7,
      minutes: 10,
    },
    {
      id: 4,
      category: 'Entradas',
      name: 'Ceviche de pescado',
      description: 'Pescado blanco, limón, culantro y chile dulce.',
      price: 10,
      minutes: 10,
    },
    {
      id: 5,
      category: 'Platos fuertes',
      name: 'Casado con pollo',
      description: 'Arroz, frijoles, ensalada, plátano y pollo.',
      price: 13,
      minutes: 20,
    },
    {
      id: 6,
      category: 'Platos fuertes',
      name: 'Filete de res',
      description: 'Filete a la parrilla con puré y vegetales.',
      price: 24,
      minutes: 25,
    },
    {
      id: 7,
      category: 'Platos fuertes',
      name: 'Pasta al pesto',
      description: 'Pasta con pesto de albahaca y parmesano.',
      price: 14,
      minutes: 18,
    },
    {
      id: 8,
      category: 'Postres',
      name: 'Tres leches',
      description: 'Queque tres leches casero.',
      price: 6,
      minutes: 5,
    },
    {
      id: 9,
      category: 'Bebidas',
      name: 'Batido de fruta',
      description: 'En agua o en leche.',
      price: 4,
      minutes: 5,
    },
    {
      id: 10,
      category: 'Bebidas',
      name: 'Café chorreado',
      description: 'Café costarricense.',
      price: 3,
      minutes: 5,
    },
  ];
  private readonly itemState = signal(this.initialItems);
  readonly items = this.itemState.asReadonly();

  /**
   * Busca un platillo en el catálogo original para conservar el importe de las comandas
   * precargadas.
   */
  originalItem(name: string): MenuItem | undefined {
    return this.initialItems.find((item) => item.name === name);
  }

  /** Crea o actualiza un platillo después de validar sus datos y su nombre. */
  save(item: Omit<MenuItem, 'id'>, id?: number): string | null {
    const name = item.name.trim();
    const description = item.description.trim();
    if (!name || !description || !this.categories.includes(item.category)) {
      return 'Completa el nombre, la descripción y la categoría.';
    }
    if (
      !Number.isFinite(item.price) ||
      item.price < 0.01 ||
      item.price > 100000 ||
      !Number.isInteger(item.minutes) ||
      item.minutes < 1 ||
      item.minutes > 300
    ) {
      return 'Revisa el precio y el tiempo de preparación.';
    }
    if (
      this.items().some(
        (other) => other.id !== id && other.name.toLowerCase() === name.toLowerCase(),
      )
    ) {
      return 'Ya hay un platillo con ese nombre.';
    }
    if (id !== undefined && !this.items().some((other) => other.id === id)) {
      return 'Ese platillo ya no está en el menú.';
    }
    const updated = {
      ...item,
      id: id ?? Math.max(0, ...this.items().map((other) => other.id)) + 1,
      name,
      description,
      price: Math.round(item.price * 100) / 100,
    };
    this.itemState.update((items) =>
      id === undefined
        ? [...items, updated]
        : items.map((other) => (other.id === id ? updated : other)),
    );
    this.audit.record(
      'Restaurante',
      id === undefined ? 'Platillo creado' : 'Platillo actualizado',
      'Platillo ' + updated.id,
      'Precio USD ' +
        updated.price +
        ' · ' +
        (updated.available === false ? 'Agotado' : 'Disponible'),
    );
    return null;
  }

  /** Devuelve el catálogo compartido tras una pausa que simula la carga inicial. */
  async load() {
    await new Promise((resolve) => setTimeout(resolve, 500));
    return this.items();
  }
}
