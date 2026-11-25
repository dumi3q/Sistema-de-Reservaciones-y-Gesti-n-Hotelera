namespace SistemaHotelero.Dominio.Enumeraciones;

// Códigos del módulo económico y de correos (compañero 3) que hospedaje necesita leer o escribir.
// Si el compañero 3 define estos mismos enums, se dejan los suyos y se borra este archivo.

public enum TipoCuenta : byte
{
    Hospedaje = 1,
    Consumo = 2
}

public enum EstadoCuenta : byte
{
    Abierta = 1,
    Cerrada = 2
}

public enum TipoCargo : byte
{
    Hospedaje = 1,
    Pedido = 2,
    LateCheckOut = 3,
    Servicio = 4,
    Ajuste = 5,
    Penalizacion = 6
}

public enum EstadoPago : byte
{
    Aplicado = 1,
    Revertido = 2
}

public enum AmbitoImpuesto : byte
{
    Hospedaje = 1,
    Restaurante = 2
}

public enum TipoNotificacionCorreo : byte
{
    ReservaConfirmada = 1,
    FacturaPagada = 2,
    Cancelacion = 3
}

public enum EstadoCorreo : byte
{
    Pendiente = 1,
    Enviado = 2,
    Fallido = 3
}
