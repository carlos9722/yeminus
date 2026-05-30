namespace ServiceOrders.Api.Models.Auth;

public sealed class UserProfileDto
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
}
