using Veterinaria.Entidades.Dtos;

namespace Veterinaria.Negocio.Servicios;

public interface IMascotaService
{
    Task<IEnumerable<MascotaDto>> ObtenerTodasAsync(string? especie, string? nombre, int? duenoId);
    Task<IEnumerable<MascotaDto>?> ObtenerPorDuenoAsync(int duenoId);
    Task<MascotaDto?> ObtenerPorIdAsync(int id);
    Task<MascotaDto> CrearAsync(MascotaCrearDto dto);
    Task<bool> ActualizarAsync(int id, MascotaActualizarDto dto);
    Task<bool> EliminarAsync(int id);
}