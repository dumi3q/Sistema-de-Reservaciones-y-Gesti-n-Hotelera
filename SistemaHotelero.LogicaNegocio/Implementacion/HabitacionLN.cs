using AutoMapper;
using Microsoft.Extensions.Logging;
using SistemaHotelero.Dominio.Clases;
using SistemaHotelero.Dominio.Entidades;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.Enumeraciones;
using SistemaHotelero.Dominio.InterfazAD;
using SistemaHotelero.Dominio.InterfazLN;
using SistemaHotelero.Utilitarios;

namespace SistemaHotelero.LogicaNegocio.Implementacion
{
    public class HabitacionLN : BaseLN<HabitacionLN>, IHabitacionLN
    {
        private static readonly List<string> IncludesDetalle = new()
        {
            "Edificio",
            "TipoHabitacion",
            "HabitacionAmenidades",
            "HabitacionAmenidades.Amenidad",
            "FotosHabitacions"
        };

        private static readonly List<string> IncludesListado = new() { "Edificio", "TipoHabitacion" };

        public HabitacionLN(
            IUnidadTrabajoEF unidadTrabajo,
            ILogger<HabitacionLN> logger,
            IMapper mapper)
            : base(unidadTrabajo, mapper, logger)
        {
        }

        #region Consultas

        public async Task<Respuesta<IEnumerable<THabitacion>>> ListarAsync(clsFiltroHabitacion filtro)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.THabitacion.BuscarAsync(
                    x => (filtro.EdificioId == null || x.EdificioId == filtro.EdificioId)
                      && (filtro.Piso == null || x.Piso == filtro.Piso)
                      && (filtro.TipoHabitacionId == null || x.TipoHabitacionId == filtro.TipoHabitacionId)
                      && (filtro.Estado == null || x.Estado == filtro.Estado),
                    IncludesListado);

                if (!respuesta.Success)
                    return Falla<IEnumerable<THabitacion>, IEnumerable<Habitacione>>(respuesta);

                var datos = _mapper.Map<IEnumerable<THabitacion>>(respuesta.Data)
                    .OrderBy(x => x.EdificioCodigo)
                    .ThenBy(x => x.Piso)
                    .ThenBy(x => x.Numero);

