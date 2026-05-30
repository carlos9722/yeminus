using ServiceOrders.Api.Models.Auth;
using ServiceOrders.Api.Repositories;

namespace ServiceOrders.Api.Services;

public sealed class AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.ValidateCredentialsAsync(
            request.Username.Trim(),
            request.Password,
            cancellationToken);

        if (user is null)
        {
            return null;
        }

        var (token, expiresAtUtc) = jwtTokenService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            User = MapToProfile(user)
        };
    }

    public async Task<UserProfileDto?> GetProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        return user is null ? null : MapToProfile(user);
    }

    private static UserProfileDto MapToProfile(Models.Entities.User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        FullName = user.FullName
    };
}
