using Veterinaria.Entidades.Dtos;

namespace Veterinaria.Negocio.Servicios;

public interface IDuenoService
{
    Task<IEnumerable<DuenoDto>> ObtenerTodosAsync(string? nombre, string? dni);
    Task<DuenoDto?> ObtenerPorIdAsync(int id);
    Task<DuenoDto> CrearAsync(DuenoCrearDto dto);
    Task<bool> ActualizarAsync(int id, DuenoActualizarDto dto);
    Task<bool> EliminarAsync(int id);
}