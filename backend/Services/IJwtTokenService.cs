using ServiceOrders.Api.Models.Entities;

namespace ServiceOrders.Api.Services;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(User user);
}
