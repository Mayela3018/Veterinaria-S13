namespace Veterinaria.Entidades.Dtos;

public class DuenoDto
{
    public int Id { get; set; }

    public string Nombres { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string? Email { get; set; }

    public int CantidadMascotas { get; set; }
}