                return Respuesta<IEnumerable<THabitacion>>.Ok(datos);
            }
            catch (Exception ex)
            {
                return Excepcion<IEnumerable<THabitacion>>(ex, "Error al listar habitaciones.");
            }
        }

        public async Task<Respuesta<THabitacion>> ObtenerAsync(THabitacion datos)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.THabitacion.ObtenerEntidadAsync(
                    x => x.Id == datos.Id, IncludesDetalle);

                if (!respuesta.Success)
                    return Falla<THabitacion, Habitacione>(respuesta);

                if (respuesta.Data == null)
                    return Falla<THabitacion>("Habitación no encontrada.", CodigosRespuesta.NoEncontrado);

                return Respuesta<THabitacion>.Ok(_mapper.Map<THabitacion>(respuesta.Data));
            }
            catch (Exception ex)
            {
                return Excepcion<THabitacion>(ex, $"Error al obtener la habitación {datos.Id}.");
            }
        }

        public async Task<Respuesta<IEnumerable<THistorialEstadoHabitacion>>> ListarHistorialEstadosAsync(int habitacionId)
        {
            try
            {
                var respuesta = await _unidadDeTrabajo.THistorialEstadoHabitacion.BuscarAsync(
                    x => x.HabitacionId == habitacionId);

                if (!respuesta.Success)
                    return Falla<IEnumerable<THistorialEstadoHabitacion>, IEnumerable<HistorialEstadosHabitacion>>(respuesta);

                return Respuesta<IEnumerable<THistorialEstadoHabitacion>>.Ok(
                    _mapper.Map<IEnumerable<THistorialEstadoHabitacion>>(respuesta.Data)
                        .OrderByDescending(x => x.FechaUtc));
            }
            catch (Exception ex)
            {
                return Excepcion<IEnumerable<THistorialEstadoHabitacion>>(ex, $"Error al listar el historial de la habitación {habitacionId}.");
            }
        }

        #endregion

        #region Crear y modificar

        public async Task<Respuesta<THabitacion>> CrearAsync(THabitacion datos)
        {
            try
            {
                var validacion = await ValidarDatosAsync(datos, idActual: null);
                if (!validacion.Success)
                    return validacion;

                var entidad = new Habitacione
                {
                    EdificioId = datos.EdificioId,
                    TipoHabitacionId = datos.TipoHabitacionId,
                    Numero = datos.Numero,
                    Piso = datos.Piso,
                    Estado = (byte)EstadoHabitacion.Disponible
                };

                var creada = await _unidadDeTrabajo.THabitacion.InsertarAsync(entidad);
                if (!creada.Success)
                    return Falla<THabitacion, Habitacione>(creada);

                return await ObtenerAsync(new THabitacion { Id = entidad.Id });
            }
            catch (Exception ex)
            {
                return Excepcion<THabitacion>(ex, "Error al crear la habitación.");
            }
        }

        // Cambia número, piso y tipo. El edificio y el estado no se cambian aquí.
        public async Task<Respuesta<THabitacion>> ModificarAsync(THabitacion datos)
        {
            try
            {
                var actual = await _unidadDeTrabajo.THabitacion.ObtenerEntidadAsync(x => x.Id == datos.Id);
                if (!actual.Success)
                    return Falla<THabitacion, Habitacione>(actual);

                if (actual.Data == null)
                    return Falla<THabitacion>("Habitación no encontrada.", CodigosRespuesta.NoEncontrado);

                var habitacion = actual.Data;
                datos.EdificioId = habitacion.EdificioId;

                var validacion = await ValidarDatosAsync(datos, habitacion.Id);
                if (!validacion.Success)
                    return validacion;

                // Cambiar el tipo cambia la capacidad: no se permite con reservas por delante
                if (datos.TipoHabitacionId != habitacion.TipoHabitacionId)
                {
                    var hoy = HoyHotel();
                    var nochesFuturas = await _unidadDeTrabajo.TReservacionNoche.ContarAsync(
                        x => x.HabitacionId == habitacion.Id && x.Activa && x.Fecha >= hoy);

                    if (!nochesFuturas.Success)
                        return Falla<THabitacion, int?>(nochesFuturas);

                    if (nochesFuturas.Data > 0)
                        return Falla<THabitacion>(
                            "No se puede cambiar el tipo: la habitación tiene reservas desde hoy en adelante.",
                            CodigosRespuesta.Conflicto);
                }

                habitacion.Numero = datos.Numero;
                habitacion.Piso = datos.Piso;
                habitacion.TipoHabitacionId = datos.TipoHabitacionId;

                var guardado = await _unidadDeTrabajo.THabitacion.ModificarAsync(habitacion);
                if (!guardado.Success)
                    return Falla<THabitacion, Habitacione>(guardado);

                return await ObtenerAsync(new THabitacion { Id = habitacion.Id });
            }
            catch (Exception ex)
            {
                return Excepcion<THabitacion>(ex, $"Error al modificar la habitación {datos.Id}.");
            }
        }

        private async Task<Respuesta<THabitacion>> ValidarDatosAsync(THabitacion datos, int? idActual)
        {
            datos.Numero = datos.Numero?.Trim() ?? string.Empty;

            if (datos.Numero.Length == 0 || datos.Numero.Length > 10)
                return Falla<THabitacion>("El número es obligatorio (máximo 10 caracteres).");

            var edificio = await _unidadDeTrabajo.TEdificio.ObtenerEntidadAsync(x => x.Id == datos.EdificioId);
            if (!edificio.Success)
                return Falla<THabitacion, Edificio>(edificio);
            if (edificio.Data == null || !edificio.Data.Activo)
                return Falla<THabitacion>("El edificio no existe o está inactivo.");

            if (datos.Piso < 1 || datos.Piso > edificio.Data.CantidadPisos)
                return Falla<THabitacion>($"El piso debe estar entre 1 y {edificio.Data.CantidadPisos}.");

            var tipo = await _unidadDeTrabajo.TTipoHabitacion.ObtenerEntidadAsync(x => x.Id == datos.TipoHabitacionId);
            if (!tipo.Success)
                return Falla<THabitacion, TiposHabitacion>(tipo);
            if (tipo.Data == null || !tipo.Data.Activo)
                return Falla<THabitacion>("El tipo de habitación no existe o está inactivo.");

            var duplicado = await _unidadDeTrabajo.THabitacion.ContarAsync(
                x => x.EdificioId == datos.EdificioId
                  && x.Numero == datos.Numero
                  && (idActual == null || x.Id != idActual));
            if (!duplicado.Success)
                return Falla<THabitacion, int?>(duplicado);
            if (duplicado.Data > 0)
                return Falla<THabitacion>(
                    $"Ya existe la habitación {datos.Numero} en el edificio {edificio.Data.Codigo}.",
                    CodigosRespuesta.Conflicto);

            return Respuesta<THabitacion>.Ok(datos);
        }

        #endregion

        #region Estado operativo

        public async Task<Respuesta<THabitacion>> CambiarEstadoAsync(int habitacionId, clsCambioEstadoHabitacion datos, int usuarioId)
        {
            try
            {
                if (!Enum.IsDefined(typeof(EstadoHabitacion), datos.EstadoNuevo))
                    return Falla<THabitacion>("El estado indicado no existe.");

                var nuevo = (EstadoHabitacion)datos.EstadoNuevo;

                if (nuevo is EstadoHabitacion.Reservada or EstadoHabitacion.Ocupada)
                    return Falla<THabitacion>("Los estados Reservada y Ocupada los asignan las reservas y el check-in.");

                var motivo = string.IsNullOrWhiteSpace(datos.Motivo) ? null : datos.Motivo.Trim();
                if (motivo != null && motivo.Length > 300)
                    return Falla<THabitacion>("El motivo admite máximo 300 caracteres.");

                if ((nuevo is EstadoHabitacion.EnMantenimiento or EstadoHabitacion.Bloqueada or EstadoHabitacion.FueraDeServicio)
                    && motivo == null)
                    return Falla<THabitacion>("Indique el motivo del cambio.");

                var actual = await _unidadDeTrabajo.THabitacion.ObtenerEntidadAsync(x => x.Id == habitacionId);
                if (!actual.Success)
                    return Falla<THabitacion, Habitacione>(actual);
                if (actual.Data == null)
                    return Falla<THabitacion>("Habitación no encontrada.", CodigosRespuesta.NoEncontrado);

                var habitacion = actual.Data;
                var anterior = (EstadoHabitacion)habitacion.Estado;

                if (anterior == nuevo)
                    return Falla<THabitacion>($"La habitación ya está en estado {nuevo}.");

                if (anterior == EstadoHabitacion.Ocupada)
                    return Falla<THabitacion>(
                        "La habitación está ocupada; su estado cambia con el check-out.",
                        CodigosRespuesta.Conflicto);

                _unidadDeTrabajo.EmpezarTransaccion();

                habitacion.Estado = (byte)nuevo;
                var guardado = await _unidadDeTrabajo.THabitacion.ModificarAsync(habitacion);
                if (!guardado.Success)
                {
                    _unidadDeTrabajo.Rollback();
                    return Falla<THabitacion, Habitacione>(guardado);
                }

                var historial = await _unidadDeTrabajo.THistorialEstadoHabitacion.InsertarAsync(
                    new HistorialEstadosHabitacion
                    {
                        HabitacionId = habitacion.Id,
                        UsuarioId = usuarioId,
                        EstadoAnterior = (byte)anterior,
                        EstadoNuevo = (byte)nuevo,
                        FechaUtc = DateTime.UtcNow,
                        Motivo = motivo
                    });
                if (!historial.Success)
                {
                    _unidadDeTrabajo.Rollback();
                    return Falla<THabitacion, HistorialEstadosHabitacion>(historial);
                }

                _unidadDeTrabajo.CompletarTran();

                return await ObtenerAsync(new THabitacion { Id = habitacion.Id });
            }
            catch (Exception ex)
            {
                _unidadDeTrabajo.Rollback();
                return Excepcion<THabitacion>(ex, $"Error al cambiar el estado de la habitación {habitacionId}.");
            }
        }

        #endregion

        #region Amenidades

        public async Task<Respuesta<THabitacion>> AsignarAmenidadesAsync(int habitacionId, List<clsAmenidadAsignada> amenidades)
        {
            try
            {
                amenidades ??= new();

                if (amenidades.GroupBy(x => x.AmenidadId).Any(g => g.Count() > 1))
                    return Falla<THabitacion>("Hay amenidades repetidas en la lista.");

                if (amenidades.Any(x => x.Cantidad < 1))
                    return Falla<THabitacion>("La cantidad de cada amenidad debe ser al menos 1.");

                var existe = await _unidadDeTrabajo.THabitacion.ContarAsync(x => x.Id == habitacionId);
                if (!existe.Success)
                    return Falla<THabitacion, int?>(existe);
                if (existe.Data == 0)
                    return Falla<THabitacion>("Habitación no encontrada.", CodigosRespuesta.NoEncontrado);

                var ids = amenidades.Select(x => x.AmenidadId).ToList();
                var validas = await _unidadDeTrabajo.TAmenidad.ContarAsync(x => ids.Contains(x.Id) && x.Activo);
                if (!validas.Success)
                    return Falla<THabitacion, int?>(validas);
                if (validas.Data != ids.Count)
                    return Falla<THabitacion>("Alguna amenidad no existe o está inactiva.");

                var actuales = await _unidadDeTrabajo.THabitacionAmenidad.BuscarAsync(x => x.HabitacionId == habitacionId);
                if (!actuales.Success)
                    return Falla<THabitacion, IEnumerable<HabitacionAmenidade>>(actuales);

                _unidadDeTrabajo.EmpezarTransaccion();

                foreach (var asignada in actuales.Data!)
                {
                    var nueva = amenidades.FirstOrDefault(x => x.AmenidadId == asignada.AmenidadId);

                    if (nueva == null)
                    {
                        var eliminada = await _unidadDeTrabajo.THabitacionAmenidad.EliminarAsync(asignada);
                        if (!eliminada.Success)
                        {
                            _unidadDeTrabajo.Rollback();
                            return Falla<THabitacion, bool>(eliminada);
                        }
                    }
                    else if (nueva.Cantidad != asignada.Cantidad)
                    {
                        asignada.Cantidad = nueva.Cantidad;
                        var modificada = await _unidadDeTrabajo.THabitacionAmenidad.ModificarAsync(asignada);
                        if (!modificada.Success)
                        {
                            _unidadDeTrabajo.Rollback();
                            return Falla<THabitacion, HabitacionAmenidade>(modificada);
                        }
                    }
                }

                var yaAsignadas = actuales.Data!.Select(x => x.AmenidadId).ToHashSet();

                foreach (var nueva in amenidades.Where(x => !yaAsignadas.Contains(x.AmenidadId)))
                {
                    var insertada = await _unidadDeTrabajo.THabitacionAmenidad.InsertarAsync(
                        new HabitacionAmenidade
                        {
                            HabitacionId = habitacionId,
                            AmenidadId = nueva.AmenidadId,
                            Cantidad = nueva.Cantidad
                        });
                    if (!insertada.Success)
                    {
                        _unidadDeTrabajo.Rollback();
                        return Falla<THabitacion, HabitacionAmenidade>(insertada);
                    }
                }

                _unidadDeTrabajo.CompletarTran();

                return await ObtenerAsync(new THabitacion { Id = habitacionId });
            }
            catch (Exception ex)
            {
                _unidadDeTrabajo.Rollback();
                return Excepcion<THabitacion>(ex, $"Error al asignar amenidades a la habitación {habitacionId}.");
            }
        }

        #endregion

        #region Fotos

        public async Task<Respuesta<TFotoHabitacion>> AgregarFotoAsync(TFotoHabitacion datos)
        {
            try
            {
                datos.Url = datos.Url?.Trim() ?? string.Empty;
                datos.Descripcion = string.IsNullOrWhiteSpace(datos.Descripcion) ? null : datos.Descripcion.Trim();

                if (datos.Url.Length == 0 || datos.Url.Length > 500)
                    return Falla<TFotoHabitacion>("La URL es obligatoria (máximo 500 caracteres).");

                if (!Uri.TryCreate(datos.Url, UriKind.RelativeOrAbsolute, out _))
                    return Falla<TFotoHabitacion>("La URL no es válida.");

                if (datos.Descripcion != null && datos.Descripcion.Length > 200)
                    return Falla<TFotoHabitacion>("La descripción admite máximo 200 caracteres.");

                var existe = await _unidadDeTrabajo.THabitacion.ContarAsync(x => x.Id == datos.HabitacionId);
                if (!existe.Success)
                    return Falla<TFotoHabitacion, int?>(existe);
                if (existe.Data == 0)
                    return Falla<TFotoHabitacion>("Habitación no encontrada.", CodigosRespuesta.NoEncontrado);

                var principal = await _unidadDeTrabajo.TFotoHabitacion.ObtenerEntidadAsync(
                    x => x.HabitacionId == datos.HabitacionId && x.EsPrincipal);
                if (!principal.Success)
                    return Falla<TFotoHabitacion, FotosHabitacion>(principal);

                // La primera foto siempre queda como principal
                var esPrincipal = datos.EsPrincipal || principal.Data == null;

                _unidadDeTrabajo.EmpezarTransaccion();

                // El índice único solo admite una principal: primero se quita la anterior
                if (esPrincipal && principal.Data != null)
                {
                    principal.Data.EsPrincipal = false;
                    var quitada = await _unidadDeTrabajo.TFotoHabitacion.ModificarAsync(principal.Data);
                    if (!quitada.Success)
                    {
                        _unidadDeTrabajo.Rollback();
                        return Falla<TFotoHabitacion, FotosHabitacion>(quitada);
                    }
                }

                var foto = new FotosHabitacion
                {
                    HabitacionId = datos.HabitacionId,
                    Url = datos.Url,
                    Descripcion = datos.Descripcion,
                    Orden = datos.Orden,
                    EsPrincipal = esPrincipal
                };

                var creada = await _unidadDeTrabajo.TFotoHabitacion.InsertarAsync(foto);
                if (!creada.Success)
                {
                    _unidadDeTrabajo.Rollback();
                    return Falla<TFotoHabitacion, FotosHabitacion>(creada);
                }

                _unidadDeTrabajo.CompletarTran();

                return Respuesta<TFotoHabitacion>.Ok(_mapper.Map<TFotoHabitacion>(foto));
            }
            catch (Exception ex)
            {
                _unidadDeTrabajo.Rollback();
                return Excepcion<TFotoHabitacion>(ex, "Error al agregar la foto.");
            }
        }

        public async Task<Respuesta<bool>> EliminarFotoAsync(int fotoId)
        {
            try
            {
                var foto = await _unidadDeTrabajo.TFotoHabitacion.ObtenerEntidadAsync(x => x.Id == fotoId);
                if (!foto.Success)
                    return Falla<bool, FotosHabitacion>(foto);
                if (foto.Data == null)
                    return Falla<bool>("Foto no encontrada.", CodigosRespuesta.NoEncontrado);

                var eraPrincipal = foto.Data.EsPrincipal;
                var habitacionId = foto.Data.HabitacionId;

                _unidadDeTrabajo.EmpezarTransaccion();

                var eliminada = await _unidadDeTrabajo.TFotoHabitacion.EliminarAsync(foto.Data);
                if (!eliminada.Success)
                {
                    _unidadDeTrabajo.Rollback();
                    return eliminada;
                }

                // Si se borró la principal, la siguiente por orden pasa a ser principal
                if (eraPrincipal)
                {
                    var restantes = await _unidadDeTrabajo.TFotoHabitacion.BuscarAsync(x => x.HabitacionId == habitacionId);
                    if (!restantes.Success)
                    {
                        _unidadDeTrabajo.Rollback();
                        return Falla<bool, IEnumerable<FotosHabitacion>>(restantes);
                    }

                    var siguiente = restantes.Data!.OrderBy(x => x.Orden).ThenBy(x => x.Id).FirstOrDefault();
                    if (siguiente != null)
                    {
                        siguiente.EsPrincipal = true;
                        var promovida = await _unidadDeTrabajo.TFotoHabitacion.ModificarAsync(siguiente);
                        if (!promovida.Success)
                        {
                            _unidadDeTrabajo.Rollback();
                            return Falla<bool, FotosHabitacion>(promovida);
                        }
                    }
                }

                _unidadDeTrabajo.CompletarTran();
                return Respuesta<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                _unidadDeTrabajo.Rollback();
                return Excepcion<bool>(ex, $"Error al eliminar la foto {fotoId}.");
            }
        }

        public async Task<Respuesta<TFotoHabitacion>> MarcarFotoPrincipalAsync(int fotoId)
        {
            try
            {
                var foto = await _unidadDeTrabajo.TFotoHabitacion.ObtenerEntidadAsync(x => x.Id == fotoId);
                if (!foto.Success)
                    return Falla<TFotoHabitacion, FotosHabitacion>(foto);
                if (foto.Data == null)
                    return Falla<TFotoHabitacion>("Foto no encontrada.", CodigosRespuesta.NoEncontrado);

                if (foto.Data.EsPrincipal)
                    return Respuesta<TFotoHabitacion>.Ok(_mapper.Map<TFotoHabitacion>(foto.Data));

                var anterior = await _unidadDeTrabajo.TFotoHabitacion.ObtenerEntidadAsync(
                    x => x.HabitacionId == foto.Data.HabitacionId && x.EsPrincipal);
                if (!anterior.Success)
                    return Falla<TFotoHabitacion, FotosHabitacion>(anterior);

                _unidadDeTrabajo.EmpezarTransaccion();

                if (anterior.Data != null)
                {
                    anterior.Data.EsPrincipal = false;
                    var quitada = await _unidadDeTrabajo.TFotoHabitacion.ModificarAsync(anterior.Data);
                    if (!quitada.Success)
                    {
                        _unidadDeTrabajo.Rollback();
                        return Falla<TFotoHabitacion, FotosHabitacion>(quitada);
                    }
                }

                foto.Data.EsPrincipal = true;
                var marcada = await _unidadDeTrabajo.TFotoHabitacion.ModificarAsync(foto.Data);
                if (!marcada.Success)
                {
                    _unidadDeTrabajo.Rollback();
                    return Falla<TFotoHabitacion, FotosHabitacion>(marcada);
                }

                _unidadDeTrabajo.CompletarTran();

                return Respuesta<TFotoHabitacion>.Ok(_mapper.Map<TFotoHabitacion>(foto.Data));
            }
            catch (Exception ex)
            {
                _unidadDeTrabajo.Rollback();
                return Excepcion<TFotoHabitacion>(ex, $"Error al marcar la foto {fotoId} como principal.");
            }
        }

        #endregion
    }
}
