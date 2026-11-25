using AutoMapper;
using Microsoft.Extensions.Logging;
using SistemaHotelero.Dominio.Entidades;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.Enumeraciones;
using SistemaHotelero.Dominio.InterfazAD;
using SistemaHotelero.Dominio.InterfazLN;
using SistemaHotelero.Utilitarios;

namespace SistemaHotelero.LogicaNegocio.Implementacion
{
    // Las reglas solo afectan cotizaciones nuevas: cada reserva ya guardó el precio de sus noches
    public class ReglaTarifaLN : BaseLN<ReglaTarifaLN>, IReglaTarifaLN
    {
        private static readonly List<string> Includes = new() { "TipoHabitacion" };

        public ReglaTarifaLN(
            IUnidadTrabajoEF unidadTrabajo,
            ILogger<ReglaTarifaLN> logger,
            IMapper mapper)
            : base(unidadTrabajo, mapper, logger)
        {
        }

        public async Task<Respuesta<IEnumerable<TReglaTarifa>>> ListarAsync(bool soloActivos)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.TReglaTarifa.BuscarAsync(x => !soloActivos || x.Activo, Includes);
                if (!respuesta.Success)
                    return Falla<IEnumerable<TReglaTarifa>, IEnumerable<ReglasTarifa>>(respuesta);

