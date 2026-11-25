using AutoMapper;
using Microsoft.Extensions.Logging;
using SistemaHotelero.Dominio.Entidades;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.InterfazAD;
using SistemaHotelero.Dominio.InterfazLN;
using SistemaHotelero.Utilitarios;

namespace SistemaHotelero.LogicaNegocio.Implementacion
{
    // Los 4 edificios vienen precargados; aquí solo se consultan y se editan sus datos
    public class EdificioLN : BaseLN<EdificioLN>, IEdificioLN
    {
        public EdificioLN(
            IUnidadTrabajoEF unidadTrabajo,
            ILogger<EdificioLN> logger,
            IMapper mapper)
            : base(unidadTrabajo, mapper, logger)
        {
        }

        public async Task<Respuesta<IEnumerable<TEdificio>>> ListarAsync()
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.TEdificio.ListarAsync();
                if (!respuesta.Success)
                    return Falla<IEnumerable<TEdificio>, IEnumerable<Edificio>>(respuesta);

                return Respuesta<IEnumerable<TEdificio>>.Ok(
                    _mapper.Map<IEnumerable<TEdificio>>(respuesta.Data).OrderBy(x => x.Codigo));
            }
            catch (Exception ex)
            {
                return Excepcion<IEnumerable<TEdificio>>(ex, "Error al listar edificios.");
            }
        }

        public async Task<Respuesta<TEdificio>> ObtenerAsync(TEdificio datos)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.TEdificio.ObtenerEntidadAsync(x => x.Id == datos.Id);
                if (!respuesta.Success)
                    return Falla<TEdificio, Edificio>(respuesta);

                if (respuesta.Data == null)
                    return Falla<TEdificio>("Edificio no encontrado.", CodigosRespuesta.NoEncontrado);

                return Respuesta<TEdificio>.Ok(_mapper.Map<TEdificio>(respuesta.Data));
            }
            catch (Exception ex)
            {
                return Excepcion<TEdificio>(ex, $"Error al obtener el edificio {datos.Id}.");
            }
        }

        // Permite cambiar nombre, descripción, pisos y Activo. El código (A-D) no cambia.
        public async Task<Respuesta<TEdificio>> ModificarAsync(TEdificio datos)
        {
            try
            {
                var actual = await _unidadDeTrabajo.TEdificio.ObtenerEntidadAsync(x => x.Id == datos.Id);
                if (!actual.Success)
                    return Falla<TEdificio, Edificio>(actual);

                if (actual.Data == null)
                    return Falla<TEdificio>("Edificio no encontrado.", CodigosRespuesta.NoEncontrado);

                datos.Nombre = datos.Nombre?.Trim() ?? string.Empty;
                if (datos.Nombre.Length == 0 || datos.Nombre.Length > 60)
                    return Falla<TEdificio>("El nombre es obligatorio (máximo 60 caracteres).");

                if (datos.CantidadPisos < 1 || datos.CantidadPisos > 6)
                    return Falla<TEdificio>("El edificio debe tener entre 1 y 6 pisos.");

                // No se pueden quitar pisos que ya tienen habitaciones
                var fueraDeRango = await _unidadDeTrabajo.THabitacion.ContarAsync(
                    x => x.EdificioId == datos.Id && x.Piso > datos.CantidadPisos);
                if (!fueraDeRango.Success)
                    return Falla<TEdificio, int?>(fueraDeRango);
                if (fueraDeRango.Data > 0)
                    return Falla<TEdificio>("Hay habitaciones en pisos superiores a la nueva cantidad de pisos.");

                var edificio = actual.Data;
                edificio.Nombre = datos.Nombre;
                edificio.Descripcion = string.IsNullOrWhiteSpace(datos.Descripcion) ? null : datos.Descripcion.Trim();
                edificio.CantidadPisos = datos.CantidadPisos;
                edificio.Activo = datos.Activo;

                var guardado = await _unidadDeTrabajo.TEdificio.ModificarAsync(edificio);
                if (!guardado.Success)
                    return Falla<TEdificio, Edificio>(guardado);

                return Respuesta<TEdificio>.Ok(_mapper.Map<TEdificio>(edificio));
            }
            catch (Exception ex)
            {
                return Excepcion<TEdificio>(ex, $"Error al modificar el edificio {datos.Id}.");
            }
        }
    }
}
