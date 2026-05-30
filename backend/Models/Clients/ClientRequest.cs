using System.ComponentModel.DataAnnotations;

namespace ServiceOrders.Api.Models.Clients;

public sealed class CreateClientRequest
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El documento de identidad es obligatorio.")]
    [MaxLength(30)]
    public string IdentityDoc { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [MaxLength(250)]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
}

public sealed class UpdateClientRequest
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El documento de identidad es obligatorio.")]
    [MaxLength(30)]
    public string IdentityDoc { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [MaxLength(250)]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
}
