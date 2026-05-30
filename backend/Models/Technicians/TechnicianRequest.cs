using System.ComponentModel.DataAnnotations;

namespace ServiceOrders.Api.Models.Technicians;

public sealed class CreateTechnicianRequest
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "La especialidad es obligatoria.")]
    [MaxLength(100)]
    public string Specialty { get; set; } = string.Empty;
}

public sealed class UpdateTechnicianRequest
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "La especialidad es obligatoria.")]
    [MaxLength(100)]
    public string Specialty { get; set; } = string.Empty;
}