                return Respuesta<IEnumerable<TReglaTarifa>>.Ok(
                    _mapper.Map<IEnumerable<TReglaTarifa>>(respuesta.Data)
                        .OrderBy(x => x.Tipo)
                        .ThenBy(x => x.FechaInicio)
                        .ThenBy(x => x.DiaSemana));
            }
            catch (Exception ex)
            {
                return Excepcion<IEnumerable<TReglaTarifa>>(ex, "Error al listar reglas de tarifa.");
            }
        }

        public async Task<Respuesta<TReglaTarifa>> ObtenerAsync(TReglaTarifa datos)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.TReglaTarifa.ObtenerEntidadAsync(x => x.Id == datos.Id, Includes);
                if (!respuesta.Success)
                    return Falla<TReglaTarifa, ReglasTarifa>(respuesta);

                if (respuesta.Data == null)
                    return Falla<TReglaTarifa>("Regla de tarifa no encontrada.", CodigosRespuesta.NoEncontrado);

                return Respuesta<TReglaTarifa>.Ok(_mapper.Map<TReglaTarifa>(respuesta.Data));
            }
            catch (Exception ex)
            {
                return Excepcion<TReglaTarifa>(ex, $"Error al obtener la regla de tarifa {datos.Id}.");
            }
        }

        public async Task<Respuesta<TReglaTarifa>> CrearAsync(TReglaTarifa datos)
        {
            try
            {
                var validacion = await ValidarAsync(datos);
                if (!validacion.Success)
                    return validacion;

                var entidad = new ReglasTarifa { Activo = true };
                Copiar(datos, entidad);

                var creada = await _unidadDeTrabajo.TReglaTarifa.InsertarAsync(entidad);
                if (!creada.Success)
                    return Falla<TReglaTarifa, ReglasTarifa>(creada);

                return await ObtenerAsync(new TReglaTarifa { Id = entidad.Id });
            }
            catch (Exception ex)
            {
                return Excepcion<TReglaTarifa>(ex, "Error al crear la regla de tarifa.");
            }
        }

        public async Task<Respuesta<TReglaTarifa>> ModificarAsync(TReglaTarifa datos)
        {
            try
            {
                var actual = await _unidadDeTrabajo.TReglaTarifa.ObtenerEntidadAsync(x => x.Id == datos.Id);
                if (!actual.Success)
                    return Falla<TReglaTarifa, ReglasTarifa>(actual);

                if (actual.Data == null)
                    return Falla<TReglaTarifa>("Regla de tarifa no encontrada.", CodigosRespuesta.NoEncontrado);

                var validacion = await ValidarAsync(datos);
                if (!validacion.Success)
                    return validacion;

                Copiar(datos, actual.Data);

                var guardada = await _unidadDeTrabajo.TReglaTarifa.ModificarAsync(actual.Data);
                if (!guardada.Success)
                    return Falla<TReglaTarifa, ReglasTarifa>(guardada);

                return await ObtenerAsync(new TReglaTarifa { Id = datos.Id });
            }
            catch (Exception ex)
            {
                return Excepcion<TReglaTarifa>(ex, $"Error al modificar la regla de tarifa {datos.Id}.");
            }
        }

        public async Task<Respuesta<TReglaTarifa>> CambiarActivoAsync(TReglaTarifa datos)
        {
            try
            {
                var actual = await _unidadDeTrabajo.TReglaTarifa.ObtenerEntidadAsync(x => x.Id == datos.Id);
                if (!actual.Success)
                    return Falla<TReglaTarifa, ReglasTarifa>(actual);

                if (actual.Data == null)
                    return Falla<TReglaTarifa>("Regla de tarifa no encontrada.", CodigosRespuesta.NoEncontrado);

                actual.Data.Activo = datos.Activo;

                var guardada = await _unidadDeTrabajo.TReglaTarifa.ModificarAsync(actual.Data);
                if (!guardada.Success)
                    return Falla<TReglaTarifa, ReglasTarifa>(guardada);

                return await ObtenerAsync(new TReglaTarifa { Id = datos.Id });
            }
            catch (Exception ex)
            {
                return Excepcion<TReglaTarifa>(ex, $"Error al cambiar el estado de la regla {datos.Id}.");
            }
        }

        // Mismas reglas que los CHECK de la tabla, con mensajes claros
        private async Task<Respuesta<TReglaTarifa>> ValidarAsync(TReglaTarifa datos)
        {
            datos.Nombre = datos.Nombre?.Trim() ?? string.Empty;

            if (datos.Nombre.Length == 0 || datos.Nombre.Length > 100)
                return Falla<TReglaTarifa>("El nombre es obligatorio (máximo 100 caracteres).");

            switch ((TipoReglaTarifa)datos.Tipo)
            {
                case TipoReglaTarifa.Temporada:
                    if (datos.FechaInicio == null || datos.FechaFin == null)
                        return Falla<TReglaTarifa>("Una temporada necesita fecha de inicio y de fin.");
                    if (datos.FechaInicio > datos.FechaFin)
                        return Falla<TReglaTarifa>("La fecha de inicio no puede ser posterior a la de fin.");
                    datos.DiaSemana = null;
                    break;

                case TipoReglaTarifa.FinDeSemana:
                    if (datos.DiaSemana is null or < 1 or > 7)
                        return Falla<TReglaTarifa>("Indique el día de la semana (1 lunes ... 7 domingo).");
                    datos.FechaInicio = null;
                    datos.FechaFin = null;
                    break;

                default:
                    return Falla<TReglaTarifa>("El tipo debe ser 1 (Temporada) o 2 (Fin de semana).");
            }

            if (!Enum.IsDefined(typeof(TipoAjusteTarifa), datos.TipoAjuste))
                return Falla<TReglaTarifa>("El tipo de ajuste debe ser 1 (Porcentaje) o 2 (Monto fijo).");

            if (datos.TipoAjuste == (byte)TipoAjusteTarifa.Porcentaje && (datos.Valor < -100 || datos.Valor > 500))
                return Falla<TReglaTarifa>("El porcentaje debe estar entre -100 y 500.");

            if (datos.TipoHabitacionId != null)
            {
                var tipo = await _unidadDeTrabajo.TTipoHabitacion.ContarAsync(x => x.Id == datos.TipoHabitacionId);
                if (!tipo.Success)
                    return Falla<TReglaTarifa, int?>(tipo);
                if (tipo.Data == 0)
                    return Falla<TReglaTarifa>("El tipo de habitación no existe.");
            }

            return Respuesta<TReglaTarifa>.Ok(datos);
        }

        private static void Copiar(TReglaTarifa origen, ReglasTarifa destino)
        {
            destino.TipoHabitacionId = origen.TipoHabitacionId;
            destino.Nombre = origen.Nombre;
            destino.Tipo = origen.Tipo;
            destino.FechaInicio = origen.FechaInicio;
            destino.FechaFin = origen.FechaFin;
            destino.DiaSemana = origen.DiaSemana;
            destino.TipoAjuste = origen.TipoAjuste;
            destino.Valor = origen.Valor;
            destino.Prioridad = origen.Prioridad;
        }
    }
}
