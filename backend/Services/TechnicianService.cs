using ServiceOrders.Api.Helpers;
using ServiceOrders.Api.Models.Entities;
using ServiceOrders.Api.Models.Technicians;
using ServiceOrders.Api.Repositories;

namespace ServiceOrders.Api.Services;

public sealed class TechnicianService(ITechnicianRepository technicianRepository) : ITechnicianService
{
    public async Task<IReadOnlyList<TechnicianDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var technicians = await technicianRepository.GetAllAsync(cancellationToken);
        return technicians.Select(MapToDto).ToList();
    }

    public async Task<TechnicianDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var technician = await technicianRepository.GetByIdAsync(id, cancellationToken);
        return technician is null ? null : MapToDto(technician);
    }

    public async Task<(TechnicianDto? Technician, string? Error)> CreateAsync(
        CreateTechnicianRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = ValidateRequest(request.Phone, request.Specialty);
        if (validationError is not null)
        {
            return (null, validationError);
        }

        var entity = new Technician
        {
            FullName = request.FullName.Trim(),
            Phone = request.Phone.Trim(),
            Specialty = request.Specialty.Trim()
        };

        var created = await technicianRepository.CreateAsync(entity, cancellationToken);
        return created is null
            ? (null, "No se pudo crear el técnico.")
            : (MapToDto(created), null);
    }

    public async Task<(TechnicianDto? Technician, string? Error)> UpdateAsync(
        int id,
        UpdateTechnicianRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await technicianRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return (null, "Técnico no encontrado.");
        }

        var validationError = ValidateRequest(request.Phone, request.Specialty);
        if (validationError is not null)
        {
            return (null, validationError);
        }

        var entity = new Technician
        {
            Id = id,
            FullName = request.FullName.Trim(),
            Phone = request.Phone.Trim(),
            Specialty = request.Specialty.Trim(),
            CreatedAt = existing.CreatedAt,
            UpdatedAt = existing.UpdatedAt
        };

        var updated = await technicianRepository.UpdateAsync(entity, cancellationToken);
        return updated is null
            ? (null, "No se pudo actualizar el técnico.")
            : (MapToDto(updated), null);
    }

    public async Task<(bool Success, string? Error)> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await technicianRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return (false, "Técnico no encontrado.");
        }

        var deleted = await technicianRepository.SoftDeleteAsync(id, cancellationToken);
        return deleted ? (true, null) : (false, "No se pudo eliminar el técnico.");
    }

    private static string? ValidateRequest(string phone, string specialty)
    {
        if (string.IsNullOrWhiteSpace(specialty))
        {
            return "La especialidad es obligatoria.";
        }

        if (!PhoneValidator.IsValid(phone))
        {
            return "El teléfono no tiene un formato válido (7-20 dígitos, puede incluir +, espacios o guiones).";
        }

        return null;
    }

    private static TechnicianDto MapToDto(Technician technician) => new()
    {
        Id = technician.Id,
        FullName = technician.FullName,
        Phone = technician.Phone,
        Specialty = technician.Specialty,
        CreatedAt = technician.CreatedAt,
        UpdatedAt = technician.UpdatedAt
    };
}
