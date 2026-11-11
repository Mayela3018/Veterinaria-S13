using Microsoft.AspNetCore.Mvc;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;
using Veterinaria.Negocio.Servicios;

namespace Veterinaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MascotasController : ControllerBase
{
    private readonly IMascotaService _mascotaService;

    public MascotasController(IMascotaService mascotaService)
    {
        _mascotaService = mascotaService;
    }

    // GET api/mascotas?especie=...&nombre=...&duenoId=...
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MascotaDto>>> Get(
        [FromQuery] string? especie,
        [FromQuery] string? nombre,
        [FromQuery] int? duenoId)
    {
        var mascotas = await _mascotaService.ObtenerTodasAsync(especie, nombre, duenoId);
        return Ok(mascotas);
    }

    // GET api/mascotas/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MascotaDto>> GetPorId(int id)
    {
        var mascota = await _mascotaService.ObtenerPorIdAsync(id);
        if (mascota == null)
            return NotFound();

        return Ok(mascota);
    }

    // POST api/mascotas
    [HttpPost]
    public async Task<ActionResult<MascotaDto>> Post([FromBody] MascotaCrearDto dto)
    {
        try
        {
            var creada = await _mascotaService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetPorId), new { id = creada.Id }, creada);
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Regla de negocio incumplida",
                Detail = ex.Message
            });
        }
    }

    // PUT api/mascotas/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] MascotaActualizarDto dto)
    {
        try
        {
            var actualizada = await _mascotaService.ActualizarAsync(id, dto);
            if (!actualizada)
                return NotFound();

            return NoContent();
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Regla de negocio incumplida",
                Detail = ex.Message
            });
        }
    }

    // DELETE api/mascotas/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var eliminada = await _mascotaService.EliminarAsync(id);
        if (!eliminada)
            return NotFound();

        return NoContent();
    }
}