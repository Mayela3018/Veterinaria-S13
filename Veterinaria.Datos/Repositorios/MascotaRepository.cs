using Microsoft.EntityFrameworkCore;
using Veterinaria.Datos.Models;
using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

public class MascotaRepository : IMascotaRepository
{
    private readonly VeterinariaContext _context;

    public MascotaRepository(VeterinariaContext context)
    {
        _context = context;
    }

    public async Task<List<Mascota>> ObtenerTodasAsync(string? especie, string? nombre, int? duenoId)
    {
        IQueryable<Mascota> consulta = _context.Mascotas
            .AsNoTracking()
            .Include(m => m.Dueno);

        if (!string.IsNullOrWhiteSpace(especie))
            consulta = consulta.Where(m => m.Especie == especie);

        if (!string.IsNullOrWhiteSpace(nombre))
            consulta = consulta.Where(m => m.Nombre.Contains(nombre));

        if (duenoId.HasValue)
            consulta = consulta.Where(m => m.DuenoId == duenoId.Value);

        return await consulta.OrderBy(m => m.Nombre).ToListAsync();
    }

    public async Task<List<Mascota>> ObtenerPorDuenoAsync(int duenoId)
    {
        return await _context.Mascotas
            .AsNoTracking()
            .Include(m => m.Dueno)
            .Where(m => m.DuenoId == duenoId)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public async Task<Mascota?> ObtenerPorIdAsync(int id)
    {
        return await _context.Mascotas
            .Include(m => m.Dueno)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<bool> ExisteDuplicadoAsync(string nombre, string especie, int duenoId, int? excluirId = null)
    {
        return await _context.Mascotas.AnyAsync(m =>
            m.DuenoId == duenoId &&
            m.Nombre == nombre &&
            m.Especie == especie &&
            (excluirId == null || m.Id != excluirId));
    }

    public async Task AgregarAsync(Mascota mascota)
    {
        _context.Mascotas.Add(mascota);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Mascota mascota)
    {
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Mascota mascota)
    {
        _context.Mascotas.Remove(mascota);
        await _context.SaveChangesAsync();
    }
}