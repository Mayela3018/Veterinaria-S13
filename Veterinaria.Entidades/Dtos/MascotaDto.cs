namespace Veterinaria.Entidades.Dtos;

public class MascotaDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Especie { get; set; } = null!;

    public string? Raza { get; set; }

    public int Edad { get; set; }

    public decimal Peso { get; set; }

    public int DuenoId { get; set; }

    public string DuenoNombre { get; set; } = null!;
}