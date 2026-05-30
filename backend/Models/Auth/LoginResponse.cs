namespace ServiceOrders.Api.Models.Auth;

public sealed class LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
    public UserProfileDto User { get; init; } = new();
}
