import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
// Separa la navegación del huésped de la del personal y carga cada pantalla al abrirla.
const routes: Routes = [
  { path: 'login', loadComponent: () => import('./login/login').then((module) => module.Login) },
  {
    path: 'registro',
    loadComponent: () => import('./registro/registro').then((module) => module.Registro),
  },
  {
    path: 'visitar',
    loadComponent: () => import('./layout/layout').then((module) => module.Layout),
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadComponent: () => import('./inicio/inicio').then((module) => module.Inicio),
      },
      {
        path: 'habitaciones',
        loadComponent: () => import('./reservar/reservar').then((module) => module.Reservar),
      },
      {
        path: 'confirmar-reserva',
        loadComponent: () =>
          import('./confirmar-reserva/confirmar-reserva').then((module) => module.ConfirmarReserva),
      },
      {
        path: 'pago-reserva',
        loadComponent: () =>
          import('./pago-reserva/pago-reserva').then((module) => module.PagoReserva),
      },
      {
        path: 'mis-facturas/:id',
        loadComponent: () => import('./facturas/facturas').then((module) => module.Facturas),
      },
      {
        path: 'mis-facturas',
        loadComponent: () => import('./facturas/facturas').then((module) => module.Facturas),
      },
      {
        path: 'mi-perfil',
        loadComponent: () => import('./mi-perfil/mi-perfil').then((module) => module.MiPerfil),
      },
      {
        path: 'mis-solicitudes',
        loadComponent: () =>
          import('./solicitudes/solicitudes').then((module) => module.Solicitudes),
      },
      {
        path: 'mis-pedidos',
        loadComponent: () =>
          import('./pedidos-habitacion/pedidos-habitacion').then(
            (module) => module.PedidosHabitacion,
          ),
      },
      {
        path: 'mis-reservaciones',
        loadComponent: () =>
          import('./mis-reservaciones/mis-reservaciones').then((module) => module.MisReservaciones),
      },
      {
        path: 'reservar-mesa',
        loadComponent: () =>
          import('./reservas-mesa/reservas-mesa').then((module) => module.ReservasMesa),
      },
      {
        path: 'servicio-habitacion',
        data: { roomService: true },
        loadComponent: () => import('./menu/menu').then((module) => module.Menu),
      },
      {
        path: 'restaurante',
        loadComponent: () => import('./menu/menu').then((module) => module.Menu),
      },
    ],
  },
  {
    // Rutas del panel de prueba con datos de demostración.
    path: 'explorar',
    loadComponent: () => import('./layout/layout').then((module) => module.Layout),
    children: [
      {
        path: 'inicio',
        loadComponent: () => import('./dashboard/dashboard').then((module) => module.Dashboard),
      },
      {
        path: 'reservaciones/editar/:id',
        loadComponent: () =>
          import('./editar-reserva/editar-reserva').then((module) => module.EditarReserva),
      },
      {
        path: 'reservaciones',
        loadComponent: () =>
          import('./reservaciones/reservaciones').then((module) => module.Reservaciones),
      },
      {
        path: 'mi-perfil',
        data: { staff: true },
        loadComponent: () => import('./mi-perfil/mi-perfil').then((module) => module.MiPerfil),
      },
      {
        path: 'reportes',
        loadComponent: () => import('./reportes/reportes').then((module) => module.Reportes),
      },
      {
        path: 'solicitudes',
        data: { staff: true },
        loadComponent: () =>
          import('./solicitudes/solicitudes').then((module) => module.Solicitudes),
      },
      {
        path: 'estadias',
        loadComponent: () => import('./estadias/estadias').then((module) => module.Estadias),
      },
      {
        path: 'huespedes',
        loadComponent: () => import('./huespedes/huespedes').then((module) => module.Huespedes),
      },
      {
        path: 'reservar',
        loadComponent: () => import('./reservar/reservar').then((module) => module.Reservar),
      },
      {
        path: 'administracion',
        children: [
          { path: '', redirectTo: 'habitaciones', pathMatch: 'full' },
          {
            path: 'habitaciones',
            loadComponent: () =>
              import('./administrar-habitaciones/administrar-habitaciones').then(
                (module) => module.AdministrarHabitaciones,
              ),
          },
          {
            path: 'tarifas',
            loadComponent: () =>
              import('./administrar-tarifas/administrar-tarifas').then(
                (module) => module.AdministrarTarifas,
              ),
          },
          {
            path: 'auditoria',
            loadComponent: () => import('./auditoria/auditoria').then((module) => module.Auditoria),
          },
          {
            path: 'configuracion',
            loadComponent: () =>
              import('./configuracion-general/configuracion-general').then(
                (module) => module.ConfiguracionGeneral,
              ),
          },
          {
            path: 'accesos',
            loadComponent: () =>
              import('./administrar-accesos/administrar-accesos').then(
                (module) => module.AdministrarAccesos,
              ),
          },
          {
            path: 'menu',
            loadComponent: () =>
              import('./administrar-menu/administrar-menu').then(
                (module) => module.AdministrarMenu,
              ),
          },
        ],
      },
      {
        path: 'caja/factura/:id',
        data: { staff: true },
        loadComponent: () => import('./facturas/facturas').then((module) => module.Facturas),
      },
      {
        path: 'caja',
        loadComponent: () => import('./caja/caja').then((module) => module.Caja),
      },
      {
        path: 'restaurante/reservas',
        data: { staff: true },
        loadComponent: () =>
          import('./reservas-mesa/reservas-mesa').then((module) => module.ReservasMesa),
      },
      {
        path: 'restaurante/habitaciones',
        data: { staff: true },
        loadComponent: () =>
          import('./pedidos-habitacion/pedidos-habitacion').then(
            (module) => module.PedidosHabitacion,
          ),
      },
      {
        path: 'restaurante/pedidos',
        loadComponent: () => import('./pedidos/pedidos').then((module) => module.Pedidos),
      },
      {
        path: 'restaurante',
        loadComponent: () =>
          import('./restaurante/restaurante').then((module) => module.Restaurante),
      },
      { path: '', redirectTo: 'inicio', pathMatch: 'full' },
    ],
  },
  { path: '', redirectTo: 'visitar', pathMatch: 'full' },
  {
    path: 'acceso-denegado',
    data: { forbidden: true },
    loadComponent: () =>
      import('./estado-pagina/estado-pagina').then((module) => module.EstadoPagina),
  },
  {
    path: '**',
    loadComponent: () =>
      import('./estado-pagina/estado-pagina').then((module) => module.EstadoPagina),
  },
];

/**
 * Relaciona las direcciones de la web con sus componentes. Organiza las rutas públicas y las
 * del personal bajo Layout.
 */
@NgModule({
  imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' })],
  exports: [RouterModule],
})
export class AppRoutingModule {}
