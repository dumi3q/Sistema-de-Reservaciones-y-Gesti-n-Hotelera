using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SistemaHotelero.AccesoDatos.Contexto;
using SistemaHotelero.Dominio.Entidades;
using SistemaHotelero.Dominio.InterfazAD;

namespace SistemaHotelero.AccesoDatos.Implementaciones
{
    public class UnidadTrabajoEF : IUnidadTrabajoEF
    {
        #region Atributos y Variables

        private HotelContext _Contexto { get; set; }

        public UnidadTrabajoEF(HotelContext Contexto)
        {
            _Contexto = Contexto;
        }

        private IDbContextTransaction? _transaction = null;

        private RepositorioAD<AccesosEstadium>? _TAccesoEstadia;
        private RepositorioAD<Amenidade>? _TAmenidad;
        private RepositorioAD<Auditorium>? _TAuditoria;
        private RepositorioAD<BloqueosHabitacion>? _TBloqueoHabitacion;
        private RepositorioAD<CargosCuentum>? _TCargoCuenta;
        private RepositorioAD<CategoriasPlatillo>? _TCategoriaPlatillo;
        private RepositorioAD<Cliente>? _TCliente;
        private RepositorioAD<Configuracion>? _TConfiguracion;
        private RepositorioAD<Cuenta>? _TCuenta;
        private RepositorioAD<DetalleFactura>? _TDetalleFactura;
        private RepositorioAD<DetallePedido>? _TDetallePedido;
        private RepositorioAD<Descuento>? _TDescuento;
        private RepositorioAD<DescuentoAplicado>? _TDescuentoAplicado;
        private RepositorioAD<Edificio>? _TEdificio;
        private RepositorioAD<Estadia>? _TEstadia;
        private RepositorioAD<Factura>? _TFactura;
        private RepositorioAD<FotosHabitacion>? _TFotoHabitacion;
        private RepositorioAD<HabitacionAmenidade>? _THabitacionAmenidad;
        private RepositorioAD<Habitacione>? _THabitacion;
        private RepositorioAD<HistorialEstadosHabitacion>? _THistorialEstadoHabitacion;
        private RepositorioAD<HistorialPedido>? _THistorialPedido;
        private RepositorioAD<HistorialReservacione>? _THistorialReservacion;
        private RepositorioAD<HistorialSolicitude>? _THistorialSolicitud;
        private RepositorioAD<HuespedesReservacion>? _THuespedReservacion;
        private RepositorioAD<Impuesto>? _TImpuesto;
        private RepositorioAD<IndisponibilidadAmenidad>? _TIndisponibilidadAmenidad;
        private RepositorioAD<Mesa>? _TMesa;
        private RepositorioAD<NotificacionesCorreo>? _TNotificacionCorreo;
        private RepositorioAD<Pago>? _TPago;
        private RepositorioAD<PedidosRestaurante>? _TPedidoRestaurante;
        private RepositorioAD<Permiso>? _TPermiso;
        private RepositorioAD<Persona>? _TPersona;
        private RepositorioAD<Platillo>? _TPlatillo;
        private RepositorioAD<RefreshToken>? _TRefreshToken;
        private RepositorioAD<ReglasTarifa>? _TReglaTarifa;
        private RepositorioAD<ReservacionAmenidad>? _TReservacionAmenidad;
        private RepositorioAD<ReservacionHabitacione>? _TReservacionHabitacion;
        private RepositorioAD<ReservacionNoch>? _TReservacionNoche;
        private RepositorioAD<Reservacione>? _TReservacion;
        private RepositorioAD<ReservacionesMesa>? _TReservacionMesa;
        private RepositorioAD<ReversosPago>? _TReversoPago;
        private RepositorioAD<Role>? _TRol;
        private RepositorioAD<SolicitudesServicio>? _TSolicitudServicio;
        private RepositorioAD<TipoDocumento>? _TTipoDocumento;
        private RepositorioAD<TipoPenalizacion>? _TTipoPenalizacion;
        private RepositorioAD<TiposHabitacion>? _TTipoHabitacion;
        private RepositorioAD<TiposSolicitud>? _TTipoSolicitud;
        private RepositorioAD<Usuario>? _TUsuario;
        private RepositorioAD<CompensacionAmenidadPendiente>? _VCompensacionAmenidadPendiente;

        #endregion

        #region Repositorios

        public IRepositorioAD<AccesosEstadium> TAccesoEstadia
        {
            get
            {
                if (_TAccesoEstadia == null)
                    _TAccesoEstadia = new RepositorioAD<AccesosEstadium>(_Contexto);

                return _TAccesoEstadia;
            }
        }

        public IRepositorioAD<Amenidade> TAmenidad
        {
            get
            {
                if (_TAmenidad == null)
                    _TAmenidad = new RepositorioAD<Amenidade>(_Contexto);

                return _TAmenidad;
            }
        }

