// Datos de demostración basados en Roles, Permisos y RolPermisos de Scripts.V2.txt.
export const permissionSeed = [
  {
    description: 'Crear, editar y desactivar usuarios',
    code: 'Usuarios.Gestionar',
  },
  {
    description: 'Asignar roles y permisos',
    code: 'Roles.Gestionar',
  },
  {
    description: 'Editar tipos, amenidades, tarifas e impuestos',
    code: 'Catalogos.Editar',
  },
  {
    description: 'Editar la configuración del hotel',
    code: 'Configuracion.Editar',
  },
  {
    description: 'Consultar la bitácora de auditoría',
    code: 'Auditoria.Ver',
  },
  {
    description: 'Ver reportes y dashboard',
    code: 'Reportes.Ver',
  },
  {
    description: 'Exportar reportes a PDF o Excel',
    code: 'Reportes.Exportar',
  },
  {
    description: 'Autorizar ajustes de cargos',
    code: 'Ajustes.Autorizar',
  },
  {
    description: 'Consultar clientes e historiales',
    code: 'Clientes.Leer',
  },
  {
    description: 'Registrar y editar clientes',
    code: 'Clientes.Gestionar',
  },
  {
    description: 'Consultar habitaciones y disponibilidad',
    code: 'Habitaciones.Leer',
  },
  {
    description: 'Editar habitaciones y cambiar estados',
    code: 'Habitaciones.Editar',
  },
  {
    description: 'Consultar todas las reservaciones',
    code: 'Reservaciones.Leer',
  },
  {
    description: 'Crear reservaciones para clientes',
    code: 'Reservaciones.Crear',
  },
  {
    description: 'Confirmar reservaciones',
    code: 'Reservaciones.Confirmar',
  },
  {
    description: 'Cancelar reservaciones',
    code: 'Reservaciones.Cancelar',
  },
  {
    description: 'Registrar check-in',
    code: 'Estadias.CheckIn',
  },
  {
    description: 'Registrar check-out',
    code: 'Estadias.CheckOut',
  },
  {
    description: 'Consultar cuentas y saldos',
    code: 'Cuentas.Leer',
  },
  {
    description: 'Registrar pagos',
    code: 'Pagos.Registrar',
  },
  {
    description: 'Registrar reversos de pago',
    code: 'Pagos.Revertir',
  },
  {
    description: 'Emitir facturas y comprobantes',
    code: 'Facturas.Emitir',
  },
  {
    description: 'Editar categorías y platillos',
    code: 'Menu.Editar',
  },
  {
    description: 'Crear pedidos para clientes',
    code: 'Pedidos.Crear',
  },
  {
    description: 'Consultar y administrar pedidos',
    code: 'Pedidos.Gestionar',
  },
  {
    description: 'Cambiar el estado de preparación',
    code: 'Pedidos.CambiarEstado',
  },
  {
    description: 'Consultar y editar el perfil propio',
    code: 'Perfil.Propio',
  },
  {
    description: 'Crear, consultar y cancelar reservaciones propias',
    code: 'Reservaciones.Propias',
  },
  {
    description: 'Crear, consultar y cancelar pedidos propios',
    code: 'Pedidos.Propios',
  },
  {
    description: 'Crear, consultar y cancelar solicitudes de servicio propias',
    code: 'Solicitudes.Propias',
  },
  {
    description: 'Ver todas las solicitudes, crearlas y asignarlas',
    code: 'Solicitudes.Gestionar',
  },
  {
    description: 'Ver solicitudes asignadas y cambiar su estado',
    code: 'Solicitudes.Atender',
  },
];

/** Roles iniciales y códigos de permiso que AccesosDemoService carga en la demo. */
export const roleSeed = [
  {
    id: 1,
    name: 'Administrador',
    description: 'Usuarios, roles, catálogos, configuración, reportes y auditoría',
    permissions: [
      'Usuarios.Gestionar',
      'Roles.Gestionar',
      'Catalogos.Editar',
      'Configuracion.Editar',
      'Auditoria.Ver',
      'Reportes.Ver',
      'Reportes.Exportar',
      'Ajustes.Autorizar',
      'Clientes.Leer',
      'Clientes.Gestionar',
      'Habitaciones.Leer',
      'Habitaciones.Editar',
      'Reservaciones.Leer',
      'Reservaciones.Crear',
      'Reservaciones.Confirmar',
      'Reservaciones.Cancelar',
      'Estadias.CheckIn',
      'Estadias.CheckOut',
      'Cuentas.Leer',
      'Pagos.Registrar',
      'Pagos.Revertir',
      'Facturas.Emitir',
      'Menu.Editar',
      'Pedidos.Crear',
      'Pedidos.Gestionar',
      'Pedidos.CambiarEstado',
      'Perfil.Propio',
      'Reservaciones.Propias',
      'Pedidos.Propios',
      'Solicitudes.Propias',
      'Solicitudes.Gestionar',
      'Solicitudes.Atender',
    ],
  },
  {
    id: 2,
    name: 'Supervisor',
    description: 'Reportes operativos, auditoría y autorización de ajustes',
    permissions: [
      'Reportes.Ver',
      'Reportes.Exportar',
      'Auditoria.Ver',
      'Ajustes.Autorizar',
      'Clientes.Leer',
      'Habitaciones.Leer',
      'Reservaciones.Leer',
      'Cuentas.Leer',
      'Solicitudes.Gestionar',
    ],
  },
  {
    id: 3,
    name: 'Recepcionista',
    description: 'Clientes, habitaciones, reservaciones, check-in, check-out y pagos',
    permissions: [
      'Clientes.Leer',
      'Clientes.Gestionar',
      'Habitaciones.Leer',
      'Habitaciones.Editar',
      'Reservaciones.Leer',
      'Reservaciones.Crear',
      'Reservaciones.Confirmar',
      'Reservaciones.Cancelar',
      'Estadias.CheckIn',
      'Estadias.CheckOut',
      'Cuentas.Leer',
      'Pagos.Registrar',
      'Pedidos.Crear',
      'Solicitudes.Gestionar',
    ],
  },
  {
    id: 4,
    name: 'Cajero',
    description: 'Pagos, saldos y comprobantes',
    permissions: [
      'Clientes.Leer',
      'Reservaciones.Leer',
      'Cuentas.Leer',
      'Pagos.Registrar',
      'Pagos.Revertir',
      'Facturas.Emitir',
    ],
  },
  {
    id: 5,
    name: 'Restaurante',
    description: 'Menú, pedidos y estados de preparación',
    permissions: ['Menu.Editar', 'Pedidos.Gestionar', 'Pedidos.CambiarEstado'],
  },
  {
    id: 6,
    name: 'Servicios',
    description: 'Limpieza y mantenimiento: atiende solicitudes de las habitaciones',
    permissions: ['Solicitudes.Atender', 'Habitaciones.Leer'],
  },
  {
    id: 7,
    name: 'Huesped',
    description: 'Perfil, reservaciones, pedidos y solicitudes propias',
    permissions: [
      'Perfil.Propio',
      'Habitaciones.Leer',
      'Reservaciones.Propias',
      'Pedidos.Propios',
      'Solicitudes.Propias',
    ],
  },
];
