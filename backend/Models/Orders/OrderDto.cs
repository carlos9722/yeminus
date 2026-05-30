namespace ServiceOrders.Api.Models.Orders;

public sealed class OrderDto
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int TechnicianId { get; init; }
    public string TechnicianName { get; init; } = string.Empty;
    public string TechnicianSpecialty { get; init; } = string.Empty;
    public int ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string ClientIdentityDoc { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; }
}

public sealed class OrderListItemDto
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string TechnicianName { get; init; } = string.Empty;
    public string TechnicianSpecialty { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public string ClientIdentityDoc { get; init; } = string.Empty;
}
