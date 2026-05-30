using Dapper;
using ServiceOrders.Api.Infrastructure;
using ServiceOrders.Api.Models.Entities;

namespace ServiceOrders.Api.Repositories;

public sealed class TechnicianRepository(IDbConnectionFactory connectionFactory) : ITechnicianRepository
{
    private const string ActiveFilter = EntitySqlFilters.ActiveOnly;

    private const string SelectColumns = """
        id AS Id,
        full_name AS FullName,
        phone AS Phone,
        specialty AS Specialty,
        created_at AS CreatedAt,
        updated_at AS UpdatedAt,
        deleted_at AS DeletedAt
        """;

    public async Task<IReadOnlyList<Technician>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            FROM technicians
            WHERE {ActiveFilter}
            ORDER BY full_name
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var result = await connection.QueryAsync<Technician>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        return result.AsList();
    }

    public async Task<Technician?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            FROM technicians
            WHERE id = @Id AND {ActiveFilter}
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Technician>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<bool> ExistsActiveAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT EXISTS(
                SELECT 1 FROM technicians
                WHERE id = @Id AND {ActiveFilter}
            )
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleAsync<bool>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<Technician?> CreateAsync(Technician technician, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO technicians (full_name, phone, specialty)
            VALUES (@FullName, @Phone, @Specialty)
            RETURNING id AS Id, full_name AS FullName, phone AS Phone, specialty AS Specialty,
                      created_at AS CreatedAt, updated_at AS UpdatedAt, deleted_at AS DeletedAt
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Technician>(
            new CommandDefinition(sql, technician, cancellationToken: cancellationToken));
    }

    public async Task<Technician?> UpdateAsync(Technician technician, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            UPDATE technicians
            SET full_name = @FullName,
                phone = @Phone,
                specialty = @Specialty,
                updated_at = NOW()
            WHERE id = @Id AND {ActiveFilter}
            RETURNING id AS Id, full_name AS FullName, phone AS Phone, specialty AS Specialty,
                      created_at AS CreatedAt, updated_at AS UpdatedAt, deleted_at AS DeletedAt
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Technician>(
            new CommandDefinition(sql, technician, cancellationToken: cancellationToken));
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            UPDATE technicians
            SET deleted_at = NOW(),
                updated_at = NOW()
            WHERE id = @Id AND {ActiveFilter}
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var affected = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        return affected > 0;
    }
}
