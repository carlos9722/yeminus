namespace ServiceOrders.Api.Models.Entities;

public sealed class ServiceOrder
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int TechnicianId { get; init; }
    public int ClientId { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
}
