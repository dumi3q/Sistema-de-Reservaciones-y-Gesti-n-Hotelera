/* Versión de scriptsCorregidos.sql compatible con SQL Server 2019 (nivel 150).
   El original se generó en SQL Server 2025: usa LEDGER, OPTIMIZED_LOCKING,
   nivel de compatibilidad 170 y la ruta de archivos de esa instancia.
   Aquí se crea la base con las opciones por defecto de cada servidor. */
USE [master]
GO
IF DB_ID(N'HotelDB') IS NULL
    CREATE DATABASE [HotelDB];
GO
ALTER DATABASE [HotelDB] SET RECOVERY SIMPLE
GO
USE [HotelDB]
GO
/****** Objeto: Sequence [dbo].[SeqCodigoReserva] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE SEQUENCE [dbo].[SeqCodigoReserva] 
 AS [int]
 START WITH 1
 INCREMENT BY 1
 MINVALUE -2147483648
 MAXVALUE 2147483647
 CACHE 
GO
USE [HotelDB]
GO
/****** Objeto: Sequence [dbo].[SeqNumeroFactura] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE SEQUENCE [dbo].[SeqNumeroFactura] 
 AS [int]
 START WITH 1
 INCREMENT BY 1
 MINVALUE -2147483648
 MAXVALUE 2147483647
 CACHE 
GO
/****** Objeto: Table [dbo].[Amenidades] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Amenidades](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](60) NOT NULL,
	[Descripcion] [nvarchar](200) NULL,
	[Activo] [bit] NOT NULL,
 CONSTRAINT [PK_Amenidades] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Amenidades_Nombre] UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[HabitacionAmenidades] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HabitacionAmenidades](
	[HabitacionId] [int] NOT NULL,
	[AmenidadId] [int] NOT NULL,
	[Cantidad] [tinyint] NOT NULL,
 CONSTRAINT [PK_HabitacionAmenidades] PRIMARY KEY CLUSTERED 
(
	[HabitacionId] ASC,
	[AmenidadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: View [dbo].[AmenidadesDisponibles] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   VIEW [dbo].[AmenidadesDisponibles]
AS
SELECT ha.HabitacionId, ha.AmenidadId, ha.Cantidad
FROM dbo.HabitacionAmenidades ha
JOIN dbo.Amenidades a ON a.Id = ha.AmenidadId
WHERE a.Activo = 1;
GO
/****** Objeto: Table [dbo].[Reservaciones] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reservaciones](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [nvarchar](30) NOT NULL,
	[ClienteId] [int] NOT NULL,
	[FechaInicio] [date] NOT NULL,
	[FechaFin] [date] NOT NULL,
	[Estado] [tinyint] NOT NULL,
	[CantidadAdultos] [tinyint] NOT NULL,
	[CantidadNinos] [tinyint] NOT NULL,
	[Subtotal] [decimal](18, 2) NOT NULL,
	[Descuento] [decimal](18, 2) NOT NULL,
	[Impuestos] [decimal](18, 2) NOT NULL,
	[Total] [decimal](18, 2) NOT NULL,
	[AnticipoRequerido] [decimal](18, 2) NOT NULL,
	[FechaExpiracionPendienteUtc] [datetime2](0) NULL,
	[Observaciones] [nvarchar](1000) NULL,
	[MotivoCancelacion] [nvarchar](500) NULL,
	[CreadoPorUsuarioId] [int] NOT NULL,
	[FechaCreacionUtc] [datetime2](0) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Reservaciones] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Reservaciones_Codigo] UNIQUE NONCLUSTERED 
(
	[Codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Reservaciones_Id_Cliente] UNIQUE NONCLUSTERED 
(
	[Id] ASC,
	[ClienteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[ReservacionAmenidades] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReservacionAmenidades](
	[ReservacionHabitacionId] [int] NOT NULL,
	[HabitacionId] [int] NOT NULL,
	[AmenidadId] [int] NOT NULL,
	[Cantidad] [tinyint] NOT NULL,
	[FechaRegistroUtc] [datetime2](0) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ReservacionHabitacionId] ASC,
	[AmenidadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ReservacionHabitacionId] ASC,
	[HabitacionId] ASC,
	[AmenidadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[ReservacionHabitaciones] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReservacionHabitaciones](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReservacionId] [int] NOT NULL,
	[HabitacionId] [int] NOT NULL,
	[FechaInicio] [date] NOT NULL,
	[FechaFin] [date] NOT NULL,
	[Adultos] [tinyint] NOT NULL,
	[Ninos] [tinyint] NOT NULL,
	[Subtotal] [decimal](18, 2) NOT NULL,
	[Descuento] [decimal](18, 2) NOT NULL,
	[Impuestos] [decimal](18, 2) NOT NULL,
	[Total] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_ReservacionHabitaciones] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_ResHab_Id_Habitacion] UNIQUE NONCLUSTERED 
(
	[Id] ASC,
	[HabitacionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[DescuentosAplicados] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DescuentosAplicados](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReservacionHabitacionId] [int] NOT NULL,
	[DescuentoId] [int] NULL,
	[IndisponibilidadId] [int] NULL,
	[HabitacionId] [int] NULL,
	[AmenidadId] [int] NULL,
	[Monto] [decimal](18, 2) NOT NULL,
	[Motivo] [nvarchar](300) NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[FechaUtc] [datetime2](0) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[ReservacionNoches] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReservacionNoches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReservacionHabitacionId] [int] NOT NULL,
	[HabitacionId] [int] NOT NULL,
	[Fecha] [date] NOT NULL,
	[TarifaBase] [decimal](18, 2) NOT NULL,
	[AjusteTarifa] [decimal](18, 2) NOT NULL,
	[TarifaFinal] [decimal](18, 2) NOT NULL,
	[Activa] [bit] NOT NULL,
 CONSTRAINT [PK_ReservacionNoches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_ResNoches_Linea_Fecha] UNIQUE NONCLUSTERED 
(
	[ReservacionHabitacionId] ASC,
	[Fecha] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[IndisponibilidadesAmenidad] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IndisponibilidadesAmenidad](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HabitacionId] [int] NOT NULL,
	[AmenidadId] [int] NOT NULL,
	[FechaInicio] [date] NOT NULL,
	[FechaFin] [date] NOT NULL,
	[Motivo] [nvarchar](300) NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[Activo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Id] ASC,
	[HabitacionId] ASC,
	[AmenidadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: View [dbo].[CompensacionesAmenidadPendientes] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Identifica las noches elegibles. El descuento se autoriza y contabiliza por separado.
CREATE VIEW [dbo].[CompensacionesAmenidadPendientes]
AS
SELECT ra.ReservacionHabitacionId, x.Id AS IndisponibilidadId,
       ra.HabitacionId, ra.AmenidadId, COUNT(*) AS NochesAfectadas,
       SUM(n.TarifaFinal) AS BaseAfectada
FROM dbo.ReservacionAmenidades ra
JOIN dbo.IndisponibilidadesAmenidad x
    ON x.HabitacionId = ra.HabitacionId AND x.AmenidadId = ra.AmenidadId AND x.Activo = 1
JOIN dbo.ReservacionNoches n ON n.ReservacionHabitacionId = ra.ReservacionHabitacionId
    AND n.Fecha >= x.FechaInicio AND n.Fecha < x.FechaFin AND n.Activa = 1
JOIN dbo.ReservacionHabitaciones rh ON rh.Id = ra.ReservacionHabitacionId
JOIN dbo.Reservaciones r ON r.Id = rh.ReservacionId AND r.Estado IN (2, 3, 4)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.DescuentosAplicados da
    WHERE da.ReservacionHabitacionId = ra.ReservacionHabitacionId AND da.IndisponibilidadId = x.Id
)
GROUP BY ra.ReservacionHabitacionId, x.Id, ra.HabitacionId, ra.AmenidadId;
GO
/****** Objeto: Table [dbo].[AccesosEstadia] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccesosEstadia](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EstadiaId] [int] NOT NULL,
	[IdentificadorAcceso] [nvarchar](128) NOT NULL,
	[Tipo] [tinyint] NOT NULL,
	[FechaEmisionUtc] [datetime2](0) NOT NULL,
	[FechaExpiracionUtc] [datetime2](0) NOT NULL,
	[FechaRevocacionUtc] [datetime2](0) NULL,
 CONSTRAINT [PK_AccesosEstadia] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Auditoria] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Auditoria](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UsuarioId] [int] NULL,
	[Accion] [nvarchar](100) NOT NULL,
	[Entidad] [nvarchar](100) NOT NULL,
	[EntidadId] [nvarchar](50) NULL,
	[FechaUtc] [datetime2](0) NOT NULL,
	[Resultado] [tinyint] NOT NULL,
	[Ip] [nvarchar](45) NULL,
	[DatosNoSensibles] [nvarchar](max) NULL,
 CONSTRAINT [PK_Auditoria] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[BloqueosHabitacion] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BloqueosHabitacion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HabitacionId] [int] NOT NULL,
	[FechaInicio] [date] NOT NULL,
	[FechaFin] [date] NOT NULL,
	[Tipo] [tinyint] NOT NULL,
	[Motivo] [nvarchar](300) NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[Activo] [bit] NOT NULL,
 CONSTRAINT [PK_BloqueosHabitacion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[CargosCuenta] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CargosCuenta](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CuentaId] [int] NOT NULL,
	[Tipo] [tinyint] NOT NULL,
	[EstadiaId] [int] NULL,
	[PedidoId] [int] NULL,
	[Concepto] [nvarchar](200) NOT NULL,
	[Cantidad] [decimal](10, 2) NOT NULL,
	[PrecioUnitario] [decimal](18, 2) NOT NULL,
	[Descuento] [decimal](18, 2) NOT NULL,
	[ImpuestoId] [int] NULL,
	[Impuestos] [decimal](18, 2) NOT NULL,
	[Total] [decimal](18, 2) NOT NULL,
	[FechaUtc] [datetime2](0) NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[CargoRevertidoId] [int] NULL,
	[MotivoAjuste] [nvarchar](300) NULL,
	[AutorizadoPorId] [int] NULL,
	[TipoPenalizacionId] [int] NULL,
 CONSTRAINT [PK_CargosCuenta] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[CategoriasPlatillo] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CategoriasPlatillo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](60) NOT NULL,
	[Descripcion] [nvarchar](200) NULL,
	[Orden] [int] NOT NULL,
	[Activo] [bit] NOT NULL,
 CONSTRAINT [PK_CategoriasPlatillo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_CategoriasPlatillo_Nombre] UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Clientes] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clientes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Preferencias] [nvarchar](500) NULL,
	[PorcentajeDescuento] [decimal](5, 2) NOT NULL,
	[FechaRegistroUtc] [datetime2](0) NOT NULL,
	[Activo] [bit] NOT NULL,
	[PersonaId] [int] NOT NULL,
 CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Clientes_Persona] UNIQUE NONCLUSTERED 
(
	[PersonaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Configuracion] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configuracion](
	[Clave] [nvarchar](100) NOT NULL,
	[Valor] [nvarchar](500) NOT NULL,
	[Descripcion] [nvarchar](300) NULL,
 CONSTRAINT [PK_Configuracion] PRIMARY KEY CLUSTERED 
(
	[Clave] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Cuentas] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cuentas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClienteId] [int] NOT NULL,
	[ReservacionId] [int] NULL,
	[Tipo] [tinyint] NOT NULL,
	[Estado] [tinyint] NOT NULL,
	[FechaAperturaUtc] [datetime2](0) NOT NULL,
	[FechaCierreUtc] [datetime2](0) NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Cuentas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Descuentos] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Descuentos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[TipoCalculo] [tinyint] NOT NULL,
	[Valor] [decimal](18, 2) NOT NULL,
	[VigenteDesde] [date] NOT NULL,
	[VigenteHasta] [date] NULL,
	[Activo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[DetalleFacturas] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetalleFacturas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FacturaId] [int] NOT NULL,
	[CargoCuentaId] [int] NOT NULL,
	[Descripcion] [nvarchar](200) NOT NULL,
	[Cantidad] [decimal](10, 2) NOT NULL,
	[PrecioUnitario] [decimal](18, 2) NOT NULL,
	[Descuento] [decimal](18, 2) NOT NULL,
	[Impuestos] [decimal](18, 2) NOT NULL,
	[Total] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_DetalleFacturas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[DetallePedidos] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetallePedidos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PedidoId] [int] NOT NULL,
	[PlatilloId] [int] NOT NULL,
	[Cantidad] [smallint] NOT NULL,
	[PrecioUnitario] [decimal](18, 2) NOT NULL,
	[Subtotal] [decimal](18, 2) NOT NULL,
	[Descuento] [decimal](18, 2) NOT NULL,
	[Impuestos] [decimal](18, 2) NOT NULL,
	[Total] [decimal](18, 2) NOT NULL,
	[Instrucciones] [nvarchar](300) NULL,
 CONSTRAINT [PK_DetallePedidos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Edificios] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Edificios](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [char](1) NOT NULL,
	[Nombre] [nvarchar](60) NOT NULL,
	[CantidadPisos] [tinyint] NOT NULL,
	[Descripcion] [nvarchar](300) NULL,
	[Activo] [bit] NOT NULL,
 CONSTRAINT [PK_Edificios] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Edificios_Codigo] UNIQUE NONCLUSTERED 
(
	[Codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Edificios_Nombre] UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Estadias] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Estadias](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReservacionHabitacionId] [int] NOT NULL,
	[CheckInRealUtc] [datetime2](0) NOT NULL,
	[CheckOutRealUtc] [datetime2](0) NULL,
	[UsuarioCheckInId] [int] NOT NULL,
	[UsuarioCheckOutId] [int] NULL,
	[Estado] [tinyint] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Estadias] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Estadias_ResHab] UNIQUE NONCLUSTERED 
(
	[ReservacionHabitacionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Facturas] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Facturas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CuentaId] [int] NOT NULL,
	[Numero] [nvarchar](30) NOT NULL,
	[FechaEmisionUtc] [datetime2](0) NOT NULL,
	[NombreReceptor] [nvarchar](200) NOT NULL,
	[DocumentoReceptor] [nvarchar](30) NOT NULL,
	[EmailReceptor] [nvarchar](150) NOT NULL,
	[Subtotal] [decimal](18, 2) NOT NULL,
	[Descuento] [decimal](18, 2) NOT NULL,
	[Impuestos] [decimal](18, 2) NOT NULL,
	[Total] [decimal](18, 2) NOT NULL,
	[Estado] [tinyint] NOT NULL,
	[TipoDocumentoReceptorId] [tinyint] NULL,
 CONSTRAINT [PK_Facturas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Facturas_Numero] UNIQUE NONCLUSTERED 
(
	[Numero] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[FotosHabitacion] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FotosHabitacion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HabitacionId] [int] NOT NULL,
	[Url] [nvarchar](500) NOT NULL,
	[Descripcion] [nvarchar](200) NULL,
	[Orden] [int] NOT NULL,
	[EsPrincipal] [bit] NOT NULL,
 CONSTRAINT [PK_FotosHabitacion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Habitaciones] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Habitaciones](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EdificioId] [int] NOT NULL,
	[TipoHabitacionId] [int] NOT NULL,
	[Numero] [nvarchar](10) NOT NULL,
	[Piso] [tinyint] NOT NULL,
	[Estado] [tinyint] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Habitaciones] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Habitaciones_Edificio_Numero] UNIQUE NONCLUSTERED 
(
	[EdificioId] ASC,
	[Numero] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[HistorialEstadosHabitacion] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HistorialEstadosHabitacion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HabitacionId] [int] NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[EstadoAnterior] [tinyint] NOT NULL,
	[EstadoNuevo] [tinyint] NOT NULL,
	[FechaUtc] [datetime2](0) NOT NULL,
	[Motivo] [nvarchar](300) NULL,
 CONSTRAINT [PK_HistorialEstadosHabitacion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[HistorialPedidos] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HistorialPedidos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PedidoId] [int] NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[EstadoAnterior] [tinyint] NULL,
	[EstadoNuevo] [tinyint] NOT NULL,
	[FechaUtc] [datetime2](0) NOT NULL,
	[Motivo] [nvarchar](300) NULL,
 CONSTRAINT [PK_HistorialPedidos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[HistorialReservaciones] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HistorialReservaciones](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReservacionId] [int] NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[FechaUtc] [datetime2](0) NOT NULL,
	[EstadoAnterior] [tinyint] NULL,
	[EstadoNuevo] [tinyint] NOT NULL,
	[Motivo] [nvarchar](500) NULL,
	[DetalleCambio] [nvarchar](max) NULL,
 CONSTRAINT [PK_HistorialReservaciones] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[HistorialSolicitudes] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HistorialSolicitudes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SolicitudId] [int] NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[EstadoAnterior] [tinyint] NULL,
	[EstadoNuevo] [tinyint] NOT NULL,
	[FechaUtc] [datetime2](0) NOT NULL,
	[Comentario] [nvarchar](300) NULL,
 CONSTRAINT [PK_HistorialSolicitudes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[HuespedesReservacion] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HuespedesReservacion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReservacionHabitacionId] [int] NOT NULL,
	[TipoDocumentoId] [tinyint] NULL,
	[NumeroDocumento] [nvarchar](30) NULL,
	[Nombres] [nvarchar](100) NOT NULL,
	[Apellidos] [nvarchar](100) NOT NULL,
	[FechaNacimiento] [date] NULL,
	[EsTitular] [bit] NOT NULL,
 CONSTRAINT [PK_HuespedesReservacion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Impuestos] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Impuestos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](60) NOT NULL,
	[Ambito] [tinyint] NOT NULL,
	[Porcentaje] [decimal](5, 2) NOT NULL,
	[VigenteDesde] [date] NOT NULL,
	[VigenteHasta] [date] NULL,
	[Activo] [bit] NOT NULL,
 CONSTRAINT [PK_Impuestos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Mesas] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Mesas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Numero] [nvarchar](10) NOT NULL,
	[Capacidad] [tinyint] NOT NULL,
	[Ubicacion] [tinyint] NOT NULL,
	[Activo] [bit] NOT NULL,
 CONSTRAINT [PK_Mesas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Mesas_Numero] UNIQUE NONCLUSTERED 
(
	[Numero] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[NotificacionesCorreo] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotificacionesCorreo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClienteId] [int] NOT NULL,
	[Tipo] [tinyint] NOT NULL,
	[ReservacionId] [int] NULL,
	[FacturaId] [int] NULL,
	[Destinatario] [nvarchar](150) NOT NULL,
	[Estado] [tinyint] NOT NULL,
	[Intentos] [int] NOT NULL,
	[UltimoError] [nvarchar](1000) NULL,
	[FechaCreacionUtc] [datetime2](0) NOT NULL,
	[FechaEnvioUtc] [datetime2](0) NULL,
 CONSTRAINT [PK_NotificacionesCorreo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Pagos] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pagos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CuentaId] [int] NOT NULL,
	[Monto] [decimal](18, 2) NOT NULL,
	[MetodoPago] [tinyint] NOT NULL,
	[Referencia] [nvarchar](100) NULL,
	[UltimosCuatro] [char](4) NULL,
	[FechaPagoUtc] [datetime2](0) NOT NULL,
	[Estado] [tinyint] NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[ClaveIdempotencia] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Pagos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Pagos_Idempotencia] UNIQUE NONCLUSTERED 
(
	[ClaveIdempotencia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[PedidosRestaurante] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PedidosRestaurante](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClienteId] [int] NOT NULL,
	[CuentaId] [int] NOT NULL,
	[EstadiaId] [int] NULL,
	[ReservacionMesaId] [int] NULL,
	[Modalidad] [tinyint] NOT NULL,
	[UbicacionEntrega] [nvarchar](100) NULL,
	[FechaPedidoUtc] [datetime2](0) NOT NULL,
	[FechaHoraDeseadaUtc] [datetime2](0) NULL,
	[EstadoPedido] [tinyint] NOT NULL,
	[Subtotal] [decimal](18, 2) NOT NULL,
	[Descuento] [decimal](18, 2) NOT NULL,
	[Impuestos] [decimal](18, 2) NOT NULL,
	[Recargos] [decimal](18, 2) NOT NULL,
	[Total] [decimal](18, 2) NOT NULL,
	[Instrucciones] [nvarchar](500) NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_PedidosRestaurante] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Permisos] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permisos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [nvarchar](100) NOT NULL,
	[Descripcion] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_Permisos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Permisos_Codigo] UNIQUE NONCLUSTERED 
(
	[Codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Personas] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Personas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TipoDocumentoId] [tinyint] NULL,
	[NumeroDocumento] [nvarchar](30) NULL,
	[Nombres] [nvarchar](100) NOT NULL,
	[Apellidos] [nvarchar](100) NOT NULL,
	[Nacionalidad] [nvarchar](60) NULL,
	[FechaNacimiento] [date] NULL,
	[EmailContacto] [nvarchar](150) NULL,
	[Telefono] [nvarchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Platillos] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Platillos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoriaId] [int] NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[Descripcion] [nvarchar](500) NOT NULL,
	[Precio] [decimal](18, 2) NOT NULL,
	[ImagenUrl] [nvarchar](500) NULL,
	[Disponible] [bit] NOT NULL,
	[TiempoPreparacionMinutos] [smallint] NOT NULL,
	[Activo] [bit] NOT NULL,
 CONSTRAINT [PK_Platillos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Platillos_Categoria_Nombre] UNIQUE NONCLUSTERED 
(
	[CategoriaId] ASC,
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[RefreshTokens] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RefreshTokens](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[TokenHash] [varchar](128) NOT NULL,
	[FamiliaId] [uniqueidentifier] NOT NULL,
	[FechaCreacionUtc] [datetime2](0) NOT NULL,
	[FechaExpiracionUtc] [datetime2](0) NOT NULL,
	[FechaUsoUtc] [datetime2](0) NULL,
	[FechaRevocacionUtc] [datetime2](0) NULL,
	[MotivoRevocacion] [nvarchar](200) NULL,
	[ReemplazadoPorId] [int] NULL,
 CONSTRAINT [PK_RefreshTokens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_RefreshTokens_TokenHash] UNIQUE NONCLUSTERED 
(
	[TokenHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[ReglasTarifa] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReglasTarifa](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TipoHabitacionId] [int] NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[Tipo] [tinyint] NOT NULL,
	[FechaInicio] [date] NULL,
	[FechaFin] [date] NULL,
	[DiaSemana] [tinyint] NULL,
	[TipoAjuste] [tinyint] NOT NULL,
	[Valor] [decimal](18, 2) NOT NULL,
	[Prioridad] [int] NOT NULL,
	[Activo] [bit] NOT NULL,
 CONSTRAINT [PK_ReglasTarifa] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[ReservacionesMesa] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReservacionesMesa](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MesaId] [int] NOT NULL,
	[ClienteId] [int] NOT NULL,
	[FechaInicioUtc] [datetime2](0) NOT NULL,
	[FechaFinUtc] [datetime2](0) NOT NULL,
	[CantidadPersonas] [tinyint] NOT NULL,
	[Estado] [tinyint] NOT NULL,
	[Observaciones] [nvarchar](500) NULL,
 CONSTRAINT [PK_ReservacionesMesa] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[ReversosPago] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReversosPago](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PagoId] [int] NOT NULL,
	[Monto] [decimal](18, 2) NOT NULL,
	[Motivo] [nvarchar](300) NOT NULL,
	[FechaUtc] [datetime2](0) NOT NULL,
	[UsuarioId] [int] NOT NULL,
 CONSTRAINT [PK_ReversosPago] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Roles] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
	[Descripcion] [nvarchar](200) NULL,
	[Activo] [bit] NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Roles_Nombre] UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[RolPermisos] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolPermisos](
	[RolId] [int] NOT NULL,
	[PermisoId] [int] NOT NULL,
 CONSTRAINT [PK_RolPermisos] PRIMARY KEY CLUSTERED 
(
	[RolId] ASC,
	[PermisoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[SolicitudesServicio] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SolicitudesServicio](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HabitacionId] [int] NOT NULL,
	[EstadiaId] [int] NULL,
	[TipoSolicitudId] [int] NOT NULL,
	[Descripcion] [nvarchar](500) NOT NULL,
	[Prioridad] [tinyint] NOT NULL,
	[Estado] [tinyint] NOT NULL,
	[SolicitadoPorUsuarioId] [int] NOT NULL,
	[AsignadoAUsuarioId] [int] NULL,
	[FechaSolicitudUtc] [datetime2](0) NOT NULL,
	[FechaPreferidaUtc] [datetime2](0) NULL,
	[FechaAsignacionUtc] [datetime2](0) NULL,
	[FechaInicioUtc] [datetime2](0) NULL,
	[FechaCompletadaUtc] [datetime2](0) NULL,
	[ComentarioCierre] [nvarchar](500) NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_SolicitudesServicio] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[TiposDocumento] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TiposDocumento](
	[Id] [tinyint] NOT NULL,
	[Nombre] [nvarchar](60) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[TiposHabitacion] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TiposHabitacion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](60) NOT NULL,
	[Descripcion] [nvarchar](500) NOT NULL,
	[CapacidadAdultos] [tinyint] NOT NULL,
	[CapacidadNinos] [tinyint] NOT NULL,
	[CapacidadMaxima] [tinyint] NOT NULL,
	[OcupacionBase] [tinyint] NOT NULL,
	[PrecioBasePorNoche] [decimal](18, 2) NOT NULL,
	[PrecioPersonaAdicional] [decimal](18, 2) NOT NULL,
	[Activo] [bit] NOT NULL,
 CONSTRAINT [PK_TiposHabitacion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_TiposHabitacion_Nombre] UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[TiposPenalizacion] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TiposPenalizacion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[MontoReferencia] [decimal](18, 2) NULL,
	[Activo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[TiposSolicitud] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TiposSolicitud](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](60) NOT NULL,
	[Descripcion] [nvarchar](200) NULL,
	[Area] [tinyint] NOT NULL,
	[TiempoObjetivoMinutos] [smallint] NOT NULL,
	[Activo] [bit] NOT NULL,
 CONSTRAINT [PK_TiposSolicitud] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_TiposSolicitud_Nombre] UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[UsuarioRoles] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsuarioRoles](
	[UsuarioId] [int] NOT NULL,
	[RolId] [int] NOT NULL,
 CONSTRAINT [PK_UsuarioRoles] PRIMARY KEY CLUSTERED 
(
	[UsuarioId] ASC,
	[RolId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Usuarios] Fecha de script: 12/11/2026 14:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuarios](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NombreUsuario] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](150) NOT NULL,
	[PasswordHash] [nvarchar](500) NOT NULL,
	[Activo] [bit] NOT NULL,
	[IntentosFallidos] [int] NOT NULL,
	[BloqueadoHastaUtc] [datetime2](0) NULL,
	[UltimoAccesoUtc] [datetime2](0) NULL,
	[FechaCreacionUtc] [datetime2](0) NOT NULL,
	[PersonaId] [int] NOT NULL,
 CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Usuarios_Email] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Usuarios_NombreUsuario] UNIQUE NONCLUSTERED 
(
	[NombreUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Usuarios_Persona] UNIQUE NONCLUSTERED 
(
	[PersonaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Accesos_Estadia] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Accesos_Estadia] ON [dbo].[AccesosEstadia]
(
	[EstadiaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [IX_Auditoria_Entidad] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Auditoria_Entidad] ON [dbo].[Auditoria]
(
	[Entidad] ASC,
	[EntidadId] ASC,
	[FechaUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Auditoria_Usuario] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Auditoria_Usuario] ON [dbo].[Auditoria]
(
	[UsuarioId] ASC,
	[FechaUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Bloqueos_Habitacion_Fechas] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Bloqueos_Habitacion_Fechas] ON [dbo].[BloqueosHabitacion]
(
	[HabitacionId] ASC,
	[FechaInicio] ASC,
	[FechaFin] ASC
)
WHERE ([Activo]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Cargos_Cuenta] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Cargos_Cuenta] ON [dbo].[CargosCuenta]
(
	[CuentaId] ASC
)
INCLUDE([Total],[Tipo],[FechaUtc]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Cargos_Pedido] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Cargos_Pedido] ON [dbo].[CargosCuenta]
(
	[PedidoId] ASC
)
WHERE ([PedidoId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [UX_Cargos_Revertido] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_Cargos_Revertido] ON [dbo].[CargosCuenta]
(
	[CargoRevertidoId] ASC
)
WHERE ([CargoRevertidoId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Cuentas_Cliente] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Cuentas_Cliente] ON [dbo].[Cuentas]
(
	[ClienteId] ASC,
	[Estado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [UX_Cuentas_Reservacion] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_Cuentas_Reservacion] ON [dbo].[Cuentas]
(
	[ReservacionId] ASC
)
WHERE ([ReservacionId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [UX_DescuentosAplicados_Incidencia] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_DescuentosAplicados_Incidencia] ON [dbo].[DescuentosAplicados]
(
	[ReservacionHabitacionId] ASC,
	[IndisponibilidadId] ASC
)
WHERE ([IndisponibilidadId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_DetFac_Factura] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_DetFac_Factura] ON [dbo].[DetalleFacturas]
(
	[FacturaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_DetPed_Pedido] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_DetPed_Pedido] ON [dbo].[DetallePedidos]
(
	[PedidoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_DetPed_Platillo] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_DetPed_Platillo] ON [dbo].[DetallePedidos]
(
	[PlatilloId] ASC
)
INCLUDE([Cantidad]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Estadias_Estado] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Estadias_Estado] ON [dbo].[Estadias]
(
	[Estado] ASC
)
INCLUDE([ReservacionHabitacionId],[CheckInRealUtc]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Facturas_Cuenta] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Facturas_Cuenta] ON [dbo].[Facturas]
(
	[CuentaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Facturas_Fecha] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Facturas_Fecha] ON [dbo].[Facturas]
(
	[FechaEmisionUtc] ASC
)
INCLUDE([Total],[Estado]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [UX_FotosHabitacion_Principal] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_FotosHabitacion_Principal] ON [dbo].[FotosHabitacion]
(
	[HabitacionId] ASC
)
WHERE ([EsPrincipal]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_HabitacionAmenidades_Amenidad] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_HabitacionAmenidades_Amenidad] ON [dbo].[HabitacionAmenidades]
(
	[AmenidadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Habitaciones_Edificio_Piso] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Habitaciones_Edificio_Piso] ON [dbo].[Habitaciones]
(
	[EdificioId] ASC,
	[Piso] ASC
)
INCLUDE([Numero],[Estado]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Habitaciones_Tipo] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Habitaciones_Tipo] ON [dbo].[Habitaciones]
(
	[TipoHabitacionId] ASC
)
INCLUDE([Estado]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_HistEstHab_Habitacion] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_HistEstHab_Habitacion] ON [dbo].[HistorialEstadosHabitacion]
(
	[HabitacionId] ASC,
	[FechaUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_HistPed_Pedido] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_HistPed_Pedido] ON [dbo].[HistorialPedidos]
(
	[PedidoId] ASC,
	[FechaUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_HistRes_Reservacion] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_HistRes_Reservacion] ON [dbo].[HistorialReservaciones]
(
	[ReservacionId] ASC,
	[FechaUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_HistSol_Solicitud] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_HistSol_Solicitud] ON [dbo].[HistorialSolicitudes]
(
	[SolicitudId] ASC,
	[FechaUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Huespedes_ResHab] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Huespedes_ResHab] ON [dbo].[HuespedesReservacion]
(
	[ReservacionHabitacionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_IndisponibilidadesAmenidad_Busqueda] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_IndisponibilidadesAmenidad_Busqueda] ON [dbo].[IndisponibilidadesAmenidad]
(
	[HabitacionId] ASC,
	[AmenidadId] ASC,
	[FechaInicio] ASC,
	[FechaFin] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Correos_Pendientes] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Correos_Pendientes] ON [dbo].[NotificacionesCorreo]
(
	[Estado] ASC,
	[FechaCreacionUtc] ASC
)
WHERE ([Estado] IN ((1), (3)))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Pagos_Cuenta] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Pagos_Cuenta] ON [dbo].[Pagos]
(
	[CuentaId] ASC
)
INCLUDE([Monto],[Estado]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Pagos_Fecha] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Pagos_Fecha] ON [dbo].[Pagos]
(
	[FechaPagoUtc] ASC
)
INCLUDE([Monto],[MetodoPago],[Estado]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Pedidos_Cliente] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Pedidos_Cliente] ON [dbo].[PedidosRestaurante]
(
	[ClienteId] ASC,
	[FechaPedidoUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Pedidos_Cuenta] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Pedidos_Cuenta] ON [dbo].[PedidosRestaurante]
(
	[CuentaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Pedidos_Estado_Fecha] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Pedidos_Estado_Fecha] ON [dbo].[PedidosRestaurante]
(
	[EstadoPedido] ASC,
	[FechaPedidoUtc] ASC
)
INCLUDE([Modalidad],[EstadiaId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [UX_Personas_Documento] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_Personas_Documento] ON [dbo].[Personas]
(
	[TipoDocumentoId] ASC,
	[NumeroDocumento] ASC
)
WHERE ([NumeroDocumento] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_RefreshTokens_Familia] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_RefreshTokens_Familia] ON [dbo].[RefreshTokens]
(
	[FamiliaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_RefreshTokens_Usuario] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_RefreshTokens_Usuario] ON [dbo].[RefreshTokens]
(
	[UsuarioId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Reservaciones_Cliente_Estado] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Reservaciones_Cliente_Estado] ON [dbo].[Reservaciones]
(
	[ClienteId] ASC,
	[Estado] ASC
)
INCLUDE([FechaInicio],[FechaFin],[Codigo]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Reservaciones_Estado_Expiracion] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Reservaciones_Estado_Expiracion] ON [dbo].[Reservaciones]
(
	[Estado] ASC,
	[FechaExpiracionPendienteUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Reservaciones_Fechas] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Reservaciones_Fechas] ON [dbo].[Reservaciones]
(
	[FechaInicio] ASC,
	[FechaFin] ASC
)
INCLUDE([Estado]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_ResMesa_Mesa_Fechas] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_ResMesa_Mesa_Fechas] ON [dbo].[ReservacionesMesa]
(
	[MesaId] ASC,
	[FechaInicioUtc] ASC,
	[FechaFinUtc] ASC
)
INCLUDE([Estado]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_ResHab_Habitacion_Fechas] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_ResHab_Habitacion_Fechas] ON [dbo].[ReservacionHabitaciones]
(
	[HabitacionId] ASC,
	[FechaInicio] ASC,
	[FechaFin] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_ResHab_Reservacion] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_ResHab_Reservacion] ON [dbo].[ReservacionHabitaciones]
(
	[ReservacionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [UX_ResNoches_Habitacion_Fecha_Activa] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_ResNoches_Habitacion_Fecha_Activa] ON [dbo].[ReservacionNoches]
(
	[HabitacionId] ASC,
	[Fecha] ASC
)
WHERE ([Activa]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Reversos_Pago] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Reversos_Pago] ON [dbo].[ReversosPago]
(
	[PagoId] ASC
)
INCLUDE([Monto]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Solicitudes_Asignado] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Solicitudes_Asignado] ON [dbo].[SolicitudesServicio]
(
	[AsignadoAUsuarioId] ASC,
	[Estado] ASC
)
WHERE ([AsignadoAUsuarioId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Solicitudes_Estadia] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Solicitudes_Estadia] ON [dbo].[SolicitudesServicio]
(
	[EstadiaId] ASC
)
WHERE ([EstadiaId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Solicitudes_Estado_Prioridad] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Solicitudes_Estado_Prioridad] ON [dbo].[SolicitudesServicio]
(
	[Estado] ASC,
	[Prioridad] DESC,
	[FechaSolicitudUtc] ASC
)
INCLUDE([HabitacionId],[TipoSolicitudId],[AsignadoAUsuarioId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Solicitudes_Habitacion] Fecha de script: 12/11/2026 14:41:38 ******/
CREATE NONCLUSTERED INDEX [IX_Solicitudes_Habitacion] ON [dbo].[SolicitudesServicio]
(
	[HabitacionId] ASC,
	[FechaSolicitudUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AccesosEstadia] ADD  CONSTRAINT [DF_Accesos_Emision]  DEFAULT (sysutcdatetime()) FOR [FechaEmisionUtc]
GO
ALTER TABLE [dbo].[Amenidades] ADD  CONSTRAINT [DF_Amenidades_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[Auditoria] ADD  CONSTRAINT [DF_Auditoria_Fecha]  DEFAULT (sysutcdatetime()) FOR [FechaUtc]
GO
ALTER TABLE [dbo].[BloqueosHabitacion] ADD  CONSTRAINT [DF_Bloqueos_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[CargosCuenta] ADD  CONSTRAINT [DF_Cargos_Descuento]  DEFAULT ((0)) FOR [Descuento]
GO
ALTER TABLE [dbo].[CargosCuenta] ADD  CONSTRAINT [DF_Cargos_Impuestos]  DEFAULT ((0)) FOR [Impuestos]
GO
ALTER TABLE [dbo].[CargosCuenta] ADD  CONSTRAINT [DF_Cargos_Fecha]  DEFAULT (sysutcdatetime()) FOR [FechaUtc]
GO
ALTER TABLE [dbo].[CategoriasPlatillo] ADD  CONSTRAINT [DF_Categorias_Orden]  DEFAULT ((0)) FOR [Orden]
GO
ALTER TABLE [dbo].[CategoriasPlatillo] ADD  CONSTRAINT [DF_Categorias_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[Clientes] ADD  CONSTRAINT [DF_Clientes_Descuento]  DEFAULT ((0)) FOR [PorcentajeDescuento]
GO
ALTER TABLE [dbo].[Clientes] ADD  CONSTRAINT [DF_Clientes_FechaRegistro]  DEFAULT (sysutcdatetime()) FOR [FechaRegistroUtc]
GO
ALTER TABLE [dbo].[Clientes] ADD  CONSTRAINT [DF_Clientes_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[Cuentas] ADD  CONSTRAINT [DF_Cuentas_Estado]  DEFAULT ((1)) FOR [Estado]
GO
ALTER TABLE [dbo].[Cuentas] ADD  CONSTRAINT [DF_Cuentas_Apertura]  DEFAULT (sysutcdatetime()) FOR [FechaAperturaUtc]
GO
ALTER TABLE [dbo].[Descuentos] ADD  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[DescuentosAplicados] ADD  DEFAULT (sysutcdatetime()) FOR [FechaUtc]
GO
ALTER TABLE [dbo].[DetallePedidos] ADD  CONSTRAINT [DF_DetPed_Descuento]  DEFAULT ((0)) FOR [Descuento]
GO
ALTER TABLE [dbo].[Edificios] ADD  CONSTRAINT [DF_Edificios_Pisos]  DEFAULT ((6)) FOR [CantidadPisos]
GO
ALTER TABLE [dbo].[Edificios] ADD  CONSTRAINT [DF_Edificios_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[Estadias] ADD  CONSTRAINT [DF_Estadias_Estado]  DEFAULT ((1)) FOR [Estado]
GO
ALTER TABLE [dbo].[Facturas] ADD  CONSTRAINT [DF_Facturas_Fecha]  DEFAULT (sysutcdatetime()) FOR [FechaEmisionUtc]
GO
ALTER TABLE [dbo].[Facturas] ADD  CONSTRAINT [DF_Facturas_Estado]  DEFAULT ((1)) FOR [Estado]
GO
ALTER TABLE [dbo].[FotosHabitacion] ADD  CONSTRAINT [DF_FotosHabitacion_Orden]  DEFAULT ((0)) FOR [Orden]
GO
ALTER TABLE [dbo].[FotosHabitacion] ADD  CONSTRAINT [DF_FotosHabitacion_Principal]  DEFAULT ((0)) FOR [EsPrincipal]
GO
ALTER TABLE [dbo].[HabitacionAmenidades] ADD  CONSTRAINT [DF_HabitacionAmenidades_Cantidad]  DEFAULT ((1)) FOR [Cantidad]
GO
ALTER TABLE [dbo].[Habitaciones] ADD  CONSTRAINT [DF_Habitaciones_Estado]  DEFAULT ((1)) FOR [Estado]
GO
ALTER TABLE [dbo].[HistorialEstadosHabitacion] ADD  CONSTRAINT [DF_HistEstHab_Fecha]  DEFAULT (sysutcdatetime()) FOR [FechaUtc]
GO
ALTER TABLE [dbo].[HistorialPedidos] ADD  CONSTRAINT [DF_HistPed_Fecha]  DEFAULT (sysutcdatetime()) FOR [FechaUtc]
GO
ALTER TABLE [dbo].[HistorialReservaciones] ADD  CONSTRAINT [DF_HistRes_Fecha]  DEFAULT (sysutcdatetime()) FOR [FechaUtc]
GO
ALTER TABLE [dbo].[HistorialSolicitudes] ADD  CONSTRAINT [DF_HistSol_Fecha]  DEFAULT (sysutcdatetime()) FOR [FechaUtc]
GO
ALTER TABLE [dbo].[HuespedesReservacion] ADD  CONSTRAINT [DF_Huespedes_Titular]  DEFAULT ((0)) FOR [EsTitular]
GO
ALTER TABLE [dbo].[Impuestos] ADD  CONSTRAINT [DF_Impuestos_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[IndisponibilidadesAmenidad] ADD  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[Mesas] ADD  CONSTRAINT [DF_Mesas_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[NotificacionesCorreo] ADD  CONSTRAINT [DF_Correos_Estado]  DEFAULT ((1)) FOR [Estado]
GO
ALTER TABLE [dbo].[NotificacionesCorreo] ADD  CONSTRAINT [DF_Correos_Intentos]  DEFAULT ((0)) FOR [Intentos]
GO
ALTER TABLE [dbo].[NotificacionesCorreo] ADD  CONSTRAINT [DF_Correos_Creacion]  DEFAULT (sysutcdatetime()) FOR [FechaCreacionUtc]
GO
ALTER TABLE [dbo].[Pagos] ADD  CONSTRAINT [DF_Pagos_Fecha]  DEFAULT (sysutcdatetime()) FOR [FechaPagoUtc]
GO
ALTER TABLE [dbo].[Pagos] ADD  CONSTRAINT [DF_Pagos_Estado]  DEFAULT ((1)) FOR [Estado]
GO
ALTER TABLE [dbo].[PedidosRestaurante] ADD  CONSTRAINT [DF_Pedidos_Fecha]  DEFAULT (sysutcdatetime()) FOR [FechaPedidoUtc]
GO
ALTER TABLE [dbo].[PedidosRestaurante] ADD  CONSTRAINT [DF_Pedidos_Estado]  DEFAULT ((1)) FOR [EstadoPedido]
GO
ALTER TABLE [dbo].[PedidosRestaurante] ADD  CONSTRAINT [DF_Pedidos_Subtotal]  DEFAULT ((0)) FOR [Subtotal]
GO
ALTER TABLE [dbo].[PedidosRestaurante] ADD  CONSTRAINT [DF_Pedidos_Descuento]  DEFAULT ((0)) FOR [Descuento]
GO
ALTER TABLE [dbo].[PedidosRestaurante] ADD  CONSTRAINT [DF_Pedidos_Impuestos]  DEFAULT ((0)) FOR [Impuestos]
GO
ALTER TABLE [dbo].[PedidosRestaurante] ADD  CONSTRAINT [DF_Pedidos_Recargos]  DEFAULT ((0)) FOR [Recargos]
GO
ALTER TABLE [dbo].[PedidosRestaurante] ADD  CONSTRAINT [DF_Pedidos_Total]  DEFAULT ((0)) FOR [Total]
GO
ALTER TABLE [dbo].[Platillos] ADD  CONSTRAINT [DF_Platillos_Disponible]  DEFAULT ((1)) FOR [Disponible]
GO
ALTER TABLE [dbo].[Platillos] ADD  CONSTRAINT [DF_Platillos_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[RefreshTokens] ADD  CONSTRAINT [DF_RefreshTokens_FechaCreacion]  DEFAULT (sysutcdatetime()) FOR [FechaCreacionUtc]
GO
ALTER TABLE [dbo].[ReglasTarifa] ADD  CONSTRAINT [DF_ReglasTarifa_Prioridad]  DEFAULT ((0)) FOR [Prioridad]
GO
ALTER TABLE [dbo].[ReglasTarifa] ADD  CONSTRAINT [DF_ReglasTarifa_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[ReservacionAmenidades] ADD  DEFAULT (sysutcdatetime()) FOR [FechaRegistroUtc]
GO
ALTER TABLE [dbo].[Reservaciones] ADD  CONSTRAINT [DF_Reservaciones_Estado]  DEFAULT ((1)) FOR [Estado]
GO
ALTER TABLE [dbo].[Reservaciones] ADD  CONSTRAINT [DF_Reservaciones_Ninos]  DEFAULT ((0)) FOR [CantidadNinos]
GO
ALTER TABLE [dbo].[Reservaciones] ADD  CONSTRAINT [DF_Reservaciones_Subtotal]  DEFAULT ((0)) FOR [Subtotal]
GO
ALTER TABLE [dbo].[Reservaciones] ADD  CONSTRAINT [DF_Reservaciones_Descuento]  DEFAULT ((0)) FOR [Descuento]
GO
ALTER TABLE [dbo].[Reservaciones] ADD  CONSTRAINT [DF_Reservaciones_Impuestos]  DEFAULT ((0)) FOR [Impuestos]
GO
ALTER TABLE [dbo].[Reservaciones] ADD  CONSTRAINT [DF_Reservaciones_Total]  DEFAULT ((0)) FOR [Total]
GO
ALTER TABLE [dbo].[Reservaciones] ADD  CONSTRAINT [DF_Reservaciones_Anticipo]  DEFAULT ((0)) FOR [AnticipoRequerido]
GO
ALTER TABLE [dbo].[Reservaciones] ADD  CONSTRAINT [DF_Reservaciones_FechaCreacion]  DEFAULT (sysutcdatetime()) FOR [FechaCreacionUtc]
GO
ALTER TABLE [dbo].[ReservacionesMesa] ADD  CONSTRAINT [DF_ResMesa_Estado]  DEFAULT ((1)) FOR [Estado]
GO
ALTER TABLE [dbo].[ReservacionHabitaciones] ADD  CONSTRAINT [DF_ResHab_Ninos]  DEFAULT ((0)) FOR [Ninos]
GO
ALTER TABLE [dbo].[ReservacionHabitaciones] ADD  CONSTRAINT [DF_ResHab_Descuento]  DEFAULT ((0)) FOR [Descuento]
GO
ALTER TABLE [dbo].[ReservacionNoches] ADD  CONSTRAINT [DF_ResNoches_Ajuste]  DEFAULT ((0)) FOR [AjusteTarifa]
GO
ALTER TABLE [dbo].[ReservacionNoches] ADD  CONSTRAINT [DF_ResNoches_Activa]  DEFAULT ((1)) FOR [Activa]
GO
ALTER TABLE [dbo].[ReversosPago] ADD  CONSTRAINT [DF_Reversos_Fecha]  DEFAULT (sysutcdatetime()) FOR [FechaUtc]
GO
ALTER TABLE [dbo].[Roles] ADD  CONSTRAINT [DF_Roles_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[SolicitudesServicio] ADD  CONSTRAINT [DF_Solicitudes_Prioridad]  DEFAULT ((2)) FOR [Prioridad]
GO
ALTER TABLE [dbo].[SolicitudesServicio] ADD  CONSTRAINT [DF_Solicitudes_Estado]  DEFAULT ((1)) FOR [Estado]
GO
ALTER TABLE [dbo].[SolicitudesServicio] ADD  CONSTRAINT [DF_Solicitudes_Fecha]  DEFAULT (sysutcdatetime()) FOR [FechaSolicitudUtc]
GO
ALTER TABLE [dbo].[TiposHabitacion] ADD  CONSTRAINT [DF_TiposHabitacion_PrecioAdicional]  DEFAULT ((0)) FOR [PrecioPersonaAdicional]
GO
ALTER TABLE [dbo].[TiposHabitacion] ADD  CONSTRAINT [DF_TiposHabitacion_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[TiposPenalizacion] ADD  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[TiposSolicitud] ADD  CONSTRAINT [DF_TiposSolicitud_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[Usuarios] ADD  CONSTRAINT [DF_Usuarios_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[Usuarios] ADD  CONSTRAINT [DF_Usuarios_Intentos]  DEFAULT ((0)) FOR [IntentosFallidos]
GO
ALTER TABLE [dbo].[Usuarios] ADD  CONSTRAINT [DF_Usuarios_FechaCreacion]  DEFAULT (sysutcdatetime()) FOR [FechaCreacionUtc]
GO
ALTER TABLE [dbo].[AccesosEstadia]  WITH CHECK ADD  CONSTRAINT [FK_Accesos_Estadias] FOREIGN KEY([EstadiaId])
REFERENCES [dbo].[Estadias] ([Id])
GO
ALTER TABLE [dbo].[AccesosEstadia] CHECK CONSTRAINT [FK_Accesos_Estadias]
GO
ALTER TABLE [dbo].[Auditoria]  WITH CHECK ADD  CONSTRAINT [FK_Auditoria_Usuarios] FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[Auditoria] CHECK CONSTRAINT [FK_Auditoria_Usuarios]
GO
ALTER TABLE [dbo].[BloqueosHabitacion]  WITH CHECK ADD  CONSTRAINT [FK_Bloqueos_Habitaciones] FOREIGN KEY([HabitacionId])
REFERENCES [dbo].[Habitaciones] ([Id])
GO
ALTER TABLE [dbo].[BloqueosHabitacion] CHECK CONSTRAINT [FK_Bloqueos_Habitaciones]
GO
ALTER TABLE [dbo].[BloqueosHabitacion]  WITH CHECK ADD  CONSTRAINT [FK_Bloqueos_Usuarios] FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[BloqueosHabitacion] CHECK CONSTRAINT [FK_Bloqueos_Usuarios]
GO
ALTER TABLE [dbo].[CargosCuenta]  WITH CHECK ADD  CONSTRAINT [FK_Cargos_Autorizador] FOREIGN KEY([AutorizadoPorId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[CargosCuenta] CHECK CONSTRAINT [FK_Cargos_Autorizador]
GO
ALTER TABLE [dbo].[CargosCuenta]  WITH CHECK ADD  CONSTRAINT [FK_Cargos_Cuentas] FOREIGN KEY([CuentaId])
REFERENCES [dbo].[Cuentas] ([Id])
GO
ALTER TABLE [dbo].[CargosCuenta] CHECK CONSTRAINT [FK_Cargos_Cuentas]
GO
ALTER TABLE [dbo].[CargosCuenta]  WITH CHECK ADD  CONSTRAINT [FK_Cargos_Estadias] FOREIGN KEY([EstadiaId])
REFERENCES [dbo].[Estadias] ([Id])
GO
ALTER TABLE [dbo].[CargosCuenta] CHECK CONSTRAINT [FK_Cargos_Estadias]
GO
ALTER TABLE [dbo].[CargosCuenta]  WITH CHECK ADD  CONSTRAINT [FK_Cargos_Impuestos] FOREIGN KEY([ImpuestoId])
REFERENCES [dbo].[Impuestos] ([Id])
GO
ALTER TABLE [dbo].[CargosCuenta] CHECK CONSTRAINT [FK_Cargos_Impuestos]
GO
ALTER TABLE [dbo].[CargosCuenta]  WITH CHECK ADD  CONSTRAINT [FK_Cargos_Pedidos] FOREIGN KEY([PedidoId])
REFERENCES [dbo].[PedidosRestaurante] ([Id])
GO
ALTER TABLE [dbo].[CargosCuenta] CHECK CONSTRAINT [FK_Cargos_Pedidos]
GO
ALTER TABLE [dbo].[CargosCuenta]  WITH CHECK ADD  CONSTRAINT [FK_Cargos_Revertido] FOREIGN KEY([CargoRevertidoId])
REFERENCES [dbo].[CargosCuenta] ([Id])
GO
ALTER TABLE [dbo].[CargosCuenta] CHECK CONSTRAINT [FK_Cargos_Revertido]
GO
ALTER TABLE [dbo].[CargosCuenta]  WITH CHECK ADD  CONSTRAINT [FK_Cargos_TiposPenalizacion] FOREIGN KEY([TipoPenalizacionId])
REFERENCES [dbo].[TiposPenalizacion] ([Id])
GO
ALTER TABLE [dbo].[CargosCuenta] CHECK CONSTRAINT [FK_Cargos_TiposPenalizacion]
GO
ALTER TABLE [dbo].[CargosCuenta]  WITH CHECK ADD  CONSTRAINT [FK_Cargos_Usuarios] FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[CargosCuenta] CHECK CONSTRAINT [FK_Cargos_Usuarios]
GO
ALTER TABLE [dbo].[Clientes]  WITH CHECK ADD  CONSTRAINT [FK_Clientes_Personas] FOREIGN KEY([PersonaId])
REFERENCES [dbo].[Personas] ([Id])
GO
ALTER TABLE [dbo].[Clientes] CHECK CONSTRAINT [FK_Clientes_Personas]
GO
ALTER TABLE [dbo].[Cuentas]  WITH CHECK ADD  CONSTRAINT [FK_Cuentas_Clientes] FOREIGN KEY([ClienteId])
REFERENCES [dbo].[Clientes] ([Id])
GO
ALTER TABLE [dbo].[Cuentas] CHECK CONSTRAINT [FK_Cuentas_Clientes]
GO
ALTER TABLE [dbo].[Cuentas]  WITH CHECK ADD  CONSTRAINT [FK_Cuentas_Reservaciones] FOREIGN KEY([ReservacionId], [ClienteId])
REFERENCES [dbo].[Reservaciones] ([Id], [ClienteId])
GO
ALTER TABLE [dbo].[Cuentas] CHECK CONSTRAINT [FK_Cuentas_Reservaciones]
GO
ALTER TABLE [dbo].[DescuentosAplicados]  WITH CHECK ADD FOREIGN KEY([DescuentoId])
REFERENCES [dbo].[Descuentos] ([Id])
GO
ALTER TABLE [dbo].[DescuentosAplicados]  WITH CHECK ADD FOREIGN KEY([ReservacionHabitacionId])
REFERENCES [dbo].[ReservacionHabitaciones] ([Id])
GO
ALTER TABLE [dbo].[DescuentosAplicados]  WITH CHECK ADD FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[DescuentosAplicados]  WITH CHECK ADD FOREIGN KEY([ReservacionHabitacionId], [HabitacionId], [AmenidadId])
REFERENCES [dbo].[ReservacionAmenidades] ([ReservacionHabitacionId], [HabitacionId], [AmenidadId])
GO
ALTER TABLE [dbo].[DescuentosAplicados]  WITH CHECK ADD FOREIGN KEY([IndisponibilidadId], [HabitacionId], [AmenidadId])
REFERENCES [dbo].[IndisponibilidadesAmenidad] ([Id], [HabitacionId], [AmenidadId])
GO
ALTER TABLE [dbo].[DetalleFacturas]  WITH CHECK ADD  CONSTRAINT [FK_DetFac_Cargos] FOREIGN KEY([CargoCuentaId])
REFERENCES [dbo].[CargosCuenta] ([Id])
GO
ALTER TABLE [dbo].[DetalleFacturas] CHECK CONSTRAINT [FK_DetFac_Cargos]
GO
ALTER TABLE [dbo].[DetalleFacturas]  WITH CHECK ADD  CONSTRAINT [FK_DetFac_Facturas] FOREIGN KEY([FacturaId])
REFERENCES [dbo].[Facturas] ([Id])
GO
ALTER TABLE [dbo].[DetalleFacturas] CHECK CONSTRAINT [FK_DetFac_Facturas]
GO
ALTER TABLE [dbo].[DetallePedidos]  WITH CHECK ADD  CONSTRAINT [FK_DetPed_Pedidos] FOREIGN KEY([PedidoId])
REFERENCES [dbo].[PedidosRestaurante] ([Id])
GO
ALTER TABLE [dbo].[DetallePedidos] CHECK CONSTRAINT [FK_DetPed_Pedidos]
GO
ALTER TABLE [dbo].[DetallePedidos]  WITH CHECK ADD  CONSTRAINT [FK_DetPed_Platillos] FOREIGN KEY([PlatilloId])
REFERENCES [dbo].[Platillos] ([Id])
GO
ALTER TABLE [dbo].[DetallePedidos] CHECK CONSTRAINT [FK_DetPed_Platillos]
GO
ALTER TABLE [dbo].[Estadias]  WITH CHECK ADD  CONSTRAINT [FK_Estadias_ResHab] FOREIGN KEY([ReservacionHabitacionId])
REFERENCES [dbo].[ReservacionHabitaciones] ([Id])
GO
ALTER TABLE [dbo].[Estadias] CHECK CONSTRAINT [FK_Estadias_ResHab]
GO
ALTER TABLE [dbo].[Estadias]  WITH CHECK ADD  CONSTRAINT [FK_Estadias_UsuarioCheckIn] FOREIGN KEY([UsuarioCheckInId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[Estadias] CHECK CONSTRAINT [FK_Estadias_UsuarioCheckIn]
GO
ALTER TABLE [dbo].[Estadias]  WITH CHECK ADD  CONSTRAINT [FK_Estadias_UsuarioCheckOut] FOREIGN KEY([UsuarioCheckOutId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[Estadias] CHECK CONSTRAINT [FK_Estadias_UsuarioCheckOut]
GO
ALTER TABLE [dbo].[Facturas]  WITH CHECK ADD  CONSTRAINT [FK_Facturas_Cuentas] FOREIGN KEY([CuentaId])
REFERENCES [dbo].[Cuentas] ([Id])
GO
ALTER TABLE [dbo].[Facturas] CHECK CONSTRAINT [FK_Facturas_Cuentas]
GO
ALTER TABLE [dbo].[Facturas]  WITH CHECK ADD  CONSTRAINT [FK_Facturas_TiposDocumento] FOREIGN KEY([TipoDocumentoReceptorId])
REFERENCES [dbo].[TiposDocumento] ([Id])
GO
ALTER TABLE [dbo].[Facturas] CHECK CONSTRAINT [FK_Facturas_TiposDocumento]
GO
ALTER TABLE [dbo].[FotosHabitacion]  WITH CHECK ADD  CONSTRAINT [FK_FotosHabitacion_Habitaciones] FOREIGN KEY([HabitacionId])
REFERENCES [dbo].[Habitaciones] ([Id])
GO
ALTER TABLE [dbo].[FotosHabitacion] CHECK CONSTRAINT [FK_FotosHabitacion_Habitaciones]
GO
ALTER TABLE [dbo].[HabitacionAmenidades]  WITH CHECK ADD  CONSTRAINT [FK_HabitacionAmenidades_Amenidades] FOREIGN KEY([AmenidadId])
REFERENCES [dbo].[Amenidades] ([Id])
GO
ALTER TABLE [dbo].[HabitacionAmenidades] CHECK CONSTRAINT [FK_HabitacionAmenidades_Amenidades]
GO
ALTER TABLE [dbo].[HabitacionAmenidades]  WITH CHECK ADD  CONSTRAINT [FK_HabitacionAmenidades_Habitaciones] FOREIGN KEY([HabitacionId])
REFERENCES [dbo].[Habitaciones] ([Id])
GO
ALTER TABLE [dbo].[HabitacionAmenidades] CHECK CONSTRAINT [FK_HabitacionAmenidades_Habitaciones]
GO
ALTER TABLE [dbo].[Habitaciones]  WITH CHECK ADD  CONSTRAINT [FK_Habitaciones_Edificios] FOREIGN KEY([EdificioId])
REFERENCES [dbo].[Edificios] ([Id])
GO
ALTER TABLE [dbo].[Habitaciones] CHECK CONSTRAINT [FK_Habitaciones_Edificios]
GO
ALTER TABLE [dbo].[Habitaciones]  WITH CHECK ADD  CONSTRAINT [FK_Habitaciones_Tipos] FOREIGN KEY([TipoHabitacionId])
REFERENCES [dbo].[TiposHabitacion] ([Id])
GO
ALTER TABLE [dbo].[Habitaciones] CHECK CONSTRAINT [FK_Habitaciones_Tipos]
GO
ALTER TABLE [dbo].[HistorialEstadosHabitacion]  WITH CHECK ADD  CONSTRAINT [FK_HistEstHab_Habitaciones] FOREIGN KEY([HabitacionId])
REFERENCES [dbo].[Habitaciones] ([Id])
GO
ALTER TABLE [dbo].[HistorialEstadosHabitacion] CHECK CONSTRAINT [FK_HistEstHab_Habitaciones]
GO
ALTER TABLE [dbo].[HistorialEstadosHabitacion]  WITH CHECK ADD  CONSTRAINT [FK_HistEstHab_Usuarios] FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[HistorialEstadosHabitacion] CHECK CONSTRAINT [FK_HistEstHab_Usuarios]
GO
ALTER TABLE [dbo].[HistorialPedidos]  WITH CHECK ADD  CONSTRAINT [FK_HistPed_Pedidos] FOREIGN KEY([PedidoId])
REFERENCES [dbo].[PedidosRestaurante] ([Id])
GO
ALTER TABLE [dbo].[HistorialPedidos] CHECK CONSTRAINT [FK_HistPed_Pedidos]
GO
ALTER TABLE [dbo].[HistorialPedidos]  WITH CHECK ADD  CONSTRAINT [FK_HistPed_Usuarios] FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[HistorialPedidos] CHECK CONSTRAINT [FK_HistPed_Usuarios]
GO
ALTER TABLE [dbo].[HistorialReservaciones]  WITH CHECK ADD  CONSTRAINT [FK_HistRes_Reservaciones] FOREIGN KEY([ReservacionId])
REFERENCES [dbo].[Reservaciones] ([Id])
GO
ALTER TABLE [dbo].[HistorialReservaciones] CHECK CONSTRAINT [FK_HistRes_Reservaciones]
GO
ALTER TABLE [dbo].[HistorialReservaciones]  WITH CHECK ADD  CONSTRAINT [FK_HistRes_Usuarios] FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[HistorialReservaciones] CHECK CONSTRAINT [FK_HistRes_Usuarios]
GO
ALTER TABLE [dbo].[HistorialSolicitudes]  WITH CHECK ADD  CONSTRAINT [FK_HistSol_Solicitudes] FOREIGN KEY([SolicitudId])
REFERENCES [dbo].[SolicitudesServicio] ([Id])
GO
ALTER TABLE [dbo].[HistorialSolicitudes] CHECK CONSTRAINT [FK_HistSol_Solicitudes]
GO
ALTER TABLE [dbo].[HistorialSolicitudes]  WITH CHECK ADD  CONSTRAINT [FK_HistSol_Usuarios] FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[HistorialSolicitudes] CHECK CONSTRAINT [FK_HistSol_Usuarios]
GO
ALTER TABLE [dbo].[HuespedesReservacion]  WITH CHECK ADD  CONSTRAINT [FK_Huespedes_ResHab] FOREIGN KEY([ReservacionHabitacionId])
REFERENCES [dbo].[ReservacionHabitaciones] ([Id])
GO
ALTER TABLE [dbo].[HuespedesReservacion] CHECK CONSTRAINT [FK_Huespedes_ResHab]
GO
ALTER TABLE [dbo].[HuespedesReservacion]  WITH CHECK ADD  CONSTRAINT [FK_Huespedes_TiposDocumento] FOREIGN KEY([TipoDocumentoId])
REFERENCES [dbo].[TiposDocumento] ([Id])
GO
ALTER TABLE [dbo].[HuespedesReservacion] CHECK CONSTRAINT [FK_Huespedes_TiposDocumento]
GO
ALTER TABLE [dbo].[IndisponibilidadesAmenidad]  WITH CHECK ADD FOREIGN KEY([AmenidadId])
REFERENCES [dbo].[Amenidades] ([Id])
GO
ALTER TABLE [dbo].[IndisponibilidadesAmenidad]  WITH CHECK ADD FOREIGN KEY([HabitacionId])
REFERENCES [dbo].[Habitaciones] ([Id])
GO
ALTER TABLE [dbo].[IndisponibilidadesAmenidad]  WITH CHECK ADD FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[NotificacionesCorreo]  WITH CHECK ADD  CONSTRAINT [FK_Correos_Clientes] FOREIGN KEY([ClienteId])
REFERENCES [dbo].[Clientes] ([Id])
GO
ALTER TABLE [dbo].[NotificacionesCorreo] CHECK CONSTRAINT [FK_Correos_Clientes]
GO
ALTER TABLE [dbo].[NotificacionesCorreo]  WITH CHECK ADD  CONSTRAINT [FK_Correos_Facturas] FOREIGN KEY([FacturaId])
REFERENCES [dbo].[Facturas] ([Id])
GO
ALTER TABLE [dbo].[NotificacionesCorreo] CHECK CONSTRAINT [FK_Correos_Facturas]
GO
ALTER TABLE [dbo].[NotificacionesCorreo]  WITH CHECK ADD  CONSTRAINT [FK_Correos_Reservaciones] FOREIGN KEY([ReservacionId])
REFERENCES [dbo].[Reservaciones] ([Id])
GO
ALTER TABLE [dbo].[NotificacionesCorreo] CHECK CONSTRAINT [FK_Correos_Reservaciones]
GO
ALTER TABLE [dbo].[Pagos]  WITH CHECK ADD  CONSTRAINT [FK_Pagos_Cuentas] FOREIGN KEY([CuentaId])
REFERENCES [dbo].[Cuentas] ([Id])
GO
ALTER TABLE [dbo].[Pagos] CHECK CONSTRAINT [FK_Pagos_Cuentas]
GO
ALTER TABLE [dbo].[Pagos]  WITH CHECK ADD  CONSTRAINT [FK_Pagos_Usuarios] FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[Pagos] CHECK CONSTRAINT [FK_Pagos_Usuarios]
GO
ALTER TABLE [dbo].[PedidosRestaurante]  WITH CHECK ADD  CONSTRAINT [FK_Pedidos_Clientes] FOREIGN KEY([ClienteId])
REFERENCES [dbo].[Clientes] ([Id])
GO
ALTER TABLE [dbo].[PedidosRestaurante] CHECK CONSTRAINT [FK_Pedidos_Clientes]
GO
ALTER TABLE [dbo].[PedidosRestaurante]  WITH CHECK ADD  CONSTRAINT [FK_Pedidos_Cuentas] FOREIGN KEY([CuentaId])
REFERENCES [dbo].[Cuentas] ([Id])
GO
ALTER TABLE [dbo].[PedidosRestaurante] CHECK CONSTRAINT [FK_Pedidos_Cuentas]
GO
ALTER TABLE [dbo].[PedidosRestaurante]  WITH CHECK ADD  CONSTRAINT [FK_Pedidos_Estadias] FOREIGN KEY([EstadiaId])
REFERENCES [dbo].[Estadias] ([Id])
GO
ALTER TABLE [dbo].[PedidosRestaurante] CHECK CONSTRAINT [FK_Pedidos_Estadias]
GO
ALTER TABLE [dbo].[PedidosRestaurante]  WITH CHECK ADD  CONSTRAINT [FK_Pedidos_ResMesa] FOREIGN KEY([ReservacionMesaId])
REFERENCES [dbo].[ReservacionesMesa] ([Id])
GO
ALTER TABLE [dbo].[PedidosRestaurante] CHECK CONSTRAINT [FK_Pedidos_ResMesa]
GO
ALTER TABLE [dbo].[Personas]  WITH CHECK ADD FOREIGN KEY([TipoDocumentoId])
REFERENCES [dbo].[TiposDocumento] ([Id])
GO
ALTER TABLE [dbo].[Platillos]  WITH CHECK ADD  CONSTRAINT [FK_Platillos_Categorias] FOREIGN KEY([CategoriaId])
REFERENCES [dbo].[CategoriasPlatillo] ([Id])
GO
ALTER TABLE [dbo].[Platillos] CHECK CONSTRAINT [FK_Platillos_Categorias]
GO
ALTER TABLE [dbo].[RefreshTokens]  WITH CHECK ADD  CONSTRAINT [FK_RefreshTokens_Reemplazo] FOREIGN KEY([ReemplazadoPorId])
REFERENCES [dbo].[RefreshTokens] ([Id])
GO
ALTER TABLE [dbo].[RefreshTokens] CHECK CONSTRAINT [FK_RefreshTokens_Reemplazo]
GO
ALTER TABLE [dbo].[RefreshTokens]  WITH CHECK ADD  CONSTRAINT [FK_RefreshTokens_Usuarios] FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[RefreshTokens] CHECK CONSTRAINT [FK_RefreshTokens_Usuarios]
GO
ALTER TABLE [dbo].[ReglasTarifa]  WITH CHECK ADD  CONSTRAINT [FK_ReglasTarifa_Tipos] FOREIGN KEY([TipoHabitacionId])
REFERENCES [dbo].[TiposHabitacion] ([Id])
GO
ALTER TABLE [dbo].[ReglasTarifa] CHECK CONSTRAINT [FK_ReglasTarifa_Tipos]
GO
ALTER TABLE [dbo].[ReservacionAmenidades]  WITH CHECK ADD FOREIGN KEY([AmenidadId])
REFERENCES [dbo].[Amenidades] ([Id])
GO
ALTER TABLE [dbo].[ReservacionAmenidades]  WITH CHECK ADD FOREIGN KEY([ReservacionHabitacionId], [HabitacionId])
REFERENCES [dbo].[ReservacionHabitaciones] ([Id], [HabitacionId])
GO
ALTER TABLE [dbo].[Reservaciones]  WITH CHECK ADD  CONSTRAINT [FK_Reservaciones_Clientes] FOREIGN KEY([ClienteId])
REFERENCES [dbo].[Clientes] ([Id])
GO
ALTER TABLE [dbo].[Reservaciones] CHECK CONSTRAINT [FK_Reservaciones_Clientes]
GO
ALTER TABLE [dbo].[Reservaciones]  WITH CHECK ADD  CONSTRAINT [FK_Reservaciones_Usuarios] FOREIGN KEY([CreadoPorUsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[Reservaciones] CHECK CONSTRAINT [FK_Reservaciones_Usuarios]
GO
ALTER TABLE [dbo].[ReservacionesMesa]  WITH CHECK ADD  CONSTRAINT [FK_ResMesa_Clientes] FOREIGN KEY([ClienteId])
REFERENCES [dbo].[Clientes] ([Id])
GO
ALTER TABLE [dbo].[ReservacionesMesa] CHECK CONSTRAINT [FK_ResMesa_Clientes]
GO
ALTER TABLE [dbo].[ReservacionesMesa]  WITH CHECK ADD  CONSTRAINT [FK_ResMesa_Mesas] FOREIGN KEY([MesaId])
REFERENCES [dbo].[Mesas] ([Id])
GO
ALTER TABLE [dbo].[ReservacionesMesa] CHECK CONSTRAINT [FK_ResMesa_Mesas]
GO
ALTER TABLE [dbo].[ReservacionHabitaciones]  WITH CHECK ADD  CONSTRAINT [FK_ResHab_Habitaciones] FOREIGN KEY([HabitacionId])
REFERENCES [dbo].[Habitaciones] ([Id])
GO
ALTER TABLE [dbo].[ReservacionHabitaciones] CHECK CONSTRAINT [FK_ResHab_Habitaciones]
GO
ALTER TABLE [dbo].[ReservacionHabitaciones]  WITH CHECK ADD  CONSTRAINT [FK_ResHab_Reservaciones] FOREIGN KEY([ReservacionId])
REFERENCES [dbo].[Reservaciones] ([Id])
GO
ALTER TABLE [dbo].[ReservacionHabitaciones] CHECK CONSTRAINT [FK_ResHab_Reservaciones]
GO
ALTER TABLE [dbo].[ReservacionNoches]  WITH CHECK ADD  CONSTRAINT [FK_ResNoches_ResHab] FOREIGN KEY([ReservacionHabitacionId], [HabitacionId])
REFERENCES [dbo].[ReservacionHabitaciones] ([Id], [HabitacionId])
GO
ALTER TABLE [dbo].[ReservacionNoches] CHECK CONSTRAINT [FK_ResNoches_ResHab]
GO
ALTER TABLE [dbo].[ReversosPago]  WITH CHECK ADD  CONSTRAINT [FK_Reversos_Pagos] FOREIGN KEY([PagoId])
REFERENCES [dbo].[Pagos] ([Id])
GO
ALTER TABLE [dbo].[ReversosPago] CHECK CONSTRAINT [FK_Reversos_Pagos]
GO
ALTER TABLE [dbo].[ReversosPago]  WITH CHECK ADD  CONSTRAINT [FK_Reversos_Usuarios] FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[ReversosPago] CHECK CONSTRAINT [FK_Reversos_Usuarios]
GO
ALTER TABLE [dbo].[RolPermisos]  WITH CHECK ADD  CONSTRAINT [FK_RolPermisos_Permisos] FOREIGN KEY([PermisoId])
REFERENCES [dbo].[Permisos] ([Id])
GO
ALTER TABLE [dbo].[RolPermisos] CHECK CONSTRAINT [FK_RolPermisos_Permisos]
GO
ALTER TABLE [dbo].[RolPermisos]  WITH CHECK ADD  CONSTRAINT [FK_RolPermisos_Roles] FOREIGN KEY([RolId])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[RolPermisos] CHECK CONSTRAINT [FK_RolPermisos_Roles]
GO
ALTER TABLE [dbo].[SolicitudesServicio]  WITH CHECK ADD  CONSTRAINT [FK_Solicitudes_Asignado] FOREIGN KEY([AsignadoAUsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[SolicitudesServicio] CHECK CONSTRAINT [FK_Solicitudes_Asignado]
GO
ALTER TABLE [dbo].[SolicitudesServicio]  WITH CHECK ADD  CONSTRAINT [FK_Solicitudes_Estadias] FOREIGN KEY([EstadiaId])
REFERENCES [dbo].[Estadias] ([Id])
GO
ALTER TABLE [dbo].[SolicitudesServicio] CHECK CONSTRAINT [FK_Solicitudes_Estadias]
GO
ALTER TABLE [dbo].[SolicitudesServicio]  WITH CHECK ADD  CONSTRAINT [FK_Solicitudes_Habitaciones] FOREIGN KEY([HabitacionId])
REFERENCES [dbo].[Habitaciones] ([Id])
GO
ALTER TABLE [dbo].[SolicitudesServicio] CHECK CONSTRAINT [FK_Solicitudes_Habitaciones]
GO
ALTER TABLE [dbo].[SolicitudesServicio]  WITH CHECK ADD  CONSTRAINT [FK_Solicitudes_Solicitante] FOREIGN KEY([SolicitadoPorUsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[SolicitudesServicio] CHECK CONSTRAINT [FK_Solicitudes_Solicitante]
GO
ALTER TABLE [dbo].[SolicitudesServicio]  WITH CHECK ADD  CONSTRAINT [FK_Solicitudes_Tipos] FOREIGN KEY([TipoSolicitudId])
REFERENCES [dbo].[TiposSolicitud] ([Id])
GO
ALTER TABLE [dbo].[SolicitudesServicio] CHECK CONSTRAINT [FK_Solicitudes_Tipos]
GO
ALTER TABLE [dbo].[UsuarioRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsuarioRoles_Roles] FOREIGN KEY([RolId])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[UsuarioRoles] CHECK CONSTRAINT [FK_UsuarioRoles_Roles]
GO
ALTER TABLE [dbo].[UsuarioRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsuarioRoles_Usuarios] FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[UsuarioRoles] CHECK CONSTRAINT [FK_UsuarioRoles_Usuarios]
GO
ALTER TABLE [dbo].[Usuarios]  WITH CHECK ADD  CONSTRAINT [FK_Usuarios_Personas] FOREIGN KEY([PersonaId])
REFERENCES [dbo].[Personas] ([Id])
GO
ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_Usuarios_Personas]
GO
ALTER TABLE [dbo].[AccesosEstadia]  WITH CHECK ADD  CONSTRAINT [CK_Accesos_Fechas] CHECK  (([FechaExpiracionUtc]>[FechaEmisionUtc]))
GO
ALTER TABLE [dbo].[AccesosEstadia] CHECK CONSTRAINT [CK_Accesos_Fechas]
GO
ALTER TABLE [dbo].[AccesosEstadia]  WITH CHECK ADD  CONSTRAINT [CK_Accesos_Tipo] CHECK  (([Tipo]=(2) OR [Tipo]=(1)))
GO
ALTER TABLE [dbo].[AccesosEstadia] CHECK CONSTRAINT [CK_Accesos_Tipo]
GO
ALTER TABLE [dbo].[Auditoria]  WITH CHECK ADD  CONSTRAINT [CK_Auditoria_Json] CHECK  (([DatosNoSensibles] IS NULL OR isjson([DatosNoSensibles])=(1)))
GO
ALTER TABLE [dbo].[Auditoria] CHECK CONSTRAINT [CK_Auditoria_Json]
GO
ALTER TABLE [dbo].[Auditoria]  WITH CHECK ADD  CONSTRAINT [CK_Auditoria_Resultado] CHECK  (([Resultado]>=(1) AND [Resultado]<=(3)))
GO
ALTER TABLE [dbo].[Auditoria] CHECK CONSTRAINT [CK_Auditoria_Resultado]
GO
ALTER TABLE [dbo].[BloqueosHabitacion]  WITH CHECK ADD  CONSTRAINT [CK_Bloqueos_Fechas] CHECK  (([FechaInicio]<[FechaFin]))
GO
ALTER TABLE [dbo].[BloqueosHabitacion] CHECK CONSTRAINT [CK_Bloqueos_Fechas]
GO
ALTER TABLE [dbo].[BloqueosHabitacion]  WITH CHECK ADD  CONSTRAINT [CK_Bloqueos_Tipo] CHECK  (([Tipo]=(2) OR [Tipo]=(1)))
GO
ALTER TABLE [dbo].[BloqueosHabitacion] CHECK CONSTRAINT [CK_Bloqueos_Tipo]
GO
ALTER TABLE [dbo].[CargosCuenta]  WITH CHECK ADD  CONSTRAINT [CK_Cargos_Ajuste] CHECK  (([Tipo]<>(5) OR [MotivoAjuste] IS NOT NULL AND [AutorizadoPorId] IS NOT NULL))
GO
ALTER TABLE [dbo].[CargosCuenta] CHECK CONSTRAINT [CK_Cargos_Ajuste]
GO
ALTER TABLE [dbo].[CargosCuenta]  WITH CHECK ADD  CONSTRAINT [CK_Cargos_Cantidad] CHECK  (([Cantidad]>(0)))
GO
ALTER TABLE [dbo].[CargosCuenta] CHECK CONSTRAINT [CK_Cargos_Cantidad]
GO
ALTER TABLE [dbo].[CargosCuenta]  WITH CHECK ADD  CONSTRAINT [CK_Cargos_Descuento] CHECK  (([Descuento]>=(0)))
GO
ALTER TABLE [dbo].[CargosCuenta] CHECK CONSTRAINT [CK_Cargos_Descuento]
GO
ALTER TABLE [dbo].[CargosCuenta]  WITH CHECK ADD  CONSTRAINT [CK_Cargos_Pedido] CHECK  (([Tipo]<>(2) OR [PedidoId] IS NOT NULL))
GO
ALTER TABLE [dbo].[CargosCuenta] CHECK CONSTRAINT [CK_Cargos_Pedido]
GO
ALTER TABLE [dbo].[CargosCuenta]  WITH CHECK ADD  CONSTRAINT [CK_Cargos_Penalizacion] CHECK  (([Tipo]=(6) AND [TipoPenalizacionId] IS NOT NULL AND [MotivoAjuste] IS NOT NULL OR [Tipo]<>(6) AND [TipoPenalizacionId] IS NULL))
GO
ALTER TABLE [dbo].[CargosCuenta] CHECK CONSTRAINT [CK_Cargos_Penalizacion]
GO
ALTER TABLE [dbo].[CargosCuenta]  WITH CHECK ADD  CONSTRAINT [CK_Cargos_Reverso] CHECK  (([CargoRevertidoId] IS NULL OR [MotivoAjuste] IS NOT NULL))
GO
ALTER TABLE [dbo].[CargosCuenta] CHECK CONSTRAINT [CK_Cargos_Reverso]
GO
ALTER TABLE [dbo].[CargosCuenta]  WITH CHECK ADD  CONSTRAINT [CK_Cargos_Tipo] CHECK  (([Tipo]>=(1) AND [Tipo]<=(7)))
GO
ALTER TABLE [dbo].[CargosCuenta] CHECK CONSTRAINT [CK_Cargos_Tipo]
GO
ALTER TABLE [dbo].[Clientes]  WITH CHECK ADD  CONSTRAINT [CK_Clientes_Descuento] CHECK  (([PorcentajeDescuento]>=(0) AND [PorcentajeDescuento]<=(100)))
GO
ALTER TABLE [dbo].[Clientes] CHECK CONSTRAINT [CK_Clientes_Descuento]
GO
ALTER TABLE [dbo].[Cuentas]  WITH CHECK ADD  CONSTRAINT [CK_Cuentas_Estado] CHECK  (([Estado]=(1) AND [FechaCierreUtc] IS NULL OR [Estado]=(2) AND [FechaCierreUtc] IS NOT NULL))
GO
ALTER TABLE [dbo].[Cuentas] CHECK CONSTRAINT [CK_Cuentas_Estado]
GO
ALTER TABLE [dbo].[Cuentas]  WITH CHECK ADD  CONSTRAINT [CK_Cuentas_Tipo] CHECK  (([Tipo]=(1) AND [ReservacionId] IS NOT NULL OR [Tipo]=(2) AND [ReservacionId] IS NULL))
GO
ALTER TABLE [dbo].[Cuentas] CHECK CONSTRAINT [CK_Cuentas_Tipo]
GO
ALTER TABLE [dbo].[Descuentos]  WITH CHECK ADD CHECK  (([TipoCalculo]=(1) AND [Valor]>(0) AND [Valor]<=(100) OR [TipoCalculo]=(2) AND [Valor]>(0)))
GO
ALTER TABLE [dbo].[Descuentos]  WITH CHECK ADD CHECK  (([VigenteHasta] IS NULL OR [VigenteHasta]>=[VigenteDesde]))
GO
ALTER TABLE [dbo].[DescuentosAplicados]  WITH CHECK ADD CHECK  (([Monto]>(0)))
GO
ALTER TABLE [dbo].[DescuentosAplicados]  WITH CHECK ADD CHECK  (([IndisponibilidadId] IS NULL AND [HabitacionId] IS NULL AND [AmenidadId] IS NULL OR [IndisponibilidadId] IS NOT NULL AND [HabitacionId] IS NOT NULL AND [AmenidadId] IS NOT NULL))
GO
ALTER TABLE [dbo].[DetalleFacturas]  WITH CHECK ADD  CONSTRAINT [CK_DetFac_Cantidad] CHECK  (([Cantidad]>(0)))
GO
ALTER TABLE [dbo].[DetalleFacturas] CHECK CONSTRAINT [CK_DetFac_Cantidad]
GO
ALTER TABLE [dbo].[DetallePedidos]  WITH CHECK ADD  CONSTRAINT [CK_DetPed_Cantidad] CHECK  (([Cantidad]>(0)))
GO
ALTER TABLE [dbo].[DetallePedidos] CHECK CONSTRAINT [CK_DetPed_Cantidad]
GO
ALTER TABLE [dbo].[DetallePedidos]  WITH CHECK ADD  CONSTRAINT [CK_DetPed_Montos] CHECK  (([PrecioUnitario]>(0) AND [Subtotal]>=(0) AND [Descuento]>=(0) AND [Impuestos]>=(0) AND [Total]>=(0)))
GO
ALTER TABLE [dbo].[DetallePedidos] CHECK CONSTRAINT [CK_DetPed_Montos]
GO
ALTER TABLE [dbo].[Edificios]  WITH CHECK ADD  CONSTRAINT [CK_Edificios_Codigo] CHECK  (([Codigo] like '[A-Z]'))
GO
ALTER TABLE [dbo].[Edificios] CHECK CONSTRAINT [CK_Edificios_Codigo]
GO
ALTER TABLE [dbo].[Edificios]  WITH CHECK ADD  CONSTRAINT [CK_Edificios_Pisos] CHECK  (([CantidadPisos]>=(1) AND [CantidadPisos]<=(6)))
GO
ALTER TABLE [dbo].[Edificios] CHECK CONSTRAINT [CK_Edificios_Pisos]
GO
ALTER TABLE [dbo].[Estadias]  WITH CHECK ADD  CONSTRAINT [CK_Estadias_Estado] CHECK  (([Estado]=(1) AND [CheckOutRealUtc] IS NULL AND [UsuarioCheckOutId] IS NULL OR [Estado]=(2) AND [CheckOutRealUtc] IS NOT NULL AND [UsuarioCheckOutId] IS NOT NULL))
GO
ALTER TABLE [dbo].[Estadias] CHECK CONSTRAINT [CK_Estadias_Estado]
GO
ALTER TABLE [dbo].[Estadias]  WITH CHECK ADD  CONSTRAINT [CK_Estadias_Fechas] CHECK  (([CheckOutRealUtc] IS NULL OR [CheckOutRealUtc]>[CheckInRealUtc]))
GO
ALTER TABLE [dbo].[Estadias] CHECK CONSTRAINT [CK_Estadias_Fechas]
GO
ALTER TABLE [dbo].[Facturas]  WITH CHECK ADD  CONSTRAINT [CK_Facturas_Estado] CHECK  (([Estado]=(2) OR [Estado]=(1)))
GO
ALTER TABLE [dbo].[Facturas] CHECK CONSTRAINT [CK_Facturas_Estado]
GO
ALTER TABLE [dbo].[Facturas]  WITH CHECK ADD  CONSTRAINT [CK_Facturas_Montos] CHECK  (([Subtotal]>=(0) AND [Descuento]>=(0) AND [Impuestos]>=(0) AND [Total]>=(0)))
GO
ALTER TABLE [dbo].[Facturas] CHECK CONSTRAINT [CK_Facturas_Montos]
GO
ALTER TABLE [dbo].[HabitacionAmenidades]  WITH CHECK ADD  CONSTRAINT [CK_HabitacionAmenidades_Cantidad] CHECK  (([Cantidad]>=(1)))
GO
ALTER TABLE [dbo].[HabitacionAmenidades] CHECK CONSTRAINT [CK_HabitacionAmenidades_Cantidad]
GO
ALTER TABLE [dbo].[Habitaciones]  WITH CHECK ADD  CONSTRAINT [CK_Habitaciones_Estado] CHECK  (([Estado]>=(1) AND [Estado]<=(7)))
GO
ALTER TABLE [dbo].[Habitaciones] CHECK CONSTRAINT [CK_Habitaciones_Estado]
GO
ALTER TABLE [dbo].[Habitaciones]  WITH CHECK ADD  CONSTRAINT [CK_Habitaciones_Piso] CHECK  (([Piso]>=(1) AND [Piso]<=(6)))
GO
ALTER TABLE [dbo].[Habitaciones] CHECK CONSTRAINT [CK_Habitaciones_Piso]
GO
ALTER TABLE [dbo].[HistorialEstadosHabitacion]  WITH CHECK ADD  CONSTRAINT [CK_HistEstHab_Estados] CHECK  (([EstadoAnterior]>=(1) AND [EstadoAnterior]<=(7) AND ([EstadoNuevo]>=(1) AND [EstadoNuevo]<=(7))))
GO
ALTER TABLE [dbo].[HistorialEstadosHabitacion] CHECK CONSTRAINT [CK_HistEstHab_Estados]
GO
ALTER TABLE [dbo].[HistorialPedidos]  WITH CHECK ADD  CONSTRAINT [CK_HistPed_Estados] CHECK  ((([EstadoAnterior] IS NULL OR [EstadoAnterior]>=(1) AND [EstadoAnterior]<=(7)) AND ([EstadoNuevo]>=(1) AND [EstadoNuevo]<=(7))))
GO
ALTER TABLE [dbo].[HistorialPedidos] CHECK CONSTRAINT [CK_HistPed_Estados]
GO
ALTER TABLE [dbo].[HistorialReservaciones]  WITH CHECK ADD  CONSTRAINT [CK_HistRes_Estados] CHECK  ((([EstadoAnterior] IS NULL OR [EstadoAnterior]>=(1) AND [EstadoAnterior]<=(5)) AND ([EstadoNuevo]>=(1) AND [EstadoNuevo]<=(5))))
GO
ALTER TABLE [dbo].[HistorialReservaciones] CHECK CONSTRAINT [CK_HistRes_Estados]
GO
ALTER TABLE [dbo].[HistorialSolicitudes]  WITH CHECK ADD  CONSTRAINT [CK_HistSol_Estados] CHECK  ((([EstadoAnterior] IS NULL OR [EstadoAnterior]>=(1) AND [EstadoAnterior]<=(5)) AND ([EstadoNuevo]>=(1) AND [EstadoNuevo]<=(5))))
GO
ALTER TABLE [dbo].[HistorialSolicitudes] CHECK CONSTRAINT [CK_HistSol_Estados]
GO
ALTER TABLE [dbo].[HuespedesReservacion]  WITH CHECK ADD  CONSTRAINT [CK_Huespedes_Documento] CHECK  (([TipoDocumentoId] IS NULL AND [NumeroDocumento] IS NULL OR [TipoDocumentoId] IS NOT NULL AND [NumeroDocumento] IS NOT NULL))
GO
ALTER TABLE [dbo].[HuespedesReservacion] CHECK CONSTRAINT [CK_Huespedes_Documento]
GO
ALTER TABLE [dbo].[Impuestos]  WITH CHECK ADD  CONSTRAINT [CK_Impuestos_Ambito] CHECK  (([Ambito]=(2) OR [Ambito]=(1)))
GO
ALTER TABLE [dbo].[Impuestos] CHECK CONSTRAINT [CK_Impuestos_Ambito]
GO
ALTER TABLE [dbo].[Impuestos]  WITH CHECK ADD  CONSTRAINT [CK_Impuestos_Porcentaje] CHECK  (([Porcentaje]>=(0) AND [Porcentaje]<=(100)))
GO
ALTER TABLE [dbo].[Impuestos] CHECK CONSTRAINT [CK_Impuestos_Porcentaje]
GO
ALTER TABLE [dbo].[Impuestos]  WITH CHECK ADD  CONSTRAINT [CK_Impuestos_Vigencia] CHECK  (([VigenteHasta] IS NULL OR [VigenteHasta]>=[VigenteDesde]))
GO
ALTER TABLE [dbo].[Impuestos] CHECK CONSTRAINT [CK_Impuestos_Vigencia]
GO
ALTER TABLE [dbo].[IndisponibilidadesAmenidad]  WITH CHECK ADD CHECK  (([FechaInicio]<[FechaFin]))
GO
ALTER TABLE [dbo].[Mesas]  WITH CHECK ADD  CONSTRAINT [CK_Mesas_Capacidad] CHECK  (([Capacidad]>=(1)))
GO
ALTER TABLE [dbo].[Mesas] CHECK CONSTRAINT [CK_Mesas_Capacidad]
GO
ALTER TABLE [dbo].[Mesas]  WITH CHECK ADD  CONSTRAINT [CK_Mesas_Ubicacion] CHECK  (([Ubicacion]>=(1) AND [Ubicacion]<=(3)))
GO
ALTER TABLE [dbo].[Mesas] CHECK CONSTRAINT [CK_Mesas_Ubicacion]
GO
ALTER TABLE [dbo].[NotificacionesCorreo]  WITH CHECK ADD  CONSTRAINT [CK_Correos_Estado] CHECK  (([Estado]>=(1) AND [Estado]<=(3)))
GO
ALTER TABLE [dbo].[NotificacionesCorreo] CHECK CONSTRAINT [CK_Correos_Estado]
GO
ALTER TABLE [dbo].[NotificacionesCorreo]  WITH CHECK ADD  CONSTRAINT [CK_Correos_Intentos] CHECK  (([Intentos]>=(0)))
GO
ALTER TABLE [dbo].[NotificacionesCorreo] CHECK CONSTRAINT [CK_Correos_Intentos]
GO
ALTER TABLE [dbo].[NotificacionesCorreo]  WITH CHECK ADD  CONSTRAINT [CK_Correos_Referencia] CHECK  (([ReservacionId] IS NOT NULL OR [FacturaId] IS NOT NULL))
GO
ALTER TABLE [dbo].[NotificacionesCorreo] CHECK CONSTRAINT [CK_Correos_Referencia]
GO
ALTER TABLE [dbo].[NotificacionesCorreo]  WITH CHECK ADD  CONSTRAINT [CK_Correos_Tipo] CHECK  (([Tipo]>=(1) AND [Tipo]<=(3)))
GO
ALTER TABLE [dbo].[NotificacionesCorreo] CHECK CONSTRAINT [CK_Correos_Tipo]
GO
ALTER TABLE [dbo].[Pagos]  WITH CHECK ADD  CONSTRAINT [CK_Pagos_Estado] CHECK  (([Estado]=(2) OR [Estado]=(1)))
GO
ALTER TABLE [dbo].[Pagos] CHECK CONSTRAINT [CK_Pagos_Estado]
GO
ALTER TABLE [dbo].[Pagos]  WITH CHECK ADD  CONSTRAINT [CK_Pagos_Metodo] CHECK  (([MetodoPago]>=(1) AND [MetodoPago]<=(3)))
GO
ALTER TABLE [dbo].[Pagos] CHECK CONSTRAINT [CK_Pagos_Metodo]
GO
ALTER TABLE [dbo].[Pagos]  WITH CHECK ADD  CONSTRAINT [CK_Pagos_Monto] CHECK  (([Monto]>(0)))
GO
ALTER TABLE [dbo].[Pagos] CHECK CONSTRAINT [CK_Pagos_Monto]
GO
ALTER TABLE [dbo].[Pagos]  WITH CHECK ADD  CONSTRAINT [CK_Pagos_Referencia] CHECK  (([MetodoPago]=(1) OR [Referencia] IS NOT NULL))
GO
ALTER TABLE [dbo].[Pagos] CHECK CONSTRAINT [CK_Pagos_Referencia]
GO
ALTER TABLE [dbo].[Pagos]  WITH CHECK ADD  CONSTRAINT [CK_Pagos_UltimosCuatro] CHECK  (([UltimosCuatro] IS NULL OR [MetodoPago]=(2) AND [UltimosCuatro] like '[0-9][0-9][0-9][0-9]'))
GO
ALTER TABLE [dbo].[Pagos] CHECK CONSTRAINT [CK_Pagos_UltimosCuatro]
GO
ALTER TABLE [dbo].[PedidosRestaurante]  WITH CHECK ADD  CONSTRAINT [CK_Pedidos_Estado] CHECK  (([EstadoPedido]>=(1) AND [EstadoPedido]<=(7)))
GO
ALTER TABLE [dbo].[PedidosRestaurante] CHECK CONSTRAINT [CK_Pedidos_Estado]
GO
ALTER TABLE [dbo].[PedidosRestaurante]  WITH CHECK ADD  CONSTRAINT [CK_Pedidos_Mesa] CHECK  (([Modalidad]<>(2) OR [ReservacionMesaId] IS NOT NULL))
GO
ALTER TABLE [dbo].[PedidosRestaurante] CHECK CONSTRAINT [CK_Pedidos_Mesa]
GO
ALTER TABLE [dbo].[PedidosRestaurante]  WITH CHECK ADD  CONSTRAINT [CK_Pedidos_Modalidad] CHECK  (([Modalidad]>=(1) AND [Modalidad]<=(3)))
GO
ALTER TABLE [dbo].[PedidosRestaurante] CHECK CONSTRAINT [CK_Pedidos_Modalidad]
GO
ALTER TABLE [dbo].[PedidosRestaurante]  WITH CHECK ADD  CONSTRAINT [CK_Pedidos_Montos] CHECK  (([Subtotal]>=(0) AND [Descuento]>=(0) AND [Impuestos]>=(0) AND [Recargos]>=(0) AND [Total]>=(0)))
GO
ALTER TABLE [dbo].[PedidosRestaurante] CHECK CONSTRAINT [CK_Pedidos_Montos]
GO
ALTER TABLE [dbo].[PedidosRestaurante]  WITH CHECK ADD  CONSTRAINT [CK_Pedidos_RoomService] CHECK  (([Modalidad]<>(1) OR [EstadiaId] IS NOT NULL))
GO
ALTER TABLE [dbo].[PedidosRestaurante] CHECK CONSTRAINT [CK_Pedidos_RoomService]
GO
ALTER TABLE [dbo].[Personas]  WITH CHECK ADD  CONSTRAINT [CK_Personas_Documento] CHECK  (([TipoDocumentoId] IS NULL AND [NumeroDocumento] IS NULL OR [TipoDocumentoId] IS NOT NULL AND [NumeroDocumento] IS NOT NULL AND len(ltrim(rtrim([NumeroDocumento])))>(0)))
GO
ALTER TABLE [dbo].[Personas] CHECK CONSTRAINT [CK_Personas_Documento]
GO
ALTER TABLE [dbo].[Platillos]  WITH CHECK ADD  CONSTRAINT [CK_Platillos_Precio] CHECK  (([Precio]>(0)))
GO
ALTER TABLE [dbo].[Platillos] CHECK CONSTRAINT [CK_Platillos_Precio]
GO
ALTER TABLE [dbo].[Platillos]  WITH CHECK ADD  CONSTRAINT [CK_Platillos_Tiempo] CHECK  (([TiempoPreparacionMinutos]>(0)))
GO
ALTER TABLE [dbo].[Platillos] CHECK CONSTRAINT [CK_Platillos_Tiempo]
GO
ALTER TABLE [dbo].[RefreshTokens]  WITH CHECK ADD  CONSTRAINT [CK_RefreshTokens_Expiracion] CHECK  (([FechaExpiracionUtc]>[FechaCreacionUtc]))
GO
ALTER TABLE [dbo].[RefreshTokens] CHECK CONSTRAINT [CK_RefreshTokens_Expiracion]
GO
ALTER TABLE [dbo].[ReglasTarifa]  WITH CHECK ADD  CONSTRAINT [CK_ReglasTarifa_DiaObligatorio] CHECK  (([Tipo]<>(2) OR [DiaSemana] IS NOT NULL))
GO
ALTER TABLE [dbo].[ReglasTarifa] CHECK CONSTRAINT [CK_ReglasTarifa_DiaObligatorio]
GO
ALTER TABLE [dbo].[ReglasTarifa]  WITH CHECK ADD  CONSTRAINT [CK_ReglasTarifa_Forma] CHECK  (([Tipo]=(1) AND [FechaInicio] IS NOT NULL AND [FechaFin] IS NOT NULL AND [FechaInicio]<=[FechaFin] AND [DiaSemana] IS NULL OR [Tipo]=(2) AND ([DiaSemana]>=(1) AND [DiaSemana]<=(7)) AND [FechaInicio] IS NULL AND [FechaFin] IS NULL))
GO
ALTER TABLE [dbo].[ReglasTarifa] CHECK CONSTRAINT [CK_ReglasTarifa_Forma]
GO
ALTER TABLE [dbo].[ReglasTarifa]  WITH CHECK ADD  CONSTRAINT [CK_ReglasTarifa_TipoAjuste] CHECK  (([TipoAjuste]=(2) OR [TipoAjuste]=(1)))
GO
ALTER TABLE [dbo].[ReglasTarifa] CHECK CONSTRAINT [CK_ReglasTarifa_TipoAjuste]
GO
ALTER TABLE [dbo].[ReservacionAmenidades]  WITH CHECK ADD CHECK  (([Cantidad]>(0)))
GO
ALTER TABLE [dbo].[Reservaciones]  WITH CHECK ADD  CONSTRAINT [CK_Reservaciones_Adultos] CHECK  (([CantidadAdultos]>=(1)))
GO
ALTER TABLE [dbo].[Reservaciones] CHECK CONSTRAINT [CK_Reservaciones_Adultos]
GO
ALTER TABLE [dbo].[Reservaciones]  WITH CHECK ADD  CONSTRAINT [CK_Reservaciones_Cancelacion] CHECK  (([Estado]<>(5) OR [MotivoCancelacion] IS NOT NULL))
GO
ALTER TABLE [dbo].[Reservaciones] CHECK CONSTRAINT [CK_Reservaciones_Cancelacion]
GO
ALTER TABLE [dbo].[Reservaciones]  WITH CHECK ADD  CONSTRAINT [CK_Reservaciones_Estado] CHECK  (([Estado]>=(1) AND [Estado]<=(5)))
GO
ALTER TABLE [dbo].[Reservaciones] CHECK CONSTRAINT [CK_Reservaciones_Estado]
GO
ALTER TABLE [dbo].[Reservaciones]  WITH CHECK ADD  CONSTRAINT [CK_Reservaciones_Expiracion] CHECK  (([Estado]<>(1) OR [FechaExpiracionPendienteUtc] IS NOT NULL))
GO
ALTER TABLE [dbo].[Reservaciones] CHECK CONSTRAINT [CK_Reservaciones_Expiracion]
GO
ALTER TABLE [dbo].[Reservaciones]  WITH CHECK ADD  CONSTRAINT [CK_Reservaciones_Fechas] CHECK  (([FechaInicio]<[FechaFin]))
GO
ALTER TABLE [dbo].[Reservaciones] CHECK CONSTRAINT [CK_Reservaciones_Fechas]
GO
ALTER TABLE [dbo].[Reservaciones]  WITH CHECK ADD  CONSTRAINT [CK_Reservaciones_Montos] CHECK  (([Subtotal]>=(0) AND [Descuento]>=(0) AND [Impuestos]>=(0) AND [Total]>=(0) AND [AnticipoRequerido]>=(0) AND [AnticipoRequerido]<=[Total]))
GO
ALTER TABLE [dbo].[Reservaciones] CHECK CONSTRAINT [CK_Reservaciones_Montos]
GO
ALTER TABLE [dbo].[ReservacionesMesa]  WITH CHECK ADD  CONSTRAINT [CK_ResMesa_Estado] CHECK  (([Estado]>=(1) AND [Estado]<=(4)))
GO
ALTER TABLE [dbo].[ReservacionesMesa] CHECK CONSTRAINT [CK_ResMesa_Estado]
GO
ALTER TABLE [dbo].[ReservacionesMesa]  WITH CHECK ADD  CONSTRAINT [CK_ResMesa_Fechas] CHECK  (([FechaInicioUtc]<[FechaFinUtc]))
GO
ALTER TABLE [dbo].[ReservacionesMesa] CHECK CONSTRAINT [CK_ResMesa_Fechas]
GO
ALTER TABLE [dbo].[ReservacionesMesa]  WITH CHECK ADD  CONSTRAINT [CK_ResMesa_Personas] CHECK  (([CantidadPersonas]>=(1)))
GO
ALTER TABLE [dbo].[ReservacionesMesa] CHECK CONSTRAINT [CK_ResMesa_Personas]
GO
ALTER TABLE [dbo].[ReservacionHabitaciones]  WITH CHECK ADD  CONSTRAINT [CK_ResHab_Adultos] CHECK  (([Adultos]>=(1)))
GO
ALTER TABLE [dbo].[ReservacionHabitaciones] CHECK CONSTRAINT [CK_ResHab_Adultos]
GO
ALTER TABLE [dbo].[ReservacionHabitaciones]  WITH CHECK ADD  CONSTRAINT [CK_ResHab_Fechas] CHECK  (([FechaInicio]<[FechaFin]))
GO
ALTER TABLE [dbo].[ReservacionHabitaciones] CHECK CONSTRAINT [CK_ResHab_Fechas]
GO
ALTER TABLE [dbo].[ReservacionHabitaciones]  WITH CHECK ADD  CONSTRAINT [CK_ResHab_Montos] CHECK  (([Subtotal]>=(0) AND [Descuento]>=(0) AND [Impuestos]>=(0) AND [Total]>=(0)))
GO
ALTER TABLE [dbo].[ReservacionHabitaciones] CHECK CONSTRAINT [CK_ResHab_Montos]
GO
ALTER TABLE [dbo].[ReservacionNoches]  WITH CHECK ADD  CONSTRAINT [CK_ResNoches_Tarifas] CHECK  (([TarifaBase]>=(0) AND [TarifaFinal]>=(0)))
GO
ALTER TABLE [dbo].[ReservacionNoches] CHECK CONSTRAINT [CK_ResNoches_Tarifas]
GO
ALTER TABLE [dbo].[ReversosPago]  WITH CHECK ADD  CONSTRAINT [CK_Reversos_Monto] CHECK  (([Monto]>(0)))
GO
ALTER TABLE [dbo].[ReversosPago] CHECK CONSTRAINT [CK_Reversos_Monto]
GO
ALTER TABLE [dbo].[SolicitudesServicio]  WITH CHECK ADD  CONSTRAINT [CK_Solicitudes_Asignacion] CHECK  ((NOT ([Estado]=(4) OR [Estado]=(3) OR [Estado]=(2)) OR [AsignadoAUsuarioId] IS NOT NULL AND [FechaAsignacionUtc] IS NOT NULL))
GO
ALTER TABLE [dbo].[SolicitudesServicio] CHECK CONSTRAINT [CK_Solicitudes_Asignacion]
GO
ALTER TABLE [dbo].[SolicitudesServicio]  WITH CHECK ADD  CONSTRAINT [CK_Solicitudes_Cancelada] CHECK  (([Estado]<>(5) OR [ComentarioCierre] IS NOT NULL))
GO
ALTER TABLE [dbo].[SolicitudesServicio] CHECK CONSTRAINT [CK_Solicitudes_Cancelada]
GO
ALTER TABLE [dbo].[SolicitudesServicio]  WITH CHECK ADD  CONSTRAINT [CK_Solicitudes_Completada] CHECK  (([Estado]=(4) AND [FechaCompletadaUtc] IS NOT NULL OR [Estado]<>(4) AND [FechaCompletadaUtc] IS NULL))
GO
ALTER TABLE [dbo].[SolicitudesServicio] CHECK CONSTRAINT [CK_Solicitudes_Completada]
GO
ALTER TABLE [dbo].[SolicitudesServicio]  WITH CHECK ADD  CONSTRAINT [CK_Solicitudes_Estado] CHECK  (([Estado]>=(1) AND [Estado]<=(5)))
GO
ALTER TABLE [dbo].[SolicitudesServicio] CHECK CONSTRAINT [CK_Solicitudes_Estado]
GO
ALTER TABLE [dbo].[SolicitudesServicio]  WITH CHECK ADD  CONSTRAINT [CK_Solicitudes_Fechas] CHECK  ((([FechaAsignacionUtc] IS NULL OR [FechaAsignacionUtc]>=[FechaSolicitudUtc]) AND ([FechaInicioUtc] IS NULL OR [FechaAsignacionUtc] IS NULL OR [FechaInicioUtc]>=[FechaAsignacionUtc]) AND ([FechaCompletadaUtc] IS NULL OR [FechaInicioUtc] IS NULL OR [FechaCompletadaUtc]>=[FechaInicioUtc])))
GO
ALTER TABLE [dbo].[SolicitudesServicio] CHECK CONSTRAINT [CK_Solicitudes_Fechas]
GO
ALTER TABLE [dbo].[SolicitudesServicio]  WITH CHECK ADD  CONSTRAINT [CK_Solicitudes_Inicio] CHECK  ((NOT ([Estado]=(4) OR [Estado]=(3)) OR [FechaInicioUtc] IS NOT NULL))
GO
ALTER TABLE [dbo].[SolicitudesServicio] CHECK CONSTRAINT [CK_Solicitudes_Inicio]
GO
ALTER TABLE [dbo].[SolicitudesServicio]  WITH CHECK ADD  CONSTRAINT [CK_Solicitudes_Prioridad] CHECK  (([Prioridad]>=(1) AND [Prioridad]<=(3)))
GO
ALTER TABLE [dbo].[SolicitudesServicio] CHECK CONSTRAINT [CK_Solicitudes_Prioridad]
GO
ALTER TABLE [dbo].[TiposHabitacion]  WITH CHECK ADD  CONSTRAINT [CK_TiposHabitacion_Adultos] CHECK  (([CapacidadAdultos]>=(1)))
GO
ALTER TABLE [dbo].[TiposHabitacion] CHECK CONSTRAINT [CK_TiposHabitacion_Adultos]
GO
ALTER TABLE [dbo].[TiposHabitacion]  WITH CHECK ADD  CONSTRAINT [CK_TiposHabitacion_Maxima] CHECK  (([CapacidadMaxima]<=(CONVERT([int],[CapacidadAdultos])+[CapacidadNinos])))
GO
ALTER TABLE [dbo].[TiposHabitacion] CHECK CONSTRAINT [CK_TiposHabitacion_Maxima]
GO
ALTER TABLE [dbo].[TiposHabitacion]  WITH CHECK ADD  CONSTRAINT [CK_TiposHabitacion_Ocupacion] CHECK  (([OcupacionBase]>=(1) AND [OcupacionBase]<=[CapacidadMaxima]))
GO
ALTER TABLE [dbo].[TiposHabitacion] CHECK CONSTRAINT [CK_TiposHabitacion_Ocupacion]
GO
ALTER TABLE [dbo].[TiposHabitacion]  WITH CHECK ADD  CONSTRAINT [CK_TiposHabitacion_Precio] CHECK  (([PrecioBasePorNoche]>(0) AND [PrecioPersonaAdicional]>=(0)))
GO
ALTER TABLE [dbo].[TiposHabitacion] CHECK CONSTRAINT [CK_TiposHabitacion_Precio]
GO
ALTER TABLE [dbo].[TiposPenalizacion]  WITH CHECK ADD CHECK  (([MontoReferencia]>=(0)))
GO
ALTER TABLE [dbo].[TiposSolicitud]  WITH CHECK ADD  CONSTRAINT [CK_TiposSolicitud_Area] CHECK  (([Area]>=(1) AND [Area]<=(3)))
GO
ALTER TABLE [dbo].[TiposSolicitud] CHECK CONSTRAINT [CK_TiposSolicitud_Area]
GO
ALTER TABLE [dbo].[TiposSolicitud]  WITH CHECK ADD  CONSTRAINT [CK_TiposSolicitud_Tiempo] CHECK  (([TiempoObjetivoMinutos]>(0)))
GO
ALTER TABLE [dbo].[TiposSolicitud] CHECK CONSTRAINT [CK_TiposSolicitud_Tiempo]
GO
ALTER TABLE [dbo].[Usuarios]  WITH CHECK ADD  CONSTRAINT [CK_Usuarios_Intentos] CHECK  (([IntentosFallidos]>=(0)))
GO
ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [CK_Usuarios_Intentos]
GO
USE [master]
GO
ALTER DATABASE [HotelDB] SET  READ_WRITE 
GO
