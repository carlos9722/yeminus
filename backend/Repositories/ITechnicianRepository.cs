using ServiceOrders.Api.Models.Entities;

namespace ServiceOrders.Api.Repositories;

public interface ITechnicianRepository
{
    Task<IReadOnlyList<Technician>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Technician?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsActiveAsync(int id, CancellationToken cancellationToken = default);
    Task<Technician?> CreateAsync(Technician technician, CancellationToken cancellationToken = default);
    Task<Technician?> UpdateAsync(Technician technician, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}
