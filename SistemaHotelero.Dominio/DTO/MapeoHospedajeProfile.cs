using AutoMapper;
using SistemaHotelero.Dominio.Entidades;
using SistemaHotelero.Dominio.EntidadesTipadas;
using SistemaHotelero.Dominio.Enumeraciones;

namespace SistemaHotelero.Dominio.DTO;

// Mapeos del módulo de hospedaje (compañero 2). Cada compañero mantiene su propio Profile.
public class MapeoHospedajeProfile : Profile
{
    public MapeoHospedajeProfile()
    {
        // Clientes: los datos personales vienen de Persona
        CreateMap<Cliente, TCliente>()
            .ForMember(d => d.TipoDocumentoId, o => o.MapFrom(s => s.Persona.TipoDocumentoId))
            .ForMember(d => d.TipoDocumentoNombre, o => o.MapFrom(s => s.Persona.TipoDocumento != null ? s.Persona.TipoDocumento.Nombre : null))
            .ForMember(d => d.NumeroDocumento, o => o.MapFrom(s => s.Persona.NumeroDocumento))
            .ForMember(d => d.Nombres, o => o.MapFrom(s => s.Persona.Nombres))
            .ForMember(d => d.Apellidos, o => o.MapFrom(s => s.Persona.Apellidos))
            .ForMember(d => d.Nacionalidad, o => o.MapFrom(s => s.Persona.Nacionalidad))
            .ForMember(d => d.FechaNacimiento, o => o.MapFrom(s => s.Persona.FechaNacimiento))
            .ForMember(d => d.EmailContacto, o => o.MapFrom(s => s.Persona.EmailContacto))
            .ForMember(d => d.Telefono, o => o.MapFrom(s => s.Persona.Telefono));

        // Habitaciones
        CreateMap<TEdificio, Edificio>().ReverseMap();
        CreateMap<TTipoHabitacion, TiposHabitacion>().ReverseMap();
        CreateMap<TAmenidad, Amenidade>().ReverseMap();
        CreateMap<TFotoHabitacion, FotosHabitacion>().ReverseMap();
        CreateMap<TBloqueoHabitacion, BloqueosHabitacion>().ReverseMap();
        CreateMap<HistorialEstadosHabitacion, THistorialEstadoHabitacion>();

        CreateMap<ReglasTarifa, TReglaTarifa>()
            .ForMember(d => d.TipoHabitacionNombre, o => o.MapFrom(s => s.TipoHabitacion != null ? s.TipoHabitacion.Nombre : null));

        CreateMap<HabitacionAmenidade, TAmenidadHabitacion>()
            .ForMember(d => d.Nombre, o => o.MapFrom(s => s.Amenidad != null ? s.Amenidad.Nombre : null));

        CreateMap<Habitacione, THabitacion>()
            .ForMember(d => d.EdificioCodigo, o => o.MapFrom(s => s.Edificio != null ? s.Edificio.Codigo : null))
            .ForMember(d => d.TipoHabitacionNombre, o => o.MapFrom(s => s.TipoHabitacion != null ? s.TipoHabitacion.Nombre : null))
            .ForMember(d => d.EstadoNombre, o => o.MapFrom(s => ((EstadoHabitacion)s.Estado).ToString()))
            .ForMember(d => d.Amenidades, o => o.MapFrom(s => s.HabitacionAmenidades))
            .ForMember(d => d.Fotos, o => o.MapFrom(s => s.FotosHabitacions.OrderByDescending(f => f.EsPrincipal).ThenBy(f => f.Orden)));
    }
}
