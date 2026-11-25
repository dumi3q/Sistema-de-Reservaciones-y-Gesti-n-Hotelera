namespace SistemaHotelero.Dominio.Enumeraciones;

// Códigos guardados como tinyint en HotelDB. No cambiar los números.

public enum EstadoHabitacion : byte
{
    Disponible = 1,
    Reservada = 2,
    Ocupada = 3,
    EnLimpieza = 4,
    EnMantenimiento = 5,
    Bloqueada = 6,
    FueraDeServicio = 7 // reemplaza al antiguo Activo = 0
}

public enum EstadoReservacion : byte
{
    PendientePago = 1,
    Confirmada = 2,
    EnCurso = 3,
    Finalizada = 4,
    Cancelada = 5
}

public enum EstadoEstadia : byte
{
    Activa = 1,
    Finalizada = 2
}

public enum TipoBloqueo : byte
{
    Mantenimiento = 1,
    Bloqueo = 2
}

public enum TipoAccesoEstadia : byte
{
    TarjetaMagnetica = 1,
    CodigoDigital = 2
}

public enum TipoReglaTarifa : byte
{
    Temporada = 1,
    FinDeSemana = 2
}

public enum TipoAjusteTarifa : byte
{
    Porcentaje = 1,
    MontoFijo = 2
}

public enum EstadoSolicitud : byte
{
    Pendiente = 1,
    Asignada = 2,
    EnProceso = 3,
    Completada = 4,
    Cancelada = 5
}

public enum PrioridadSolicitud : byte
{
    Baja = 1,
    Media = 2,
    Alta = 3
}
