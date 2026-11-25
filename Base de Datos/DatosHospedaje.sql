/* =====================================================================
   Datos base del módulo de hospedaje (compañero 2) para HotelDB
   Se puede ejecutar varias veces: solo inserta lo que falta.
   Ejecutar DESPUÉS de scriptsCorregidos.sql
   ===================================================================== */

USE HotelDB;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

/* ---------- Tipos de documento ---------- */
INSERT INTO dbo.TiposDocumento (Id, Nombre)
SELECT v.Id, v.Nombre
FROM (VALUES (CAST(1 AS tinyint), N'Cédula'), (2, N'DNI'), (3, N'Pasaporte')) AS v (Id, Nombre)
WHERE NOT EXISTS (SELECT 1 FROM dbo.TiposDocumento t WHERE t.Id = v.Id);

/* ---------- Configuración general ---------- */
INSERT INTO dbo.Configuracion (Clave, Valor, Descripcion)
SELECT v.Clave, v.Valor, v.Descripcion
FROM (VALUES
    (N'HoraCheckIn',              N'15:00',              N'Hora local de entrada'),
    (N'HoraCheckOut',             N'12:00',              N'Hora local de salida'),
    (N'RecargoLateCheckOutPorHora', N'15.00',            N'Monto por hora tarde'),
    (N'PrefijoReserva',           N'HOT',                N'Prefijo del código de reserva'),
    (N'HorasExpiracionPendiente', N'24',                 N'Horas antes de cancelar una reserva sin anticipo'),
    (N'AnticipoTipo',             N'Porcentaje',         N'Porcentaje o PrimeraNoche'),
    (N'AnticipoValor',            N'30',                 N'Porcentaje del total si AnticipoTipo = Porcentaje'),
    (N'ZonaHoraria',              N'America/Costa_Rica', N'Zona horaria del hotel'),
    (N'Moneda',                   N'USD',                N'Moneda de precios y pagos')
) AS v (Clave, Valor, Descripcion)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Configuracion c WHERE c.Clave = v.Clave);

/* ---------- Impuesto de hospedaje ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Impuestos WHERE Nombre = N'IVA hospedaje')
    INSERT INTO dbo.Impuestos (Nombre, Ambito, Porcentaje, VigenteDesde, Activo)
    VALUES (N'IVA hospedaje', 1, 13.00, '2026-01-01', 1);

/* ---------- Tipos de habitación ---------- */
INSERT INTO dbo.TiposHabitacion
    (Nombre, Descripcion, CapacidadAdultos, CapacidadNinos, CapacidadMaxima, OcupacionBase,
     PrecioBasePorNoche, PrecioPersonaAdicional, Activo)
SELECT v.*, 1
FROM (VALUES
    (N'Simple',             N'Una cama individual, ideal para una persona',      CAST(1 AS tinyint), CAST(0 AS tinyint), CAST(1 AS tinyint), CAST(1 AS tinyint), CAST( 60.00 AS decimal(18,2)), CAST( 0.00 AS decimal(18,2))),
    (N'Doble Estándar',     N'Dos camas matrimoniales',                          2, 2, 4, 2,  90.00, 15.00),
    (N'Suite Ejecutiva',    N'Cama king, sala de estar y escritorio',            2, 1, 3, 2, 160.00, 25.00),
    (N'Suite Presidencial', N'Cama king, jacuzzi, sala, comedor y vista al mar', 4, 2, 6, 2, 350.00, 40.00),
    (N'Familiar',           N'Dos ambientes, una cama king y dos individuales',  4, 3, 6, 4, 140.00, 20.00)
) AS v (Nombre, Descripcion, CapacidadAdultos, CapacidadNinos, CapacidadMaxima, OcupacionBase, PrecioBasePorNoche, PrecioPersonaAdicional)
WHERE NOT EXISTS (SELECT 1 FROM dbo.TiposHabitacion t WHERE t.Nombre = v.Nombre);

/* ---------- Amenidades ---------- */
INSERT INTO dbo.Amenidades (Nombre, Descripcion, Activo)
SELECT v.Nombre, NULL, 1
FROM (VALUES (N'Jacuzzi'), (N'Vista al mar'), (N'Balcón'), (N'Cama king'),
             (N'Aire acondicionado'), (N'Minibar'), (N'Caja fuerte'), (N'Wi-Fi')) AS v (Nombre)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Amenidades a WHERE a.Nombre = v.Nombre);

/* ---------- Temporadas y fines de semana (DiaSemana ISO: 5 viernes, 6 sábado) ---------- */
INSERT INTO dbo.ReglasTarifa (TipoHabitacionId, Nombre, Tipo, FechaInicio, FechaFin, DiaSemana, TipoAjuste, Valor, Prioridad, Activo)
SELECT NULL, v.Nombre, v.Tipo, v.FechaInicio, v.FechaFin, v.DiaSemana, 1, v.Valor, v.Prioridad, 1
FROM (VALUES
    (N'Fin de semana - viernes', CAST(2 AS tinyint), CAST(NULL AS date), CAST(NULL AS date), CAST(5 AS tinyint), CAST(15.00 AS decimal(18,2)), 1),
    (N'Fin de semana - sábado',  2, NULL, NULL, 6, 15.00, 1),
    (N'Navidad y Año Nuevo',     1, '2026-12-20', '2027-01-05', NULL, 30.00, 2),
    (N'Semana Santa 2027',       1, '2027-03-21', '2027-03-28', NULL, 25.00, 2),
    (N'Verano 2027',             1, '2027-01-06', '2027-04-30', NULL, 10.00, 1)
) AS v (Nombre, Tipo, FechaInicio, FechaFin, DiaSemana, Valor, Prioridad)
WHERE NOT EXISTS (SELECT 1 FROM dbo.ReglasTarifa r WHERE r.Nombre = v.Nombre);

