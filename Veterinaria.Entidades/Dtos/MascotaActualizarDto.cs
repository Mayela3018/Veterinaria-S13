using System.ComponentModel.DataAnnotations;

namespace Veterinaria.Entidades.Dtos;

public class MascotaActualizarDto
{
    [Required(ErrorMessage = "El nombre de la mascota es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La especie es obligatoria.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "La especie debe tener entre 3 y 30 caracteres.")]
    public string Especie { get; set; } = null!;

    [StringLength(50, ErrorMessage = "La raza no puede superar los 50 caracteres.")]
    public string? Raza { get; set; }

    [Range(0, 40, ErrorMessage = "La edad debe estar entre 0 y 40 años.")]
    public int Edad { get; set; }

    [Range(0.01, 999.99, ErrorMessage = "El peso debe ser mayor a 0 y menor o igual a 999.99 kg.")]
    public decimal Peso { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar el dueño de la mascota.")]
    public int DuenoId { get; set; }
}