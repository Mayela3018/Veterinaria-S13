using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

public interface IMascotaRepository
{
    Task<List<Mascota>> ObtenerTodasAsync(string? especie, string? nombre, int? duenoId);
    Task<List<Mascota>> ObtenerPorDuenoAsync(int duenoId);
    Task<Mascota?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteDuplicadoAsync(string nombre, string especie, int duenoId, int? excluirId = null);
    Task AgregarAsync(Mascota mascota);
    Task ActualizarAsync(Mascota mascota);
    Task EliminarAsync(Mascota mascota);
}