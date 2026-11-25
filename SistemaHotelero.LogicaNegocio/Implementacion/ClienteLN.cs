using AutoMapper;
using Microsoft.Extensions.Logging;
using SistemaHotelero.Dominio.Entidades;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.InterfazAD;
using SistemaHotelero.Dominio.InterfazLN;
using SistemaHotelero.Utilitarios;
using System.Net.Mail;

namespace SistemaHotelero.LogicaNegocio.Implementacion
{
    public class ClienteLN : BaseLN<ClienteLN>, IClienteLN
    {
        private static readonly List<string> IncludesCliente = new() { "Persona", "Persona.TipoDocumento" };

        public ClienteLN(
            IUnidadTrabajoEF unidadTrabajo,
            ILogger<ClienteLN> logger,
            IMapper mapper)
            : base(unidadTrabajo, mapper, logger)
        {
        }

        public async Task<Respuesta<IEnumerable<TCliente>>> ListarAsync(string? texto, bool? activo)
        {
            try
            {
                var t = (texto ?? string.Empty).Trim();

                var respuesta = await _unidadDeTrabajo.TCliente.BuscarAsync(
                    x => (activo == null || x.Activo == activo)
                      && (t == ""
                          || x.Persona.Nombres.Contains(t)
                          || x.Persona.Apellidos.Contains(t)
                          || (x.Persona.NumeroDocumento != null && x.Persona.NumeroDocumento.Contains(t))
                          || (x.Persona.EmailContacto != null && x.Persona.EmailContacto.Contains(t))),
                    IncludesCliente);

                if (!respuesta.Success)
                    return Falla<IEnumerable<TCliente>, IEnumerable<Cliente>>(respuesta);

                var datos = _mapper.Map<IEnumerable<TCliente>>(respuesta.Data)
                    .OrderBy(x => x.Apellidos)
                    .ThenBy(x => x.Nombres);

                return Respuesta<IEnumerable<TCliente>>.Ok(datos);
            }
            catch (Exception ex)
            {
                return Excepcion<IEnumerable<TCliente>>(ex, "Error al listar clientes.");
            }
        }

        public async Task<Respuesta<TCliente>> ObtenerAsync(TCliente datos)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(
                    x => x.Id == datos.Id, IncludesCliente);

                if (!respuesta.Success)
                    return Falla<TCliente, Cliente>(respuesta);

                if (respuesta.Data == null)
                    return Falla<TCliente>("Cliente no encontrado.", CodigosRespuesta.NoEncontrado);