        public IRepositorioAD<Auditorium> TAuditoria
        {
            get
            {
                if (_TAuditoria == null)
                    _TAuditoria = new RepositorioAD<Auditorium>(_Contexto);

                return _TAuditoria;
            }
        }

        public IRepositorioAD<BloqueosHabitacion> TBloqueoHabitacion
        {
            get
            {
                if (_TBloqueoHabitacion == null)
                    _TBloqueoHabitacion = new RepositorioAD<BloqueosHabitacion>(_Contexto);

                return _TBloqueoHabitacion;
            }
        }

        public IRepositorioAD<CargosCuentum> TCargoCuenta
        {
            get
            {
                if (_TCargoCuenta == null)
                    _TCargoCuenta = new RepositorioAD<CargosCuentum>(_Contexto);

                return _TCargoCuenta;
            }
        }

        public IRepositorioAD<CategoriasPlatillo> TCategoriaPlatillo
        {
            get
            {
                if (_TCategoriaPlatillo == null)
                    _TCategoriaPlatillo = new RepositorioAD<CategoriasPlatillo>(_Contexto);

                return _TCategoriaPlatillo;
            }
        }

        public IRepositorioAD<Cliente> TCliente
        {
            get
            {
                if (_TCliente == null)
                    _TCliente = new RepositorioAD<Cliente>(_Contexto);

                return _TCliente;
            }
        }

        public IRepositorioAD<Configuracion> TConfiguracion
        {
            get
            {
                if (_TConfiguracion == null)
                    _TConfiguracion = new RepositorioAD<Configuracion>(_Contexto);

                return _TConfiguracion;
            }
        }

        public IRepositorioAD<Cuenta> TCuenta
        {
            get
            {
                if (_TCuenta == null)
                    _TCuenta = new RepositorioAD<Cuenta>(_Contexto);

                return _TCuenta;
            }
        }

        public IRepositorioAD<DetalleFactura> TDetalleFactura
        {
            get
            {
                if (_TDetalleFactura == null)
                    _TDetalleFactura = new RepositorioAD<DetalleFactura>(_Contexto);

                return _TDetalleFactura;
            }
        }

        public IRepositorioAD<DetallePedido> TDetallePedido
        {
            get
            {
                if (_TDetallePedido == null)
                    _TDetallePedido = new RepositorioAD<DetallePedido>(_Contexto);

                return _TDetallePedido;
            }
        }

        public IRepositorioAD<Descuento> TDescuento
        {
            get
            {
                if (_TDescuento == null)
                    _TDescuento = new RepositorioAD<Descuento>(_Contexto);

                return _TDescuento;
            }
        }

        public IRepositorioAD<DescuentoAplicado> TDescuentoAplicado
        {
            get
            {
                if (_TDescuentoAplicado == null)
                    _TDescuentoAplicado = new RepositorioAD<DescuentoAplicado>(_Contexto);

                return _TDescuentoAplicado;
            }
        }

        public IRepositorioAD<Edificio> TEdificio
        {
            get
            {
                if (_TEdificio == null)
                    _TEdificio = new RepositorioAD<Edificio>(_Contexto);

                return _TEdificio;
            }
        }

        public IRepositorioAD<Estadia> TEstadia
        {
            get
            {
                if (_TEstadia == null)
                    _TEstadia = new RepositorioAD<Estadia>(_Contexto);

                return _TEstadia;
            }
        }

        public IRepositorioAD<Factura> TFactura
        {
            get
            {
                if (_TFactura == null)
                    _TFactura = new RepositorioAD<Factura>(_Contexto);

                return _TFactura;
            }
        }

        public IRepositorioAD<FotosHabitacion> TFotoHabitacion
        {
            get
            {
                if (_TFotoHabitacion == null)
                    _TFotoHabitacion = new RepositorioAD<FotosHabitacion>(_Contexto);

                return _TFotoHabitacion;
            }
        }

        public IRepositorioAD<HabitacionAmenidade> THabitacionAmenidad
        {
            get
            {
                if (_THabitacionAmenidad == null)
                    _THabitacionAmenidad = new RepositorioAD<HabitacionAmenidade>(_Contexto);

                return _THabitacionAmenidad;
            }
        }

        public IRepositorioAD<Habitacione> THabitacion
        {
            get
            {
                if (_THabitacion == null)
                    _THabitacion = new RepositorioAD<Habitacione>(_Contexto);

                return _THabitacion;
            }
        }

        public IRepositorioAD<HistorialEstadosHabitacion> THistorialEstadoHabitacion
        {
            get
            {
                if (_THistorialEstadoHabitacion == null)
                    _THistorialEstadoHabitacion = new RepositorioAD<HistorialEstadosHabitacion>(_Contexto);

                return _THistorialEstadoHabitacion;
            }
        }

