namespace ServiceOrders.Api.Models.Clients;

public sealed class ClientDto
{
    public int Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string IdentityDoc { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