                return Respuesta<TCliente>.Ok(_mapper.Map<TCliente>(respuesta.Data));
            }
            catch (Exception ex)
            {
                return Excepcion<TCliente>(ex, $"Error al obtener el cliente {datos.Id}.");
            }
        }

        public async Task<Respuesta<TCliente>> CrearAsync(TCliente datos)
        {
            try
            {
                Normalizar(datos);

                var validacion = await ValidarAsync(datos, personaIdActual: null);
                if (!validacion.Success)
                    return validacion;

                var persona = new Persona();
                CopiarPersona(datos, persona);

                _unidadDeTrabajo.EmpezarTransaccion();

                var personaCreada = await _unidadDeTrabajo.TPersona.InsertarAsync(persona);
                if (!personaCreada.Success)
                {
                    _unidadDeTrabajo.Rollback();
                    return Falla<TCliente, Persona>(personaCreada);
                }

                var cliente = new Cliente
                {
                    PersonaId = persona.Id,
                    Preferencias = datos.Preferencias,
                    PorcentajeDescuento = datos.PorcentajeDescuento,
                    FechaRegistroUtc = DateTime.UtcNow,
                    Activo = true
                };

                var clienteCreado = await _unidadDeTrabajo.TCliente.InsertarAsync(cliente);
                if (!clienteCreado.Success)
                {
                    _unidadDeTrabajo.Rollback();
                    return Falla<TCliente, Cliente>(clienteCreado);
                }

                _unidadDeTrabajo.CompletarTran();

                return await ObtenerAsync(new TCliente { Id = cliente.Id });
            }
            catch (Exception ex)
            {
                _unidadDeTrabajo.Rollback();
                return Excepcion<TCliente>(ex, "Error al crear el cliente.");
            }
        }

        public async Task<Respuesta<TCliente>> ModificarAsync(TCliente datos)
        {
            try
            {
                var actual = await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(
                    x => x.Id == datos.Id, new List<string> { "Persona" });

                if (!actual.Success)
                    return Falla<TCliente, Cliente>(actual);

                if (actual.Data == null)
                    return Falla<TCliente>("Cliente no encontrado.", CodigosRespuesta.NoEncontrado);

                Normalizar(datos);

                var cliente = actual.Data;
                var validacion = await ValidarAsync(datos, cliente.PersonaId);
                if (!validacion.Success)
                    return validacion;

                CopiarPersona(datos, cliente.Persona);
                cliente.Preferencias = datos.Preferencias;
                cliente.PorcentajeDescuento = datos.PorcentajeDescuento;

                // Cliente y Persona están en el contexto: un solo SaveChanges guarda ambos
                var guardado = await _unidadDeTrabajo.TCliente.ModificarAsync(cliente);
                if (!guardado.Success)
                    return Falla<TCliente, Cliente>(guardado);

                return await ObtenerAsync(new TCliente { Id = cliente.Id });
            }
            catch (Exception ex)
            {
                return Excepcion<TCliente>(ex, $"Error al modificar el cliente {datos.Id}.");
            }
        }

        // Borrado lógico: un cliente con reservas no se elimina, se desactiva
        public async Task<Respuesta<TCliente>> CambiarActivoAsync(TCliente datos)
        {
            try
            {
                var actual = await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(x => x.Id == datos.Id);

                if (!actual.Success)
                    return Falla<TCliente, Cliente>(actual);

                if (actual.Data == null)
                    return Falla<TCliente>("Cliente no encontrado.", CodigosRespuesta.NoEncontrado);

                actual.Data.Activo = datos.Activo;

                var guardado = await _unidadDeTrabajo.TCliente.ModificarAsync(actual.Data);
                if (!guardado.Success)
                    return Falla<TCliente, Cliente>(guardado);

                return await ObtenerAsync(new TCliente { Id = datos.Id });
            }
            catch (Exception ex)
            {
                return Excepcion<TCliente>(ex, $"Error al cambiar el estado del cliente {datos.Id}.");
            }
        }

        #region Validaciones

        private async Task<Respuesta<TCliente>> ValidarAsync(TCliente datos, int? personaIdActual)
        {
            if (string.IsNullOrWhiteSpace(datos.Nombres) || datos.Nombres.Length > 100)
                return Falla<TCliente>("Los nombres son obligatorios (máximo 100 caracteres).");

            if (string.IsNullOrWhiteSpace(datos.Apellidos) || datos.Apellidos.Length > 100)
                return Falla<TCliente>("Los apellidos son obligatorios (máximo 100 caracteres).");

            if ((datos.TipoDocumentoId == null) != (datos.NumeroDocumento == null))
                return Falla<TCliente>("Indique el tipo y el número de documento, o ninguno de los dos.");

            if (datos.NumeroDocumento != null && datos.NumeroDocumento.Length > 30)
                return Falla<TCliente>("El número de documento admite máximo 30 caracteres.");

            if (datos.EmailContacto != null
                && (datos.EmailContacto.Length > 150 || !MailAddress.TryCreate(datos.EmailContacto, out _)))
                return Falla<TCliente>("El correo no es válido.");

            if (datos.FechaNacimiento != null && datos.FechaNacimiento > DateOnly.FromDateTime(DateTime.Today))
                return Falla<TCliente>("La fecha de nacimiento no puede ser futura.");

            if (datos.PorcentajeDescuento < 0 || datos.PorcentajeDescuento > 100)
                return Falla<TCliente>("El descuento debe estar entre 0 y 100.");

            if (datos.TipoDocumentoId != null)
            {
                var tipo = await _unidadDeTrabajo.TTipoDocumento.ContarAsync(x => x.Id == datos.TipoDocumentoId);
                if (!tipo.Success)
                    return Falla<TCliente, int?>(tipo);
                if (tipo.Data == 0)
                    return Falla<TCliente>("El tipo de documento no existe.");

                var duplicado = await _unidadDeTrabajo.TPersona.ContarAsync(
                    x => x.TipoDocumentoId == datos.TipoDocumentoId
                      && x.NumeroDocumento == datos.NumeroDocumento
                      && (personaIdActual == null || x.Id != personaIdActual));
                if (!duplicado.Success)
                    return Falla<TCliente, int?>(duplicado);
                if (duplicado.Data > 0)
                    return Falla<TCliente>("Ya existe una persona con ese documento.", CodigosRespuesta.Conflicto);
            }

            return Respuesta<TCliente>.Ok(datos);
        }

        // Quita espacios y convierte textos vacíos en null
        private static void Normalizar(TCliente datos)
        {
            static string? Limpiar(string? valor) =>
                string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

            datos.Nombres = datos.Nombres?.Trim() ?? string.Empty;
            datos.Apellidos = datos.Apellidos?.Trim() ?? string.Empty;
            datos.NumeroDocumento = Limpiar(datos.NumeroDocumento);
            datos.Nacionalidad = Limpiar(datos.Nacionalidad);
            datos.EmailContacto = Limpiar(datos.EmailContacto);
            datos.Telefono = Limpiar(datos.Telefono);
            datos.Preferencias = Limpiar(datos.Preferencias);
        }

        private static void CopiarPersona(TCliente origen, Persona destino)
        {
            destino.TipoDocumentoId = origen.TipoDocumentoId;
            destino.NumeroDocumento = origen.NumeroDocumento;
            destino.Nombres = origen.Nombres;
            destino.Apellidos = origen.Apellidos;
            destino.Nacionalidad = origen.Nacionalidad;
            destino.FechaNacimiento = origen.FechaNacimiento;
            destino.EmailContacto = origen.EmailContacto;
            destino.Telefono = origen.Telefono;
        }

        #endregion
    }
}
