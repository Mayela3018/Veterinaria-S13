using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

public interface IDuenoRepository
{
    Task<List<Dueno>> ObtenerTodosAsync(string? nombre, string? dni);
    Task<Dueno?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteDniAsync(string dni, int? excluirId = null);
    Task<bool> TieneMascotasAsync(int id);
    Task AgregarAsync(Dueno dueno);
    Task ActualizarAsync(Dueno dueno);
    Task EliminarAsync(Dueno dueno);
}