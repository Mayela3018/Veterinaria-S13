using AutoMapper;
using Veterinaria.Entidades;
using Veterinaria.Entidades.Dtos;

namespace Veterinaria.Negocio.Mapeos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entidad -> DTO de lectura (lo que la API devuelve)
        CreateMap<Dueno, DuenoDto>()
            .ForMember(d => d.CantidadMascotas, o => o.MapFrom(s => s.Mascotas.Count));

        // DTO de entrada -> Entidad (lo que llega del cliente)
        CreateMap<DuenoCrearDto, Dueno>();
        CreateMap<DuenoActualizarDto, Dueno>();

        CreateMap<Mascota, MascotaDto>()
            .ForMember(d => d.DuenoNombre, o => o.MapFrom(s => s.Dueno.Nombres));

        CreateMap<MascotaCrearDto, Mascota>();
        CreateMap<MascotaActualizarDto, Mascota>();
    }
}