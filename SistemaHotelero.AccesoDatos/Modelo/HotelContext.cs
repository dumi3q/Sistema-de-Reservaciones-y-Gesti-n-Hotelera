using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SistemaHotelero.AccesoDatos.Modelo;

public partial class HotelContext : DbContext
{
    public HotelContext()
    {
    }

    public HotelContext(DbContextOptions<HotelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccesosEstadium> AccesosEstadia { get; set; }

    public virtual DbSet<Amenidade> Amenidades { get; set; }

    public virtual DbSet<Auditorium> Auditoria { get; set; }

    public virtual DbSet<BloqueosHabitacion> BloqueosHabitacions { get; set; }

    public virtual DbSet<CargosCuentum> CargosCuenta { get; set; }

    public virtual DbSet<CategoriasPlatillo> CategoriasPlatillos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Configuracion> Configuracions { get; set; }

    public virtual DbSet<Cuenta> Cuentas { get; set; }

    public virtual DbSet<DetalleFactura> DetalleFacturas { get; set; }

    public virtual DbSet<DetallePedido> DetallePedidos { get; set; }

    public virtual DbSet<Edificio> Edificios { get; set; }

    public virtual DbSet<Estadia> Estadias { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<FotosHabitacion> FotosHabitacions { get; set; }

    public virtual DbSet<HabitacionAmenidade> HabitacionAmenidades { get; set; }

    public virtual DbSet<Habitacione> Habitaciones { get; set; }

    public virtual DbSet<HistorialEstadosHabitacion> HistorialEstadosHabitacions { get; set; }

    public virtual DbSet<HistorialPedido> HistorialPedidos { get; set; }

    public virtual DbSet<HistorialReservacione> HistorialReservaciones { get; set; }

    public virtual DbSet<HistorialSolicitude> HistorialSolicitudes { get; set; }

    public virtual DbSet<HuespedesReservacion> HuespedesReservacions { get; set; }

    public virtual DbSet<Impuesto> Impuestos { get; set; }

    public virtual DbSet<Mesa> Mesas { get; set; }

    public virtual DbSet<NotificacionesCorreo> NotificacionesCorreos { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<PedidosRestaurante> PedidosRestaurantes { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Platillo> Platillos { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<ReglasTarifa> ReglasTarifas { get; set; }

    public virtual DbSet<ReservacionHabitacione> ReservacionHabitaciones { get; set; }

    public virtual DbSet<ReservacionNoch> ReservacionNoches { get; set; }

    public virtual DbSet<Reservacione> Reservaciones { get; set; }

    public virtual DbSet<ReservacionesMesa> ReservacionesMesas { get; set; }

    public virtual DbSet<ReversosPago> ReversosPagos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SolicitudesServicio> SolicitudesServicios { get; set; }

    public virtual DbSet<TiposHabitacion> TiposHabitacions { get; set; }

    public virtual DbSet<TiposSolicitud> TiposSolicituds { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS01;Initial Catalog=HotelDB;Integrated Security=True;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccesosEstadium>(entity =>
        {
            entity.HasIndex(e => e.EstadiaId, "IX_Accesos_Estadia");

            entity.Property(e => e.FechaEmisionUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Accesos_Emision");
            entity.Property(e => e.FechaExpiracionUtc).HasPrecision(0);
            entity.Property(e => e.FechaRevocacionUtc).HasPrecision(0);
            entity.Property(e => e.IdentificadorAcceso).HasMaxLength(128);

            entity.HasOne(d => d.Estadia).WithMany(p => p.AccesosEstadia)
                .HasForeignKey(d => d.EstadiaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Accesos_Estadias");
        });

        modelBuilder.Entity<Amenidade>(entity =>
        {
            entity.HasIndex(e => e.Nombre, "UQ_Amenidades_Nombre").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Amenidades_Activo");
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(60);
        });

        modelBuilder.Entity<Auditorium>(entity =>
        {
            entity.HasIndex(e => new { e.Entidad, e.EntidadId, e.FechaUtc }, "IX_Auditoria_Entidad");

            entity.HasIndex(e => new { e.UsuarioId, e.FechaUtc }, "IX_Auditoria_Usuario");

            entity.Property(e => e.Accion).HasMaxLength(100);
            entity.Property(e => e.Entidad).HasMaxLength(100);
            entity.Property(e => e.EntidadId).HasMaxLength(50);
            entity.Property(e => e.FechaUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Auditoria_Fecha");
            entity.Property(e => e.Ip).HasMaxLength(45);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Auditoria)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_Auditoria_Usuarios");
        });

        modelBuilder.Entity<BloqueosHabitacion>(entity =>
        {
            entity.ToTable("BloqueosHabitacion");

            entity.HasIndex(e => new { e.HabitacionId, e.FechaInicio, e.FechaFin }, "IX_Bloqueos_Habitacion_Fechas").HasFilter("([Activo]=(1))");

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Bloqueos_Activo");
            entity.Property(e => e.Motivo).HasMaxLength(300);

            entity.HasOne(d => d.Habitacion).WithMany(p => p.BloqueosHabitacions)
                .HasForeignKey(d => d.HabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bloqueos_Habitaciones");

            entity.HasOne(d => d.Usuario).WithMany(p => p.BloqueosHabitacions)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bloqueos_Usuarios");
        });

        modelBuilder.Entity<CargosCuentum>(entity =>
        {
            entity.HasIndex(e => e.CuentaId, "IX_Cargos_Cuenta");

            entity.HasIndex(e => e.PedidoId, "IX_Cargos_Pedido").HasFilter("([PedidoId] IS NOT NULL)");

            entity.HasIndex(e => e.CargoRevertidoId, "UX_Cargos_Revertido")
                .IsUnique()
                .HasFilter("([CargoRevertidoId] IS NOT NULL)");

            entity.Property(e => e.Cantidad).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Concepto).HasMaxLength(200);
            entity.Property(e => e.Descuento).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FechaUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Cargos_Fecha");
            entity.Property(e => e.Impuestos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MotivoAjuste).HasMaxLength(300);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.AutorizadoPor).WithMany(p => p.CargosCuentumAutorizadoPors)
                .HasForeignKey(d => d.AutorizadoPorId)
                .HasConstraintName("FK_Cargos_Autorizador");

            entity.HasOne(d => d.CargoRevertido).WithOne(p => p.InverseCargoRevertido)
                .HasForeignKey<CargosCuentum>(d => d.CargoRevertidoId)
                .HasConstraintName("FK_Cargos_Revertido");

            entity.HasOne(d => d.Cuenta).WithMany(p => p.CargosCuenta)
                .HasForeignKey(d => d.CuentaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cargos_Cuentas");

            entity.HasOne(d => d.Estadia).WithMany(p => p.CargosCuenta)
                .HasForeignKey(d => d.EstadiaId)
                .HasConstraintName("FK_Cargos_Estadias");

            entity.HasOne(d => d.Impuesto).WithMany(p => p.CargosCuenta)
                .HasForeignKey(d => d.ImpuestoId)
                .HasConstraintName("FK_Cargos_Impuestos");

            entity.HasOne(d => d.Pedido).WithMany(p => p.CargosCuenta)
                .HasForeignKey(d => d.PedidoId)
                .HasConstraintName("FK_Cargos_Pedidos");

            entity.HasOne(d => d.Usuario).WithMany(p => p.CargosCuentumUsuarios)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cargos_Usuarios");
        });

        modelBuilder.Entity<CategoriasPlatillo>(entity =>
        {
            entity.ToTable("CategoriasPlatillo");

            entity.HasIndex(e => e.Nombre, "UQ_CategoriasPlatillo_Nombre").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Categorias_Activo");
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(60);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasIndex(e => new { e.TipoDocumento, e.NumeroDocumento }, "UQ_Clientes_Documento").IsUnique();

            entity.HasIndex(e => e.Email, "UQ_Clientes_Email").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Clientes_Activo");
            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FechaRegistroUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Clientes_FechaRegistro");
            entity.Property(e => e.Nacionalidad).HasMaxLength(60);
            entity.Property(e => e.Nombres).HasMaxLength(100);
            entity.Property(e => e.NumeroDocumento).HasMaxLength(30);
            entity.Property(e => e.PorcentajeDescuento).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Preferencias).HasMaxLength(500);
            entity.Property(e => e.Telefono).HasMaxLength(30);
        });

        modelBuilder.Entity<Configuracion>(entity =>
        {
            entity.HasKey(e => e.Clave);

            entity.ToTable("Configuracion");

            entity.Property(e => e.Clave).HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(300);
            entity.Property(e => e.Valor).HasMaxLength(500);
        });

        modelBuilder.Entity<Cuenta>(entity =>
        {
            entity.HasIndex(e => new { e.ClienteId, e.Estado }, "IX_Cuentas_Cliente");

            entity.HasIndex(e => e.ReservacionId, "UX_Cuentas_Reservacion")
                .IsUnique()
                .HasFilter("([ReservacionId] IS NOT NULL)");

            entity.Property(e => e.Estado).HasDefaultValue((byte)1, "DF_Cuentas_Estado");
            entity.Property(e => e.FechaAperturaUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Cuentas_Apertura");
            entity.Property(e => e.FechaCierreUtc).HasPrecision(0);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Cliente).WithMany(p => p.Cuenta)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuentas_Clientes");

            entity.HasOne(d => d.Reservacion).WithOne(p => p.Cuenta)
                .HasForeignKey<Cuenta>(d => d.ReservacionId)
                .HasConstraintName("FK_Cuentas_Reservaciones");
        });

        modelBuilder.Entity<DetalleFactura>(entity =>
        {
            entity.HasIndex(e => e.FacturaId, "IX_DetFac_Factura");

            entity.Property(e => e.Cantidad).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Descuento).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Impuestos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CargoCuenta).WithMany(p => p.DetalleFacturas)
                .HasForeignKey(d => d.CargoCuentaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetFac_Cargos");

            entity.HasOne(d => d.Factura).WithMany(p => p.DetalleFacturas)
                .HasForeignKey(d => d.FacturaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetFac_Facturas");
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.HasIndex(e => e.PedidoId, "IX_DetPed_Pedido");

            entity.HasIndex(e => e.PlatilloId, "IX_DetPed_Platillo");

            entity.Property(e => e.Descuento).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Impuestos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Instrucciones).HasMaxLength(300);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Pedido).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetPed_Pedidos");

            entity.HasOne(d => d.Platillo).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.PlatilloId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetPed_Platillos");
        });

        modelBuilder.Entity<Edificio>(entity =>
        {
            entity.HasIndex(e => e.Codigo, "UQ_Edificios_Codigo").IsUnique();

            entity.HasIndex(e => e.Nombre, "UQ_Edificios_Nombre").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Edificios_Activo");
            entity.Property(e => e.CantidadPisos).HasDefaultValue((byte)6, "DF_Edificios_Pisos");
            entity.Property(e => e.Codigo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Descripcion).HasMaxLength(300);
            entity.Property(e => e.Nombre).HasMaxLength(60);
        });

        modelBuilder.Entity<Estadia>(entity =>
        {
            entity.HasIndex(e => e.Estado, "IX_Estadias_Estado");

            entity.HasIndex(e => e.ReservacionHabitacionId, "UQ_Estadias_ResHab").IsUnique();

            entity.Property(e => e.CheckInRealUtc).HasPrecision(0);
            entity.Property(e => e.CheckOutRealUtc).HasPrecision(0);
            entity.Property(e => e.Estado).HasDefaultValue((byte)1, "DF_Estadias_Estado");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.ReservacionHabitacion).WithOne(p => p.Estadia)
                .HasForeignKey<Estadia>(d => d.ReservacionHabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Estadias_ResHab");

            entity.HasOne(d => d.UsuarioCheckIn).WithMany(p => p.EstadiaUsuarioCheckIns)
                .HasForeignKey(d => d.UsuarioCheckInId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Estadias_UsuarioCheckIn");

            entity.HasOne(d => d.UsuarioCheckOut).WithMany(p => p.EstadiaUsuarioCheckOuts)
                .HasForeignKey(d => d.UsuarioCheckOutId)
                .HasConstraintName("FK_Estadias_UsuarioCheckOut");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasIndex(e => e.CuentaId, "IX_Facturas_Cuenta");

            entity.HasIndex(e => e.FechaEmisionUtc, "IX_Facturas_Fecha");

            entity.HasIndex(e => e.Numero, "UQ_Facturas_Numero").IsUnique();

            entity.Property(e => e.Descuento).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DocumentoReceptor).HasMaxLength(30);
            entity.Property(e => e.EmailReceptor).HasMaxLength(150);
            entity.Property(e => e.Estado).HasDefaultValue((byte)1, "DF_Facturas_Estado");
            entity.Property(e => e.FechaEmisionUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Facturas_Fecha");
            entity.Property(e => e.Impuestos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NombreReceptor).HasMaxLength(200);
            entity.Property(e => e.Numero).HasMaxLength(30);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Cuenta).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.CuentaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Facturas_Cuentas");
        });

        modelBuilder.Entity<FotosHabitacion>(entity =>
        {
            entity.ToTable("FotosHabitacion");

            entity.HasIndex(e => e.HabitacionId, "UX_FotosHabitacion_Principal")
                .IsUnique()
                .HasFilter("([EsPrincipal]=(1))");

            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Url).HasMaxLength(500);

            entity.HasOne(d => d.Habitacion).WithOne(p => p.FotosHabitacion)
                .HasForeignKey<FotosHabitacion>(d => d.HabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FotosHabitacion_Habitaciones");
        });

        modelBuilder.Entity<HabitacionAmenidade>(entity =>
        {
            entity.HasKey(e => new { e.HabitacionId, e.AmenidadId });

            entity.HasIndex(e => e.AmenidadId, "IX_HabitacionAmenidades_Amenidad");

            entity.Property(e => e.Cantidad).HasDefaultValue((byte)1, "DF_HabitacionAmenidades_Cantidad");

            entity.HasOne(d => d.Amenidad).WithMany(p => p.HabitacionAmenidades)
                .HasForeignKey(d => d.AmenidadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HabitacionAmenidades_Amenidades");

            entity.HasOne(d => d.Habitacion).WithMany(p => p.HabitacionAmenidades)
                .HasForeignKey(d => d.HabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HabitacionAmenidades_Habitaciones");
        });

        modelBuilder.Entity<Habitacione>(entity =>
        {
            entity.HasIndex(e => new { e.EdificioId, e.Piso }, "IX_Habitaciones_Edificio_Piso");

            entity.HasIndex(e => e.TipoHabitacionId, "IX_Habitaciones_Tipo");

            entity.HasIndex(e => new { e.EdificioId, e.Numero }, "UQ_Habitaciones_Edificio_Numero").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Habitaciones_Activo");
            entity.Property(e => e.EstadoOperativo).HasDefaultValue((byte)1, "DF_Habitaciones_Estado");
            entity.Property(e => e.Numero).HasMaxLength(10);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Edificio).WithMany(p => p.Habitaciones)
                .HasForeignKey(d => d.EdificioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Habitaciones_Edificios");

            entity.HasOne(d => d.TipoHabitacion).WithMany(p => p.Habitaciones)
                .HasForeignKey(d => d.TipoHabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Habitaciones_Tipos");
        });

        modelBuilder.Entity<HistorialEstadosHabitacion>(entity =>
        {
            entity.ToTable("HistorialEstadosHabitacion");

            entity.HasIndex(e => new { e.HabitacionId, e.FechaUtc }, "IX_HistEstHab_Habitacion");

            entity.Property(e => e.FechaUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_HistEstHab_Fecha");
            entity.Property(e => e.Motivo).HasMaxLength(300);

            entity.HasOne(d => d.Habitacion).WithMany(p => p.HistorialEstadosHabitacions)
                .HasForeignKey(d => d.HabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistEstHab_Habitaciones");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistorialEstadosHabitacions)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistEstHab_Usuarios");
        });

        modelBuilder.Entity<HistorialPedido>(entity =>
        {
            entity.HasIndex(e => new { e.PedidoId, e.FechaUtc }, "IX_HistPed_Pedido");

            entity.Property(e => e.FechaUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_HistPed_Fecha");
            entity.Property(e => e.Motivo).HasMaxLength(300);

            entity.HasOne(d => d.Pedido).WithMany(p => p.HistorialPedidos)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistPed_Pedidos");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistorialPedidos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistPed_Usuarios");
        });

        modelBuilder.Entity<HistorialReservacione>(entity =>
        {
            entity.HasIndex(e => new { e.ReservacionId, e.FechaUtc }, "IX_HistRes_Reservacion");

            entity.Property(e => e.FechaUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_HistRes_Fecha");
            entity.Property(e => e.Motivo).HasMaxLength(500);

            entity.HasOne(d => d.Reservacion).WithMany(p => p.HistorialReservaciones)
                .HasForeignKey(d => d.ReservacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistRes_Reservaciones");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistorialReservaciones)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistRes_Usuarios");
        });

        modelBuilder.Entity<HistorialSolicitude>(entity =>
        {
            entity.HasIndex(e => new { e.SolicitudId, e.FechaUtc }, "IX_HistSol_Solicitud");

            entity.Property(e => e.Comentario).HasMaxLength(300);
            entity.Property(e => e.FechaUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_HistSol_Fecha");

            entity.HasOne(d => d.Solicitud).WithMany(p => p.HistorialSolicitudes)
                .HasForeignKey(d => d.SolicitudId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistSol_Solicitudes");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistorialSolicitudes)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistSol_Usuarios");
        });

        modelBuilder.Entity<HuespedesReservacion>(entity =>
        {
            entity.ToTable("HuespedesReservacion");

            entity.HasIndex(e => e.ReservacionHabitacionId, "IX_Huespedes_ResHab");

            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Nombres).HasMaxLength(100);
            entity.Property(e => e.NumeroDocumento).HasMaxLength(30);

            entity.HasOne(d => d.ReservacionHabitacion).WithMany(p => p.HuespedesReservacions)
                .HasForeignKey(d => d.ReservacionHabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Huespedes_ResHab");
        });

        modelBuilder.Entity<Impuesto>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Impuestos_Activo");
            entity.Property(e => e.Nombre).HasMaxLength(60);
            entity.Property(e => e.Porcentaje).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<Mesa>(entity =>
        {
            entity.HasIndex(e => e.Numero, "UQ_Mesas_Numero").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Mesas_Activo");
            entity.Property(e => e.Numero).HasMaxLength(10);
        });

        modelBuilder.Entity<NotificacionesCorreo>(entity =>
        {
            entity.ToTable("NotificacionesCorreo");

            entity.HasIndex(e => new { e.Estado, e.FechaCreacionUtc }, "IX_Correos_Pendientes").HasFilter("([Estado] IN ((1), (3)))");

            entity.Property(e => e.Destinatario).HasMaxLength(150);
            entity.Property(e => e.Estado).HasDefaultValue((byte)1, "DF_Correos_Estado");
            entity.Property(e => e.FechaCreacionUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Correos_Creacion");
            entity.Property(e => e.FechaEnvioUtc).HasPrecision(0);
            entity.Property(e => e.UltimoError).HasMaxLength(1000);

            entity.HasOne(d => d.Cliente).WithMany(p => p.NotificacionesCorreos)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Correos_Clientes");

            entity.HasOne(d => d.Factura).WithMany(p => p.NotificacionesCorreos)
                .HasForeignKey(d => d.FacturaId)
                .HasConstraintName("FK_Correos_Facturas");

            entity.HasOne(d => d.Reservacion).WithMany(p => p.NotificacionesCorreos)
                .HasForeignKey(d => d.ReservacionId)
                .HasConstraintName("FK_Correos_Reservaciones");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasIndex(e => e.CuentaId, "IX_Pagos_Cuenta");

            entity.HasIndex(e => e.FechaPagoUtc, "IX_Pagos_Fecha");

            entity.HasIndex(e => e.ClaveIdempotencia, "UQ_Pagos_Idempotencia").IsUnique();

            entity.Property(e => e.Estado).HasDefaultValue((byte)1, "DF_Pagos_Estado");
            entity.Property(e => e.FechaPagoUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Pagos_Fecha");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Referencia).HasMaxLength(100);
            entity.Property(e => e.UltimosCuatro)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Cuenta).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.CuentaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pagos_Cuentas");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pagos_Usuarios");
        });

        modelBuilder.Entity<PedidosRestaurante>(entity =>
        {
            entity.ToTable("PedidosRestaurante");

            entity.HasIndex(e => new { e.ClienteId, e.FechaPedidoUtc }, "IX_Pedidos_Cliente");

            entity.HasIndex(e => e.CuentaId, "IX_Pedidos_Cuenta");

            entity.HasIndex(e => new { e.EstadoPedido, e.FechaPedidoUtc }, "IX_Pedidos_Estado_Fecha");

            entity.Property(e => e.Descuento).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EstadoPedido).HasDefaultValue((byte)1, "DF_Pedidos_Estado");
            entity.Property(e => e.FechaHoraDeseadaUtc).HasPrecision(0);
            entity.Property(e => e.FechaPedidoUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Pedidos_Fecha");
            entity.Property(e => e.Impuestos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Instrucciones).HasMaxLength(500);
            entity.Property(e => e.Recargos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UbicacionEntrega).HasMaxLength(100);

            entity.HasOne(d => d.Cliente).WithMany(p => p.PedidosRestaurantes)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedidos_Clientes");

            entity.HasOne(d => d.Cuenta).WithMany(p => p.PedidosRestaurantes)
                .HasForeignKey(d => d.CuentaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedidos_Cuentas");

            entity.HasOne(d => d.Estadia).WithMany(p => p.PedidosRestaurantes)
                .HasForeignKey(d => d.EstadiaId)
                .HasConstraintName("FK_Pedidos_Estadias");

            entity.HasOne(d => d.ReservacionMesa).WithMany(p => p.PedidosRestaurantes)
                .HasForeignKey(d => d.ReservacionMesaId)
                .HasConstraintName("FK_Pedidos_ResMesa");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasIndex(e => e.Codigo, "UQ_Permisos_Codigo").IsUnique();

            entity.Property(e => e.Codigo).HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(200);
        });

        modelBuilder.Entity<Platillo>(entity =>
        {
            entity.HasIndex(e => new { e.CategoriaId, e.Nombre }, "UQ_Platillos_Categoria_Nombre").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Platillos_Activo");
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Disponible).HasDefaultValue(true, "DF_Platillos_Disponible");
            entity.Property(e => e.ImagenUrl).HasMaxLength(500);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Platillos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Platillos_Categorias");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(e => e.FamiliaId, "IX_RefreshTokens_Familia");

            entity.HasIndex(e => e.UsuarioId, "IX_RefreshTokens_Usuario");

            entity.HasIndex(e => e.TokenHash, "UQ_RefreshTokens_TokenHash").IsUnique();

            entity.Property(e => e.FechaCreacionUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_RefreshTokens_FechaCreacion");
            entity.Property(e => e.FechaExpiracionUtc).HasPrecision(0);
            entity.Property(e => e.FechaRevocacionUtc).HasPrecision(0);
            entity.Property(e => e.FechaUsoUtc).HasPrecision(0);
            entity.Property(e => e.MotivoRevocacion).HasMaxLength(200);
            entity.Property(e => e.TokenHash)
                .HasMaxLength(128)
                .IsUnicode(false);

            entity.HasOne(d => d.ReemplazadoPor).WithMany(p => p.InverseReemplazadoPor)
                .HasForeignKey(d => d.ReemplazadoPorId)
                .HasConstraintName("FK_RefreshTokens_Reemplazo");

            entity.HasOne(d => d.Usuario).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshTokens_Usuarios");
        });

        modelBuilder.Entity<ReglasTarifa>(entity =>
        {
            entity.ToTable("ReglasTarifa");

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_ReglasTarifa_Activo");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Valor).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.TipoHabitacion).WithMany(p => p.ReglasTarifas)
                .HasForeignKey(d => d.TipoHabitacionId)
                .HasConstraintName("FK_ReglasTarifa_Tipos");
        });

        modelBuilder.Entity<ReservacionHabitacione>(entity =>
        {
            entity.HasIndex(e => new { e.HabitacionId, e.FechaInicio, e.FechaFin }, "IX_ResHab_Habitacion_Fechas");

            entity.HasIndex(e => e.ReservacionId, "IX_ResHab_Reservacion");

            entity.Property(e => e.Descuento).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Impuestos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Habitacion).WithMany(p => p.ReservacionHabitaciones)
                .HasForeignKey(d => d.HabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResHab_Habitaciones");

            entity.HasOne(d => d.Reservacion).WithMany(p => p.ReservacionHabitaciones)
                .HasForeignKey(d => d.ReservacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResHab_Reservaciones");
        });

        modelBuilder.Entity<ReservacionNoch>(entity =>
        {
            entity.HasIndex(e => new { e.ReservacionHabitacionId, e.Fecha }, "UQ_ResNoches_Linea_Fecha").IsUnique();

            entity.HasIndex(e => new { e.HabitacionId, e.Fecha }, "UX_ResNoches_Habitacion_Fecha_Activa")
                .IsUnique()
                .HasFilter("([Activa]=(1))");

            entity.Property(e => e.Activa).HasDefaultValue(true, "DF_ResNoches_Activa");
            entity.Property(e => e.AjusteTarifa).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TarifaBase).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TarifaFinal).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Habitacion).WithMany(p => p.ReservacionNoches)
                .HasForeignKey(d => d.HabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResNoches_Habitaciones");

            entity.HasOne(d => d.ReservacionHabitacion).WithMany(p => p.ReservacionNoches)
                .HasForeignKey(d => d.ReservacionHabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResNoches_ResHab");
        });

        modelBuilder.Entity<Reservacione>(entity =>
        {
            entity.HasIndex(e => new { e.ClienteId, e.Estado }, "IX_Reservaciones_Cliente_Estado");

            entity.HasIndex(e => new { e.Estado, e.FechaExpiracionPendienteUtc }, "IX_Reservaciones_Estado_Expiracion");

            entity.HasIndex(e => new { e.FechaInicio, e.FechaFin }, "IX_Reservaciones_Fechas");

            entity.HasIndex(e => e.Codigo, "UQ_Reservaciones_Codigo").IsUnique();

            entity.Property(e => e.AnticipoRequerido).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Codigo).HasMaxLength(30);
            entity.Property(e => e.Descuento).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Estado).HasDefaultValue((byte)1, "DF_Reservaciones_Estado");
            entity.Property(e => e.FechaCreacionUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Reservaciones_FechaCreacion");
            entity.Property(e => e.FechaExpiracionPendienteUtc).HasPrecision(0);
            entity.Property(e => e.Impuestos).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MotivoCancelacion).HasMaxLength(500);
            entity.Property(e => e.Observaciones).HasMaxLength(1000);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Reservaciones)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservaciones_Clientes");

            entity.HasOne(d => d.CreadoPorUsuario).WithMany(p => p.Reservaciones)
                .HasForeignKey(d => d.CreadoPorUsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservaciones_Usuarios");
        });

        modelBuilder.Entity<ReservacionesMesa>(entity =>
        {
            entity.ToTable("ReservacionesMesa");

            entity.HasIndex(e => new { e.MesaId, e.FechaInicioUtc, e.FechaFinUtc }, "IX_ResMesa_Mesa_Fechas");

            entity.Property(e => e.Estado).HasDefaultValue((byte)1, "DF_ResMesa_Estado");
            entity.Property(e => e.FechaFinUtc).HasPrecision(0);
            entity.Property(e => e.FechaInicioUtc).HasPrecision(0);
            entity.Property(e => e.Observaciones).HasMaxLength(500);

            entity.HasOne(d => d.Cliente).WithMany(p => p.ReservacionesMesas)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResMesa_Clientes");

            entity.HasOne(d => d.Mesa).WithMany(p => p.ReservacionesMesas)
                .HasForeignKey(d => d.MesaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResMesa_Mesas");
        });

        modelBuilder.Entity<ReversosPago>(entity =>
        {
            entity.ToTable("ReversosPago");

            entity.HasIndex(e => e.PagoId, "IX_Reversos_Pago");

            entity.Property(e => e.FechaUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Reversos_Fecha");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Motivo).HasMaxLength(300);

            entity.HasOne(d => d.Pago).WithMany(p => p.ReversosPagos)
                .HasForeignKey(d => d.PagoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reversos_Pagos");

            entity.HasOne(d => d.Usuario).WithMany(p => p.ReversosPagos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reversos_Usuarios");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.Nombre, "UQ_Roles_Nombre").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Roles_Activo");
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(50);

            entity.HasMany(d => d.Permisos).WithMany(p => p.Rols)
                .UsingEntity<Dictionary<string, object>>(
                    "RolPermiso",
                    r => r.HasOne<Permiso>().WithMany()
                        .HasForeignKey("PermisoId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolPermisos_Permisos"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RolId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolPermisos_Roles"),
                    j =>
                    {
                        j.HasKey("RolId", "PermisoId");
                        j.ToTable("RolPermisos");
                    });
        });

        modelBuilder.Entity<SolicitudesServicio>(entity =>
        {
            entity.ToTable("SolicitudesServicio");

            entity.HasIndex(e => new { e.AsignadoAusuarioId, e.Estado }, "IX_Solicitudes_Asignado").HasFilter("([AsignadoAUsuarioId] IS NOT NULL)");

            entity.HasIndex(e => e.EstadiaId, "IX_Solicitudes_Estadia").HasFilter("([EstadiaId] IS NOT NULL)");

            entity.HasIndex(e => new { e.Estado, e.Prioridad, e.FechaSolicitudUtc }, "IX_Solicitudes_Estado_Prioridad").IsDescending(false, true, false);

            entity.HasIndex(e => new { e.HabitacionId, e.FechaSolicitudUtc }, "IX_Solicitudes_Habitacion");

            entity.Property(e => e.AsignadoAusuarioId).HasColumnName("AsignadoAUsuarioId");
            entity.Property(e => e.ComentarioCierre).HasMaxLength(500);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Estado).HasDefaultValue((byte)1, "DF_Solicitudes_Estado");
            entity.Property(e => e.FechaAsignacionUtc).HasPrecision(0);
            entity.Property(e => e.FechaCompletadaUtc).HasPrecision(0);
            entity.Property(e => e.FechaInicioUtc).HasPrecision(0);
            entity.Property(e => e.FechaPreferidaUtc).HasPrecision(0);
            entity.Property(e => e.FechaSolicitudUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Solicitudes_Fecha");
            entity.Property(e => e.Prioridad).HasDefaultValue((byte)2, "DF_Solicitudes_Prioridad");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.AsignadoAusuario).WithMany(p => p.SolicitudesServicioAsignadoAusuarios)
                .HasForeignKey(d => d.AsignadoAusuarioId)
                .HasConstraintName("FK_Solicitudes_Asignado");

            entity.HasOne(d => d.Estadia).WithMany(p => p.SolicitudesServicios)
                .HasForeignKey(d => d.EstadiaId)
                .HasConstraintName("FK_Solicitudes_Estadias");

            entity.HasOne(d => d.Habitacion).WithMany(p => p.SolicitudesServicios)
                .HasForeignKey(d => d.HabitacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Solicitudes_Habitaciones");

            entity.HasOne(d => d.SolicitadoPorUsuario).WithMany(p => p.SolicitudesServicioSolicitadoPorUsuarios)
                .HasForeignKey(d => d.SolicitadoPorUsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Solicitudes_Solicitante");

            entity.HasOne(d => d.TipoSolicitud).WithMany(p => p.SolicitudesServicios)
                .HasForeignKey(d => d.TipoSolicitudId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Solicitudes_Tipos");
        });

        modelBuilder.Entity<TiposHabitacion>(entity =>
        {
            entity.ToTable("TiposHabitacion");

            entity.HasIndex(e => e.Nombre, "UQ_TiposHabitacion_Nombre").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_TiposHabitacion_Activo");
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Nombre).HasMaxLength(60);
            entity.Property(e => e.PrecioBasePorNoche).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PrecioPersonaAdicional).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<TiposSolicitud>(entity =>
        {
            entity.ToTable("TiposSolicitud");

            entity.HasIndex(e => e.Nombre, "UQ_TiposSolicitud_Nombre").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_TiposSolicitud_Activo");
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(60);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasIndex(e => e.Email, "UQ_Usuarios_Email").IsUnique();

            entity.HasIndex(e => e.NombreUsuario, "UQ_Usuarios_NombreUsuario").IsUnique();

            entity.HasIndex(e => e.ClienteId, "UX_Usuarios_ClienteId")
                .IsUnique()
                .HasFilter("([ClienteId] IS NOT NULL)");

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Usuarios_Activo");
            entity.Property(e => e.BloqueadoHastaUtc).HasPrecision(0);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FechaCreacionUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Usuarios_FechaCreacion");
            entity.Property(e => e.NombreUsuario).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.UltimoAccesoUtc).HasPrecision(0);

            entity.HasOne(d => d.Cliente).WithOne(p => p.Usuario)
                .HasForeignKey<Usuario>(d => d.ClienteId)
                .HasConstraintName("FK_Usuarios_Clientes");

            entity.HasMany(d => d.Rols).WithMany(p => p.Usuarios)
                .UsingEntity<Dictionary<string, object>>(
                    "UsuarioRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RolId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UsuarioRoles_Roles"),
                    l => l.HasOne<Usuario>().WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UsuarioRoles_Usuarios"),
                    j =>
                    {
                        j.HasKey("UsuarioId", "RolId");
                        j.ToTable("UsuarioRoles");
                    });
        });
        modelBuilder.HasSequence<int>("SeqCodigoReserva");
        modelBuilder.HasSequence<int>("SeqNumeroFactura");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
