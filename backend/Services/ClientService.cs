using ServiceOrders.Api.Helpers;
using ServiceOrders.Api.Models.Clients;
using ServiceOrders.Api.Models.Entities;
using ServiceOrders.Api.Repositories;

namespace ServiceOrders.Api.Services;

public sealed class ClientService(IClientRepository clientRepository) : IClientService
{
    public async Task<IReadOnlyList<ClientDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var clients = await clientRepository.GetAllAsync(cancellationToken);
        return clients.Select(MapToDto).ToList();
    }

    public async Task<ClientDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = await clientRepository.GetByIdAsync(id, cancellationToken);
        return client is null ? null : MapToDto(client);
    }

    public async Task<(ClientDto? Client, string? Error)> CreateAsync(
        CreateClientRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = ValidateRequest(request.IdentityDoc, request.Phone);
        if (validationError is not null)
        {
            return (null, validationError);
        }

        var identityDoc = request.IdentityDoc.Trim();

        if (await clientRepository.ExistsByIdentityDocAsync(identityDoc, cancellationToken: cancellationToken))
        {
            return (null, "Ya existe un cliente con ese documento de identidad.");
        }

        var entity = new Client
        {
            FullName = request.FullName.Trim(),
            IdentityDoc = identityDoc,
            Address = request.Address.Trim(),
            Phone = request.Phone.Trim()
        };

        var created = await clientRepository.CreateAsync(entity, cancellationToken);
        return created is null
            ? (null, "No se pudo crear el cliente.")
            : (MapToDto(created), null);
    }

    public async Task<(ClientDto? Client, string? Error)> UpdateAsync(
        int id,
        UpdateClientRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await clientRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return (null, "Cliente no encontrado.");
        }

        var validationError = ValidateRequest(request.IdentityDoc, request.Phone);
        if (validationError is not null)
        {
            return (null, validationError);
        }

        var identityDoc = request.IdentityDoc.Trim();

        if (await clientRepository.ExistsByIdentityDocAsync(identityDoc, id, cancellationToken))
        {
            return (null, "Ya existe otro cliente con ese documento de identidad.");
        }

        var entity = new Client
        {
            Id = id,
            FullName = request.FullName.Trim(),
            IdentityDoc = identityDoc,
            Address = request.Address.Trim(),
            Phone = request.Phone.Trim(),
            CreatedAt = existing.CreatedAt,
            UpdatedAt = existing.UpdatedAt
        };

        var updated = await clientRepository.UpdateAsync(entity, cancellationToken);
        return updated is null
            ? (null, "No se pudo actualizar el cliente.")
            : (MapToDto(updated), null);
    }

    public async Task<(bool Success, string? Error)> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await clientRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return (false, "Cliente no encontrado.");
        }

        var deleted = await clientRepository.SoftDeleteAsync(id, cancellationToken);
        return deleted ? (true, null) : (false, "No se pudo eliminar el cliente.");
    }

    private static string? ValidateRequest(string identityDoc, string phone)
    {
        if (string.IsNullOrWhiteSpace(identityDoc))
        {
            return "El documento de identidad es obligatorio.";
        }

        if (!PhoneValidator.IsValid(phone))
        {
            return "El teléfono no tiene un formato válido (7-20 dígitos, puede incluir +, espacios o guiones).";
        }

        return null;
    }

    private static ClientDto MapToDto(Client client) => new()
    {
        Id = client.Id,
        FullName = client.FullName,
        IdentityDoc = client.IdentityDoc,
        Address = client.Address,
        Phone = client.Phone,
        CreatedAt = client.CreatedAt,
        UpdatedAt = client.UpdatedAt
    };
}
