using ServiceOrders.Api.Helpers;
using ServiceOrders.Api.Models.Entities;
using ServiceOrders.Api.Models.Orders;
using ServiceOrders.Api.Repositories;

namespace ServiceOrders.Api.Services;

public sealed class ServiceOrderService(
    IServiceOrderRepository orderRepository,
    ITechnicianRepository technicianRepository,
    IClientRepository clientRepository) : IServiceOrderService
{
    public async Task<IReadOnlyList<OrderListItemDto>> SearchAsync(
        OrderFilterQuery filter,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (!OrderStatusHelper.IsValid(filter.Status))
            {
                return [];
            }

            filter.Status = OrderStatusHelper.Normalize(filter.Status);
        }

        var rows = await orderRepository.SearchAsync(filter, cancellationToken);
        return rows.Select(row => new OrderListItemDto
        {
            Id = row.Id,
            CreatedAt = row.CreatedAt,
            Status = row.Status,
            Description = row.Description,
            TechnicianName = row.TechnicianName,
            TechnicianSpecialty = row.TechnicianSpecialty,
            ClientName = row.ClientName,
            ClientIdentityDoc = row.ClientIdentityDoc
        }).ToList();
    }

    public async Task<OrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await orderRepository.GetDetailByIdAsync(id, cancellationToken);
        return row is null ? null : MapDetailToDto(row);
    }

    public async Task<(OrderDto? Order, string? Error)> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateAssignmentsAsync(
            request.TechnicianId,
            request.ClientId,
            OrderStatusHelper.Pendiente,
            cancellationToken);

        if (validationError is not null)
        {
            return (null, validationError);
        }

        if (string.IsNullOrWhiteSpace(request.Description) || request.Description.Trim().Length < 5)
        {
            return (null, "La descripción debe tener al menos 5 caracteres.");
        }

        var entity = new ServiceOrder
        {
            Description = request.Description.Trim(),
            TechnicianId = request.TechnicianId,
            ClientId = request.ClientId,
            Status = OrderStatusHelper.Pendiente
        };

        var created = await orderRepository.CreateAsync(entity, cancellationToken);
        if (created is null)
        {
            return (null, "No se pudo crear la orden.");
        }

        var detail = await orderRepository.GetDetailByIdAsync(created.Id, cancellationToken);
        return detail is null
            ? (null, "No se pudo obtener la orden creada.")
            : (MapDetailToDto(detail), null);
    }

    public async Task<(OrderDto? Order, string? Error)> UpdateAsync(
        int id,
        UpdateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await orderRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return (null, "Orden no encontrada.");
        }

        var validationError = await ValidateAssignmentsAsync(
            request.TechnicianId,
            request.ClientId,
            request.Status,
            cancellationToken);

        if (validationError is not null)
        {
            return (null, validationError);
        }

        if (string.IsNullOrWhiteSpace(request.Description) || request.Description.Trim().Length < 5)
        {
            return (null, "La descripción debe tener al menos 5 caracteres.");
        }

        var entity = new ServiceOrder
        {
            Id = id,
            Description = request.Description.Trim(),
            TechnicianId = request.TechnicianId,
            ClientId = request.ClientId,
            Status = OrderStatusHelper.Normalize(request.Status),
            CreatedAt = existing.CreatedAt,
            UpdatedAt = existing.UpdatedAt
        };

        var updated = await orderRepository.UpdateAsync(entity, cancellationToken);
        if (updated is null)
        {
            return (null, "No se pudo actualizar la orden.");
        }

        var detail = await orderRepository.GetDetailByIdAsync(id, cancellationToken);
        return detail is null
            ? (null, "No se pudo obtener la orden actualizada.")
            : (MapDetailToDto(detail), null);
    }

    public async Task<(bool Success, string? Error)> ChangeStatusAsync(
        int id,
        ChangeOrderStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await orderRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return (false, "Orden no encontrada.");
        }

        if (!OrderStatusHelper.IsValid(request.Status))
        {
            return (false, "El estado no es válido.");
        }

        var updated = await orderRepository.UpdateStatusAsync(
            id,
            OrderStatusHelper.Normalize(request.Status),
            cancellationToken);

        return updated ? (true, null) : (false, "No se pudo cambiar el estado.");
    }

    public async Task<(bool Success, string? Error)> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await orderRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return (false, "Orden no encontrada.");
        }

        var deleted = await orderRepository.SoftDeleteAsync(id, cancellationToken);
        return deleted ? (true, null) : (false, "No se pudo eliminar la orden.");
    }

    private async Task<string?> ValidateAssignmentsAsync(
        int technicianId,
        int clientId,
        string status,
        CancellationToken cancellationToken)
    {
        if (!OrderStatusHelper.IsValid(status))
        {
            return "El estado no es válido.";
        }

        if (!await technicianRepository.ExistsActiveAsync(technicianId, cancellationToken))
        {
            return "El técnico seleccionado no existe o no está activo.";
        }

        if (!await clientRepository.ExistsActiveAsync(clientId, cancellationToken))
        {
            return "El cliente seleccionado no existe o no está activo.";
        }

        return null;
    }

    private static OrderDto MapDetailToDto(OrderDetailRow row) => new()
    {
        Id = row.Id,
        CreatedAt = row.CreatedAt,
        Status = row.Status,
        Description = row.Description,
        TechnicianId = row.TechnicianId,
        TechnicianName = row.TechnicianName,
        TechnicianSpecialty = row.TechnicianSpecialty,
        ClientId = row.ClientId,
        ClientName = row.ClientName,
        ClientIdentityDoc = row.ClientIdentityDoc,
        UpdatedAt = row.UpdatedAt
    };
}
