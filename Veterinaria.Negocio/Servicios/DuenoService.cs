using AutoMapper;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;

namespace Veterinaria.Negocio.Servicios;

public class DuenoService : IDuenoService
{
    private readonly IDuenoRepository _repositorio;
    private readonly IMapper _mapper;

    public DuenoService(IDuenoRepository repositorio, IMapper mapper)
    {
        _repositorio = repositorio;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DuenoDto>> ObtenerTodosAsync(string? nombre, string? dni)
    {
        var duenos = await _repositorio.ObtenerTodosAsync(nombre, dni);
        return _mapper.Map<IEnumerable<DuenoDto>>(duenos);
    }

    public async Task<DuenoDto?> ObtenerPorIdAsync(int id)
    {
        var dueno = await _repositorio.ObtenerPorIdAsync(id);
        return dueno == null ? null : _mapper.Map<DuenoDto>(dueno);
    }

    public async Task<DuenoDto> CrearAsync(DuenoCrearDto dto)
    {
        // Regla de negocio 1: el DNI no se puede repetir
        if (await _repositorio.ExisteDniAsync(dto.Dni))
            throw new ReglaNegocioException($"Ya existe un dueño registrado con el DNI {dto.Dni}.");

        var dueno = _mapper.Map<Dueno>(dto);
        await _repositorio.AgregarAsync(dueno);
        return _mapper.Map<DuenoDto>(dueno);
    }

    public async Task<bool> ActualizarAsync(int id, DuenoActualizarDto dto)
    {
        var existente = await _repositorio.ObtenerPorIdAsync(id);
        if (existente == null)
            return false;

        if (await _repositorio.ExisteDniAsync(dto.Dni, id))
            throw new ReglaNegocioException($"Ya existe otro dueño registrado con el DNI {dto.Dni}.");

        _mapper.Map(dto, existente);
        await _repositorio.ActualizarAsync(existente);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var existente = await _repositorio.ObtenerPorIdAsync(id);
        if (existente == null)
            return false;

        // Regla de negocio 2: no se elimina un dueño que tiene mascotas
        if (await _repositorio.TieneMascotasAsync(id))
            throw new ReglaNegocioException("No se puede eliminar al dueño porque tiene mascotas registradas.");

        await _repositorio.EliminarAsync(existente);
        return true;
    }
}