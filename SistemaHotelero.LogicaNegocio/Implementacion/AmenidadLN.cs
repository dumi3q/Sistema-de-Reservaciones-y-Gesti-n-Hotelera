using AutoMapper;
using Microsoft.Extensions.Logging;
using SistemaHotelero.Dominio.Entidades;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.InterfazAD;
using SistemaHotelero.Dominio.InterfazLN;
using SistemaHotelero.Utilitarios;

namespace SistemaHotelero.LogicaNegocio.Implementacion
{
    public class AmenidadLN : BaseLN<AmenidadLN>, IAmenidadLN
    {
        public AmenidadLN(
            IUnidadTrabajoEF unidadTrabajo,
            ILogger<AmenidadLN> logger,
            IMapper mapper)
            : base(unidadTrabajo, mapper, logger)
        {
        }

        public async Task<Respuesta<IEnumerable<TAmenidad>>> ListarAsync(bool soloActivos)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.TAmenidad.BuscarAsync(x => !soloActivos || x.Activo);
                if (!respuesta.Success)
                    return Falla<IEnumerable<TAmenidad>, IEnumerable<Amenidade>>(respuesta);

                return Respuesta<IEnumerable<TAmenidad>>.Ok(
                    _mapper.Map<IEnumerable<TAmenidad>>(respuesta.Data).OrderBy(x => x.Nombre));
            }
            catch (Exception ex)
            {
                return Excepcion<IEnumerable<TAmenidad>>(ex, "Error al listar amenidades.");
            }
        }

        public async Task<Respuesta<TAmenidad>> ObtenerAsync(TAmenidad datos)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.TAmenidad.ObtenerEntidadAsync(x => x.Id == datos.Id);
                if (!respuesta.Success)
                    return Falla<TAmenidad, Amenidade>(respuesta);

                if (respuesta.Data == null)
                    return Falla<TAmenidad>("Amenidad no encontrada.", CodigosRespuesta.NoEncontrado);

                return Respuesta<TAmenidad>.Ok(_mapper.Map<TAmenidad>(respuesta.Data));
            }
            catch (Exception ex)
            {
                return Excepcion<TAmenidad>(ex, $"Error al obtener la amenidad {datos.Id}.");
            }
        }

        public async Task<Respuesta<TAmenidad>> CrearAsync(TAmenidad datos)
        {
            try
            {
                var validacion = await ValidarAsync(datos, idActual: null);
                if (!validacion.Success)
                    return validacion;

                var entidad = new Amenidade
                {
                    Nombre = datos.Nombre,
                    Descripcion = datos.Descripcion,
                    Activo = true
                };

                var creado = await _unidadDeTrabajo.TAmenidad.InsertarAsync(entidad);
                if (!creado.Success)
                    return Falla<TAmenidad, Amenidade>(creado);

                return Respuesta<TAmenidad>.Ok(_mapper.Map<TAmenidad>(entidad));
            }
            catch (Exception ex)
            {
                return Excepcion<TAmenidad>(ex, "Error al crear la amenidad.");
            }
        }

        public async Task<Respuesta<TAmenidad>> ModificarAsync(TAmenidad datos)
        {
            try
            {
                var actual = await _unidadDeTrabajo.TAmenidad.ObtenerEntidadAsync(x => x.Id == datos.Id);
                if (!actual.Success)
                    return Falla<TAmenidad, Amenidade>(actual);

                if (actual.Data == null)
                    return Falla<TAmenidad>("Amenidad no encontrada.", CodigosRespuesta.NoEncontrado);

                var validacion = await ValidarAsync(datos, datos.Id);
                if (!validacion.Success)
                    return validacion;

                actual.Data.Nombre = datos.Nombre;
                actual.Data.Descripcion = datos.Descripcion;

                var guardado = await _unidadDeTrabajo.TAmenidad.ModificarAsync(actual.Data);
                if (!guardado.Success)
                    return Falla<TAmenidad, Amenidade>(guardado);

                return Respuesta<TAmenidad>.Ok(_mapper.Map<TAmenidad>(actual.Data));
            }
            catch (Exception ex)
            {
                return Excepcion<TAmenidad>(ex, $"Error al modificar la amenidad {datos.Id}.");
            }
        }

        public async Task<Respuesta<TAmenidad>> CambiarActivoAsync(TAmenidad datos)
        {
            try
            {
                var actual = await _unidadDeTrabajo.TAmenidad.ObtenerEntidadAsync(x => x.Id == datos.Id);
                if (!actual.Success)
                    return Falla<TAmenidad, Amenidade>(actual);

                if (actual.Data == null)
                    return Falla<TAmenidad>("Amenidad no encontrada.", CodigosRespuesta.NoEncontrado);

                actual.Data.Activo = datos.Activo;

                var guardado = await _unidadDeTrabajo.TAmenidad.ModificarAsync(actual.Data);
                if (!guardado.Success)
                    return Falla<TAmenidad, Amenidade>(guardado);

                return Respuesta<TAmenidad>.Ok(_mapper.Map<TAmenidad>(actual.Data));
            }
            catch (Exception ex)
            {
                return Excepcion<TAmenidad>(ex, $"Error al cambiar el estado de la amenidad {datos.Id}.");
            }
        }

        private async Task<Respuesta<TAmenidad>> ValidarAsync(TAmenidad datos, int? idActual)
        {
            datos.Nombre = datos.Nombre?.Trim() ?? string.Empty;
            datos.Descripcion = string.IsNullOrWhiteSpace(datos.Descripcion) ? null : datos.Descripcion.Trim();

            if (datos.Nombre.Length == 0 || datos.Nombre.Length > 60)
                return Falla<TAmenidad>("El nombre es obligatorio (máximo 60 caracteres).");

            if (datos.Descripcion != null && datos.Descripcion.Length > 200)
                return Falla<TAmenidad>("La descripción admite máximo 200 caracteres.");

            var duplicado = await _unidadDeTrabajo.TAmenidad.ContarAsync(
                x => x.Nombre == datos.Nombre && (idActual == null || x.Id != idActual));
            if (!duplicado.Success)
                return Falla<TAmenidad, int?>(duplicado);
            if (duplicado.Data > 0)
                return Falla<TAmenidad>("Ya existe una amenidad con ese nombre.", CodigosRespuesta.Conflicto);

            return Respuesta<TAmenidad>.Ok(datos);
        }
    }
}
