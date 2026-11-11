using Microsoft.EntityFrameworkCore;
using Veterinaria.Datos.Models;
using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

public class DuenoRepository : IDuenoRepository
{
    private readonly VeterinariaContext _context;

    public DuenoRepository(VeterinariaContext context)
    {
        _context = context;
    }

    public async Task<List<Dueno>> ObtenerTodosAsync(string? nombre, string? dni)
    {
        IQueryable<Dueno> consulta = _context.Duenos
            .AsNoTracking()
            .Include(d => d.Mascotas);

        if (!string.IsNullOrWhiteSpace(nombre))
            consulta = consulta.Where(d => d.Nombres.Contains(nombre));

        if (!string.IsNullOrWhiteSpace(dni))
            consulta = consulta.Where(d => d.Dni == dni);

        return await consulta.OrderBy(d => d.Nombres).ToListAsync();
    }

    public async Task<Dueno?> ObtenerPorIdAsync(int id)
    {
        return await _context.Duenos
            .Include(d => d.Mascotas)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<bool> ExisteDniAsync(string dni, int? excluirId = null)
    {
        return await _context.Duenos
            .AnyAsync(d => d.Dni == dni && (excluirId == null || d.Id != excluirId));
    }

    public async Task<bool> TieneMascotasAsync(int id)
    {
        return await _context.Mascotas.AnyAsync(m => m.DuenoId == id);
    }

    public async Task AgregarAsync(Dueno dueno)
    {
        _context.Duenos.Add(dueno);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Dueno dueno)
    {
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Dueno dueno)
    {
        _context.Duenos.Remove(dueno);
        await _context.SaveChangesAsync();
    }
}