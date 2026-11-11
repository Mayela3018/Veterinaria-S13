using AutoMapper;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;

namespace Veterinaria.Negocio.Servicios;

public class MascotaService : IMascotaService
{
    private readonly IMascotaRepository _repositorio;
    private readonly IDuenoRepository _duenoRepositorio;
    private readonly IMapper _mapper;

    public MascotaService(IMascotaRepository repositorio, IDuenoRepository duenoRepositorio, IMapper mapper)
    {
        _repositorio = repositorio;
        _duenoRepositorio = duenoRepositorio;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MascotaDto>> ObtenerTodasAsync(string? especie, string? nombre, int? duenoId)
    {
        var mascotas = await _repositorio.ObtenerTodasAsync(especie, nombre, duenoId);
        return _mapper.Map<IEnumerable<MascotaDto>>(mascotas);
    }

    public async Task<IEnumerable<MascotaDto>?> ObtenerPorDuenoAsync(int duenoId)
    {
        var dueno = await _duenoRepositorio.ObtenerPorIdAsync(duenoId);
        if (dueno == null)
            return null;

        var mascotas = await _repositorio.ObtenerPorDuenoAsync(duenoId);
        return _mapper.Map<IEnumerable<MascotaDto>>(mascotas);
    }

    public async Task<MascotaDto?> ObtenerPorIdAsync(int id)
    {
        var mascota = await _repositorio.ObtenerPorIdAsync(id);
        return mascota == null ? null : _mapper.Map<MascotaDto>(mascota);
    }

    public async Task<MascotaDto> CrearAsync(MascotaCrearDto dto)
    {
        // Regla de negocio 3: la mascota debe pertenecer a un dueño que exista
        var dueno = await _duenoRepositorio.ObtenerPorIdAsync(dto.DuenoId);
        if (dueno == null)
            throw new ReglaNegocioException($"No existe un dueño con el Id {dto.DuenoId}.");

        // Regla de negocio 4: un dueño no puede tener dos mascotas con el mismo nombre y especie
        if (await _repositorio.ExisteDuplicadoAsync(dto.Nombre, dto.Especie, dto.DuenoId))
            throw new ReglaNegocioException($"El dueño ya tiene una mascota llamada {dto.Nombre} de la especie {dto.Especie}.");

        var mascota = _mapper.Map<Mascota>(dto);
        await _repositorio.AgregarAsync(mascota);

        var creada = await _repositorio.ObtenerPorIdAsync(mascota.Id);
        return _mapper.Map<MascotaDto>(creada);
    }

    public async Task<bool> ActualizarAsync(int id, MascotaActualizarDto dto)
    {
        var existente = await _repositorio.ObtenerPorIdAsync(id);
        if (existente == null)
            return false;

        var dueno = await _duenoRepositorio.ObtenerPorIdAsync(dto.DuenoId);
        if (dueno == null)
            throw new ReglaNegocioException($"No existe un dueño con el Id {dto.DuenoId}.");

        if (await _repositorio.ExisteDuplicadoAsync(dto.Nombre, dto.Especie, dto.DuenoId, id))
            throw new ReglaNegocioException($"El dueño ya tiene otra mascota llamada {dto.Nombre} de la especie {dto.Especie}.");

        _mapper.Map(dto, existente);
        await _repositorio.ActualizarAsync(existente);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var existente = await _repositorio.ObtenerPorIdAsync(id);
        if (existente == null)
            return false;

        await _repositorio.EliminarAsync(existente);
        return true;
    }
}