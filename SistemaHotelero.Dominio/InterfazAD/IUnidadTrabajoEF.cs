using SistemaHotelero.Dominio.Entidades;

namespace SistemaHotelero.Dominio.InterfazAD
{
    public interface IUnidadTrabajoEF : IDisposable
    {
        IRepositorioAD<AccesosEstadium> TAccesoEstadia { get; }
        IRepositorioAD<Amenidade> TAmenidad { get; }
        IRepositorioAD<Auditorium> TAuditoria { get; }
        IRepositorioAD<BloqueosHabitacion> TBloqueoHabitacion { get; }
        IRepositorioAD<CargosCuentum> TCargoCuenta { get; }
        IRepositorioAD<CategoriasPlatillo> TCategoriaPlatillo { get; }
        IRepositorioAD<Cliente> TCliente { get; }
        IRepositorioAD<Configuracion> TConfiguracion { get; }
        IRepositorioAD<Cuenta> TCuenta { get; }
        IRepositorioAD<DetalleFactura> TDetalleFactura { get; }
        IRepositorioAD<DetallePedido> TDetallePedido { get; }
        IRepositorioAD<Descuento> TDescuento { get; }
        IRepositorioAD<DescuentoAplicado> TDescuentoAplicado { get; }
        IRepositorioAD<Edificio> TEdificio { get; }
        IRepositorioAD<Estadia> TEstadia { get; }
        IRepositorioAD<Factura> TFactura { get; }
        IRepositorioAD<FotosHabitacion> TFotoHabitacion { get; }
        IRepositorioAD<HabitacionAmenidade> THabitacionAmenidad { get; }
        IRepositorioAD<Habitacione> THabitacion { get; }
        IRepositorioAD<HistorialEstadosHabitacion> THistorialEstadoHabitacion { get; }
        IRepositorioAD<HistorialPedido> THistorialPedido { get; }
        IRepositorioAD<HistorialReservacione> THistorialReservacion { get; }
        IRepositorioAD<HistorialSolicitude> THistorialSolicitud { get; }
        IRepositorioAD<HuespedesReservacion> THuespedReservacion { get; }
        IRepositorioAD<Impuesto> TImpuesto { get; }
        IRepositorioAD<IndisponibilidadAmenidad> TIndisponibilidadAmenidad { get; }
        IRepositorioAD<Mesa> TMesa { get; }
        IRepositorioAD<NotificacionesCorreo> TNotificacionCorreo { get; }
        IRepositorioAD<Pago> TPago { get; }
        IRepositorioAD<PedidosRestaurante> TPedidoRestaurante { get; }
        IRepositorioAD<Permiso> TPermiso { get; }
        IRepositorioAD<Persona> TPersona { get; }
        IRepositorioAD<Platillo> TPlatillo { get; }
        IRepositorioAD<RefreshToken> TRefreshToken { get; }
        IRepositorioAD<ReglasTarifa> TReglaTarifa { get; }
        IRepositorioAD<ReservacionAmenidad> TReservacionAmenidad { get; }
        IRepositorioAD<ReservacionHabitacione> TReservacionHabitacion { get; }
        IRepositorioAD<ReservacionNoch> TReservacionNoche { get; }
        IRepositorioAD<Reservacione> TReservacion { get; }
        IRepositorioAD<ReservacionesMesa> TReservacionMesa { get; }
        IRepositorioAD<ReversosPago> TReversoPago { get; }
        IRepositorioAD<Role> TRol { get; }
        IRepositorioAD<SolicitudesServicio> TSolicitudServicio { get; }
        IRepositorioAD<TipoDocumento> TTipoDocumento { get; }
        IRepositorioAD<TipoPenalizacion> TTipoPenalizacion { get; }
        IRepositorioAD<TiposHabitacion> TTipoHabitacion { get; }
        IRepositorioAD<TiposSolicitud> TTipoSolicitud { get; }
        IRepositorioAD<Usuario> TUsuario { get; }

        // Vistas: solo lectura (Listar, Buscar, ObtenerEntidad, Contar)
        IRepositorioAD<CompensacionAmenidadPendiente> VCompensacionAmenidadPendiente { get; }

        // Secuencias de SQL Server (NEXT VALUE FOR). Participan en la transacción activa.
        Task<int> SiguienteCodigoReservaAsync();
        Task<int> SiguienteNumeroFacturaAsync();

        int Completar();
        void CompletarTran();
        void EmpezarTransaccion();
        void Rollback();
        void CerrarConexion();
    }
}