        public IRepositorioAD<HistorialPedido> THistorialPedido
        {
            get
            {
                if (_THistorialPedido == null)
                    _THistorialPedido = new RepositorioAD<HistorialPedido>(_Contexto);

                return _THistorialPedido;
            }
        }

        public IRepositorioAD<HistorialReservacione> THistorialReservacion
        {
            get
            {
                if (_THistorialReservacion == null)
                    _THistorialReservacion = new RepositorioAD<HistorialReservacione>(_Contexto);

                return _THistorialReservacion;
            }
        }

        public IRepositorioAD<HistorialSolicitude> THistorialSolicitud
        {
            get
            {
                if (_THistorialSolicitud == null)
                    _THistorialSolicitud = new RepositorioAD<HistorialSolicitude>(_Contexto);

                return _THistorialSolicitud;
            }
        }

        public IRepositorioAD<HuespedesReservacion> THuespedReservacion
        {
            get
            {
                if (_THuespedReservacion == null)
                    _THuespedReservacion = new RepositorioAD<HuespedesReservacion>(_Contexto);

                return _THuespedReservacion;
            }
        }

        public IRepositorioAD<Impuesto> TImpuesto
        {
            get
            {
                if (_TImpuesto == null)
                    _TImpuesto = new RepositorioAD<Impuesto>(_Contexto);

                return _TImpuesto;
            }
        }

        public IRepositorioAD<IndisponibilidadAmenidad> TIndisponibilidadAmenidad
        {
            get
            {
                if (_TIndisponibilidadAmenidad == null)
                    _TIndisponibilidadAmenidad = new RepositorioAD<IndisponibilidadAmenidad>(_Contexto);

                return _TIndisponibilidadAmenidad;
            }
        }

        public IRepositorioAD<Mesa> TMesa
        {
            get
            {
                if (_TMesa == null)
                    _TMesa = new RepositorioAD<Mesa>(_Contexto);

                return _TMesa;
            }
        }

        public IRepositorioAD<NotificacionesCorreo> TNotificacionCorreo
        {
            get
            {
                if (_TNotificacionCorreo == null)
                    _TNotificacionCorreo = new RepositorioAD<NotificacionesCorreo>(_Contexto);

                return _TNotificacionCorreo;
            }
        }

        public IRepositorioAD<Pago> TPago
        {
            get
            {
                if (_TPago == null)
                    _TPago = new RepositorioAD<Pago>(_Contexto);

                return _TPago;
            }
        }

        public IRepositorioAD<PedidosRestaurante> TPedidoRestaurante
        {
            get
            {
                if (_TPedidoRestaurante == null)
                    _TPedidoRestaurante = new RepositorioAD<PedidosRestaurante>(_Contexto);

                return _TPedidoRestaurante;
            }
        }

        public IRepositorioAD<Permiso> TPermiso
        {
            get
            {
                if (_TPermiso == null)
                    _TPermiso = new RepositorioAD<Permiso>(_Contexto);

                return _TPermiso;
            }
        }

        public IRepositorioAD<Persona> TPersona
        {
            get
            {
                if (_TPersona == null)
                    _TPersona = new RepositorioAD<Persona>(_Contexto);

                return _TPersona;
            }
        }

        public IRepositorioAD<Platillo> TPlatillo
        {
            get
            {
                if (_TPlatillo == null)
                    _TPlatillo = new RepositorioAD<Platillo>(_Contexto);

                return _TPlatillo;
            }
        }

        public IRepositorioAD<RefreshToken> TRefreshToken
        {
            get
            {
                if (_TRefreshToken == null)
                    _TRefreshToken = new RepositorioAD<RefreshToken>(_Contexto);

                return _TRefreshToken;
            }
        }

        public IRepositorioAD<ReglasTarifa> TReglaTarifa
        {
            get
            {
                if (_TReglaTarifa == null)
                    _TReglaTarifa = new RepositorioAD<ReglasTarifa>(_Contexto);

                return _TReglaTarifa;
            }
        }

        public IRepositorioAD<ReservacionAmenidad> TReservacionAmenidad
        {
            get
            {
                if (_TReservacionAmenidad == null)
                    _TReservacionAmenidad = new RepositorioAD<ReservacionAmenidad>(_Contexto);

                return _TReservacionAmenidad;
            }
        }

        public IRepositorioAD<ReservacionHabitacione> TReservacionHabitacion
        {
            get
            {
                if (_TReservacionHabitacion == null)
                    _TReservacionHabitacion = new RepositorioAD<ReservacionHabitacione>(_Contexto);

                return _TReservacionHabitacion;
            }
        }

        public IRepositorioAD<ReservacionNoch> TReservacionNoche
        {
            get
            {
                if (_TReservacionNoche == null)
                    _TReservacionNoche = new RepositorioAD<ReservacionNoch>(_Contexto);

                return _TReservacionNoche;
            }
        }

