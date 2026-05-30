using ServiceOrders.Api.Models.Orders;

namespace ServiceOrders.Api.Services;

public interface IServiceOrderService
{
    Task<IReadOnlyList<OrderListItemDto>> SearchAsync(OrderFilterQuery filter, CancellationToken cancellationToken = default);
    Task<OrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(OrderDto? Order, string? Error)> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<(OrderDto? Order, string? Error)> UpdateAsync(int id, UpdateOrderRequest request, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> ChangeStatusAsync(int id, ChangeOrderStatusRequest request, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}
