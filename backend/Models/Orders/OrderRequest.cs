using System.ComponentModel.DataAnnotations;

namespace ServiceOrders.Api.Models.Orders;

public sealed class CreateOrderRequest
{
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [MinLength(5)]
    public string Description { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un técnico válido.")]
    public int TechnicianId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un cliente válido.")]
    public int ClientId { get; set; }
}

public sealed class UpdateOrderRequest
{
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [MinLength(5)]
    public string Description { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un técnico válido.")]
    public int TechnicianId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un cliente válido.")]
    public int ClientId { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio.")]
    public string Status { get; set; } = string.Empty;
}

public sealed class ChangeOrderStatusRequest
{
    [Required(ErrorMessage = "El estado es obligatorio.")]
    public string Status { get; set; } = string.Empty;
}