        public IRepositorioAD<Reservacione> TReservacion
        {
            get
            {
                if (_TReservacion == null)
                    _TReservacion = new RepositorioAD<Reservacione>(_Contexto);

                return _TReservacion;
            }
        }

        public IRepositorioAD<ReservacionesMesa> TReservacionMesa
        {
            get
            {
                if (_TReservacionMesa == null)
                    _TReservacionMesa = new RepositorioAD<ReservacionesMesa>(_Contexto);

                return _TReservacionMesa;
            }
        }

        public IRepositorioAD<ReversosPago> TReversoPago
        {
            get
            {
                if (_TReversoPago == null)
                    _TReversoPago = new RepositorioAD<ReversosPago>(_Contexto);

                return _TReversoPago;
            }
        }

        public IRepositorioAD<Role> TRol
        {
            get
            {
                if (_TRol == null)
                    _TRol = new RepositorioAD<Role>(_Contexto);

                return _TRol;
            }
        }

        public IRepositorioAD<SolicitudesServicio> TSolicitudServicio
        {
            get
            {
                if (_TSolicitudServicio == null)
                    _TSolicitudServicio = new RepositorioAD<SolicitudesServicio>(_Contexto);

                return _TSolicitudServicio;
            }
        }

        public IRepositorioAD<TipoDocumento> TTipoDocumento
        {
            get
            {
                if (_TTipoDocumento == null)
                    _TTipoDocumento = new RepositorioAD<TipoDocumento>(_Contexto);

                return _TTipoDocumento;
            }
        }

        public IRepositorioAD<TipoPenalizacion> TTipoPenalizacion
        {
            get
            {
                if (_TTipoPenalizacion == null)
                    _TTipoPenalizacion = new RepositorioAD<TipoPenalizacion>(_Contexto);

                return _TTipoPenalizacion;
            }
        }

        public IRepositorioAD<TiposHabitacion> TTipoHabitacion
        {
            get
            {
                if (_TTipoHabitacion == null)
                    _TTipoHabitacion = new RepositorioAD<TiposHabitacion>(_Contexto);

                return _TTipoHabitacion;
            }
        }

        public IRepositorioAD<TiposSolicitud> TTipoSolicitud
        {
            get
            {
                if (_TTipoSolicitud == null)
                    _TTipoSolicitud = new RepositorioAD<TiposSolicitud>(_Contexto);

                return _TTipoSolicitud;
            }
        }

        public IRepositorioAD<Usuario> TUsuario
        {
            get
            {
                if (_TUsuario == null)
                    _TUsuario = new RepositorioAD<Usuario>(_Contexto);

                return _TUsuario;
            }
        }

        public IRepositorioAD<CompensacionAmenidadPendiente> VCompensacionAmenidadPendiente
        {
            get
            {
                if (_VCompensacionAmenidadPendiente == null)
                    _VCompensacionAmenidadPendiente = new RepositorioAD<CompensacionAmenidadPendiente>(_Contexto);

                return _VCompensacionAmenidadPendiente;
            }
        }

        #endregion

        #region Métodos

        public Task<int> SiguienteCodigoReservaAsync()
            => SiguienteValorAsync("SELECT NEXT VALUE FOR dbo.SeqCodigoReserva AS [Value]");

        public Task<int> SiguienteNumeroFacturaAsync()
            => SiguienteValorAsync("SELECT NEXT VALUE FOR dbo.SeqNumeroFactura AS [Value]");

        // Se usa ToListAsync y no FirstAsync: EF envolvería la consulta en una
        // subconsulta y SQL Server no permite NEXT VALUE FOR dentro de subconsultas
        private async Task<int> SiguienteValorAsync(string sql)
        {
            var valores = await _Contexto.Database.SqlQueryRaw<int>(sql).ToListAsync();
            return valores[0];
        }

        public int Completar()
        {
            return _Contexto.SaveChanges();
        }

        public void EmpezarTransaccion()
        {
            if (_transaction != null)
                throw new InvalidOperationException(
                    "Ya hay una transacción activa.");

            _transaction = _Contexto.Database.BeginTransaction();
        }

        public void CompletarTran()
        {
            if (_transaction == null)
                throw new InvalidOperationException(
                    "No hay una transacción activa.");

            try
            {
                _Contexto.SaveChanges();
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                _Contexto.ChangeTracker.Clear();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void Rollback()
        {
            if (_transaction == null)
                return;

            try
            {
                _transaction.Rollback();
            }
            finally
            {
                // Lo deshecho en la BD no debe quedar pendiente en el contexto
                _Contexto.ChangeTracker.Clear();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void CerrarConexion()
        {
            _Contexto.Database.CloseConnection();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _Contexto.Dispose();
        }

        #endregion
    }
}
