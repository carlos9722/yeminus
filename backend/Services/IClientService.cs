using ServiceOrders.Api.Models.Clients;

namespace ServiceOrders.Api.Services;

public interface IClientService
{
    Task<IReadOnlyList<ClientDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ClientDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(ClientDto? Client, string? Error)> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken = default);
    Task<(ClientDto? Client, string? Error)> UpdateAsync(int id, UpdateClientRequest request, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}