/* ---------- 4 edificios de 6 pisos ---------- */
INSERT INTO dbo.Edificios (Codigo, Nombre, CantidadPisos, Activo)
SELECT v.Codigo, v.Nombre, 6, 1
FROM (VALUES ('A', N'Edificio A'), ('B', N'Edificio B'), ('C', N'Edificio C'), ('D', N'Edificio D')) AS v (Codigo, Nombre)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Edificios e WHERE e.Codigo = v.Codigo);

/* ---------- 96 habitaciones: 4 por piso en cada edificio ----------
   Número = piso * 100 + posición. Posición 1 Simple, 2 Doble Estándar, 3 Familiar,
   4 Suite Ejecutiva (en el piso 6 la posición 4 es Suite Presidencial). Estado 1 = Disponible */
INSERT INTO dbo.Habitaciones (EdificioId, TipoHabitacionId, Numero, Piso, Estado)
SELECT e.Id, t.Id, CAST(p.Piso * 100 + n.Posicion AS nvarchar(10)), p.Piso, 1
FROM dbo.Edificios e
CROSS JOIN (VALUES (1), (2), (3), (4), (5), (6)) AS p (Piso)
CROSS JOIN (VALUES (1), (2), (3), (4)) AS n (Posicion)
JOIN dbo.TiposHabitacion t ON t.Nombre =
     CASE
         WHEN n.Posicion = 1 THEN N'Simple'
         WHEN n.Posicion = 2 THEN N'Doble Estándar'
         WHEN n.Posicion = 3 THEN N'Familiar'
         WHEN p.Piso = 6     THEN N'Suite Presidencial'
         ELSE N'Suite Ejecutiva'
     END
WHERE e.Codigo IN ('A', 'B', 'C', 'D')
  AND NOT EXISTS (SELECT 1 FROM dbo.Habitaciones h
                  WHERE h.EdificioId = e.Id AND h.Numero = CAST(p.Piso * 100 + n.Posicion AS nvarchar(10)));

/* ---------- Amenidades por tipo de habitación ---------- */
INSERT INTO dbo.HabitacionAmenidades (HabitacionId, AmenidadId, Cantidad)
SELECT h.Id, a.Id, 1
FROM dbo.Habitaciones h
JOIN dbo.TiposHabitacion t ON t.Id = h.TipoHabitacionId
JOIN dbo.Amenidades a ON
     a.Nombre IN (N'Aire acondicionado', N'Wi-Fi', N'Caja fuerte')
  OR (t.Nombre IN (N'Suite Ejecutiva', N'Suite Presidencial') AND a.Nombre IN (N'Cama king', N'Minibar', N'Balcón'))
  OR (t.Nombre = N'Suite Presidencial' AND a.Nombre IN (N'Jacuzzi', N'Vista al mar'))
  OR (t.Nombre = N'Familiar' AND a.Nombre IN (N'Cama king', N'Balcón'))
WHERE NOT EXISTS (SELECT 1 FROM dbo.HabitacionAmenidades x WHERE x.HabitacionId = h.Id AND x.AmenidadId = a.Id);

/* ---------- Usuario de prueba (solo desarrollo) ----------
   Lo usa la API mientras no exista el login (Desarrollo:UsuarioPruebaId en appsettings.Development.json).
   PasswordHash es un texto de relleno: NO sirve para iniciar sesión. El compañero 3 crea los usuarios reales. */
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE NombreUsuario = N'pruebas')
BEGIN
    DECLARE @PersonaId int;

    INSERT INTO dbo.Personas (TipoDocumentoId, NumeroDocumento, Nombres, Apellidos, EmailContacto)
    VALUES (NULL, NULL, N'Usuario', N'Pruebas', N'pruebas@hotel.local');
    SET @PersonaId = SCOPE_IDENTITY();

    INSERT INTO dbo.Usuarios (PersonaId, NombreUsuario, Email, PasswordHash, Activo, IntentosFallidos, FechaCreacionUtc)
    VALUES (@PersonaId, N'pruebas', N'pruebas@hotel.local', N'SIN-LOGIN', 1, 0, SYSUTCDATETIME());
END

COMMIT TRANSACTION;

SELECT (SELECT Id FROM dbo.Usuarios WHERE NombreUsuario = N'pruebas') AS UsuarioPruebaId,
       (SELECT COUNT(*) FROM dbo.Edificios)    AS Edificios,
       (SELECT COUNT(*) FROM dbo.Habitaciones) AS Habitaciones,
       (SELECT COUNT(*) FROM dbo.TiposHabitacion) AS TiposHabitacion;
GO
