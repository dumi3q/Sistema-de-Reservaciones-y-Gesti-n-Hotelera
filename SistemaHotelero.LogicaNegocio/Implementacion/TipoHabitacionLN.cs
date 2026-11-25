using AutoMapper;
using Microsoft.Extensions.Logging;
using SistemaHotelero.Dominio.Entidades;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.InterfazAD;
using SistemaHotelero.Dominio.InterfazLN;
using SistemaHotelero.Utilitarios;

namespace SistemaHotelero.LogicaNegocio.Implementacion
{
    public class TipoHabitacionLN : BaseLN<TipoHabitacionLN>, ITipoHabitacionLN
    {
        public TipoHabitacionLN(
            IUnidadTrabajoEF unidadTrabajo,
            ILogger<TipoHabitacionLN> logger,
            IMapper mapper)
            : base(unidadTrabajo, mapper, logger)
        {
        }

        public async Task<Respuesta<IEnumerable<TTipoHabitacion>>> ListarAsync(bool soloActivos)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.TTipoHabitacion.BuscarAsync(
                    x => !soloActivos || x.Activo);
                if (!respuesta.Success)
                    return Falla<IEnumerable<TTipoHabitacion>, IEnumerable<TiposHabitacion>>(respuesta);

                return Respuesta<IEnumerable<TTipoHabitacion>>.Ok(
                    _mapper.Map<IEnumerable<TTipoHabitacion>>(respuesta.Data).OrderBy(x => x.PrecioBasePorNoche));
            }
            catch (Exception ex)
            {
                return Excepcion<IEnumerable<TTipoHabitacion>>(ex, "Error al listar tipos de habitación.");
            }
        }

        public async Task<Respuesta<TTipoHabitacion>> ObtenerAsync(TTipoHabitacion datos)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.TTipoHabitacion.ObtenerEntidadAsync(x => x.Id == datos.Id);
                if (!respuesta.Success)
                    return Falla<TTipoHabitacion, TiposHabitacion>(respuesta);

                if (respuesta.Data == null)
                    return Falla<TTipoHabitacion>("Tipo de habitación no encontrado.", CodigosRespuesta.NoEncontrado);

                return Respuesta<TTipoHabitacion>.Ok(_mapper.Map<TTipoHabitacion>(respuesta.Data));
            }
            catch (Exception ex)
            {
                return Excepcion<TTipoHabitacion>(ex, $"Error al obtener el tipo de habitación {datos.Id}.");
            }
        }

        public async Task<Respuesta<TTipoHabitacion>> CrearAsync(TTipoHabitacion datos)
        {
            try
            {
                var validacion = await ValidarAsync(datos, idActual: null);
                if (!validacion.Success)
                    return validacion;

                var entidad = _mapper.Map<TiposHabitacion>(datos);
                entidad.Id = 0;
                entidad.Activo = true;

                var creado = await _unidadDeTrabajo.TTipoHabitacion.InsertarAsync(entidad);
                if (!creado.Success)
                    return Falla<TTipoHabitacion, TiposHabitacion>(creado);

                return Respuesta<TTipoHabitacion>.Ok(_mapper.Map<TTipoHabitacion>(entidad));
            }
            catch (Exception ex)
            {
                return Excepcion<TTipoHabitacion>(ex, "Error al crear el tipo de habitación.");
            }
        }

        // Cambiar el precio no altera reservas existentes: cada noche guarda su propia tarifa
        public async Task<Respuesta<TTipoHabitacion>> ModificarAsync(TTipoHabitacion datos)
        {
            try
            {
                var actual = await _unidadDeTrabajo.TTipoHabitacion.ObtenerEntidadAsync(x => x.Id == datos.Id);
                if (!actual.Success)
                    return Falla<TTipoHabitacion, TiposHabitacion>(actual);

                if (actual.Data == null)
                    return Falla<TTipoHabitacion>("Tipo de habitación no encontrado.", CodigosRespuesta.NoEncontrado);

                var validacion = await ValidarAsync(datos, datos.Id);
                if (!validacion.Success)
                    return validacion;

                var entidad = actual.Data;
                var activo = entidad.Activo;
                _mapper.Map(datos, entidad);
                entidad.Activo = activo; // se cambia con CambiarActivo

                var guardado = await _unidadDeTrabajo.TTipoHabitacion.ModificarAsync(entidad);
                if (!guardado.Success)
                    return Falla<TTipoHabitacion, TiposHabitacion>(guardado);

                return Respuesta<TTipoHabitacion>.Ok(_mapper.Map<TTipoHabitacion>(entidad));
            }
            catch (Exception ex)
            {
                return Excepcion<TTipoHabitacion>(ex, $"Error al modificar el tipo de habitación {datos.Id}.");
            }
        }

        public async Task<Respuesta<TTipoHabitacion>> CambiarActivoAsync(TTipoHabitacion datos)
        {
            try
            {
                var actual = await _unidadDeTrabajo.TTipoHabitacion.ObtenerEntidadAsync(x => x.Id == datos.Id);
                if (!actual.Success)
                    return Falla<TTipoHabitacion, TiposHabitacion>(actual);

                if (actual.Data == null)
                    return Falla<TTipoHabitacion>("Tipo de habitación no encontrado.", CodigosRespuesta.NoEncontrado);

                actual.Data.Activo = datos.Activo;

                var guardado = await _unidadDeTrabajo.TTipoHabitacion.ModificarAsync(actual.Data);
                if (!guardado.Success)
                    return Falla<TTipoHabitacion, TiposHabitacion>(guardado);

                return Respuesta<TTipoHabitacion>.Ok(_mapper.Map<TTipoHabitacion>(actual.Data));
            }
            catch (Exception ex)
            {
                return Excepcion<TTipoHabitacion>(ex, $"Error al cambiar el estado del tipo {datos.Id}.");
            }
        }

        // Las mismas reglas que los CHECK de la tabla, para dar mensajes claros
        private async Task<Respuesta<TTipoHabitacion>> ValidarAsync(TTipoHabitacion datos, int? idActual)
        {
            datos.Nombre = datos.Nombre?.Trim() ?? string.Empty;
            datos.Descripcion = datos.Descripcion?.Trim() ?? string.Empty;

            if (datos.Nombre.Length == 0 || datos.Nombre.Length > 60)
                return Falla<TTipoHabitacion>("El nombre es obligatorio (máximo 60 caracteres).");

            if (datos.Descripcion.Length == 0 || datos.Descripcion.Length > 500)
                return Falla<TTipoHabitacion>("La descripción es obligatoria (máximo 500 caracteres).");

            if (datos.CapacidadAdultos < 1)
                return Falla<TTipoHabitacion>("Debe admitir al menos un adulto.");

            if (datos.CapacidadMaxima > datos.CapacidadAdultos + datos.CapacidadNinos)
                return Falla<TTipoHabitacion>("La capacidad máxima no puede superar adultos + niños.");

            if (datos.OcupacionBase < 1 || datos.OcupacionBase > datos.CapacidadMaxima)
                return Falla<TTipoHabitacion>("La ocupación base debe estar entre 1 y la capacidad máxima.");

            if (datos.PrecioBasePorNoche <= 0)
                return Falla<TTipoHabitacion>("El precio por noche debe ser mayor a 0.");

            if (datos.PrecioPersonaAdicional < 0)
                return Falla<TTipoHabitacion>("El precio por persona adicional no puede ser negativo.");

            var duplicado = await _unidadDeTrabajo.TTipoHabitacion.ContarAsync(
                x => x.Nombre == datos.Nombre && (idActual == null || x.Id != idActual));
            if (!duplicado.Success)
                return Falla<TTipoHabitacion, int?>(duplicado);
            if (duplicado.Data > 0)
                return Falla<TTipoHabitacion>("Ya existe un tipo de habitación con ese nombre.", CodigosRespuesta.Conflicto);

            return Respuesta<TTipoHabitacion>.Ok(datos);
        }
    }
}
