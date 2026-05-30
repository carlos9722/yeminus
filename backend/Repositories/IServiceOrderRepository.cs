using ServiceOrders.Api.Models.Entities;
using ServiceOrders.Api.Models.Orders;

namespace ServiceOrders.Api.Repositories;

public interface IServiceOrderRepository
{
    Task<IReadOnlyList<OrderListRow>> SearchAsync(OrderFilterQuery filter, CancellationToken cancellationToken = default);
    Task<OrderDetailRow?> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceOrder?> CreateAsync(ServiceOrder order, CancellationToken cancellationToken = default);
    Task<ServiceOrder?> UpdateAsync(ServiceOrder order, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(int id, string status, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}

public sealed class OrderListRow
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string TechnicianName { get; init; } = string.Empty;
    public string TechnicianSpecialty { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public string ClientIdentityDoc { get; init; } = string.Empty;
}

public sealed class OrderDetailRow
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int TechnicianId { get; init; }
    public string TechnicianName { get; init; } = string.Empty;
    public string TechnicianSpecialty { get; init; } = string.Empty;
    public int ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string ClientIdentityDoc { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; }
}
