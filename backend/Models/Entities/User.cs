namespace ServiceOrders.Api.Models.Entities;

public sealed class User
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
}
