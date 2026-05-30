using ServiceOrders.Api.Models.Technicians;

namespace ServiceOrders.Api.Services;

public interface ITechnicianService
{
    Task<IReadOnlyList<TechnicianDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TechnicianDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(TechnicianDto? Technician, string? Error)> CreateAsync(
        CreateTechnicianRequest request,
        CancellationToken cancellationToken = default);
    Task<(TechnicianDto? Technician, string? Error)> UpdateAsync(
        int id,
        UpdateTechnicianRequest request,
        CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}
