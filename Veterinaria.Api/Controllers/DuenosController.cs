using Microsoft.AspNetCore.Mvc;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;
using Veterinaria.Negocio.Servicios;

namespace Veterinaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DuenosController : ControllerBase
{
    private readonly IDuenoService _duenoService;
    private readonly IMascotaService _mascotaService;

    public DuenosController(IDuenoService duenoService, IMascotaService mascotaService)
    {
        _duenoService = duenoService;
        _mascotaService = mascotaService;
    }

    // GET api/duenos?nombre=...&dni=...
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DuenoDto>>> Get([FromQuery] string? nombre, [FromQuery] string? dni)
    {
        var duenos = await _duenoService.ObtenerTodosAsync(nombre, dni);
        return Ok(duenos);
    }

    // GET api/duenos/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DuenoDto>> GetPorId(int id)
    {
        var dueno = await _duenoService.ObtenerPorIdAsync(id);
        if (dueno == null)
            return NotFound();

        return Ok(dueno);
    }

    // GET api/duenos/1/mascotas  (ruta anidada)
    [HttpGet("{id:int}/mascotas")]
    public async Task<ActionResult<IEnumerable<MascotaDto>>> GetMascotas(int id)
    {
        var mascotas = await _mascotaService.ObtenerPorDuenoAsync(id);
        if (mascotas == null)
            return NotFound();

        return Ok(mascotas);
    }

    // POST api/duenos
    [HttpPost]
    public async Task<ActionResult<DuenoDto>> Post([FromBody] DuenoCrearDto dto)
    {
        try
        {
            var creado = await _duenoService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetPorId), new { id = creado.Id }, creado);
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

    // PUT api/duenos/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] DuenoActualizarDto dto)
    {
        try
        {
            var actualizado = await _duenoService.ActualizarAsync(id, dto);
            if (!actualizado)
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

    // DELETE api/duenos/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var eliminado = await _duenoService.EliminarAsync(id);
            if (!eliminado)
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
}