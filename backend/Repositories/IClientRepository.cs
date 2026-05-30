using ServiceOrders.Api.Models.Entities;

namespace ServiceOrders.Api.Repositories;

public interface IClientRepository
{
    Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdentityDocAsync(string identityDoc, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<Client?> CreateAsync(Client client, CancellationToken cancellationToken = default);
    Task<Client?> UpdateAsync(Client client, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}
