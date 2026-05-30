using ServiceOrders.Api.Models.Entities;

namespace ServiceOrders.Api.Repositories;

public interface IUserRepository
{
    Task<User?> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
