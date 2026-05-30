using Dapper;
using ServiceOrders.Api.Infrastructure;
using ServiceOrders.Api.Models.Entities;

namespace ServiceOrders.Api.Repositories;

public sealed class ClientRepository(IDbConnectionFactory connectionFactory) : IClientRepository
{
    private const string ActiveFilter = EntitySqlFilters.ActiveOnly;

    private const string SelectColumns = """
        id AS Id,
        full_name AS FullName,
        identity_doc AS IdentityDoc,
        address AS Address,
        phone AS Phone,
        created_at AS CreatedAt,
        updated_at AS UpdatedAt,
        deleted_at AS DeletedAt
        """;

    public async Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            FROM clients
            WHERE {ActiveFilter}
            ORDER BY full_name
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var result = await connection.QueryAsync<Client>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        return result.AsList();
    }

    public async Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            FROM clients
            WHERE id = @Id AND {ActiveFilter}
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Client>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<bool> ExistsByIdentityDocAsync(
        string identityDoc,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT EXISTS(
                SELECT 1 FROM clients
                WHERE identity_doc = @IdentityDoc
                  AND {ActiveFilter}
                  AND (@ExcludeId IS NULL OR id <> @ExcludeId)
            )
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleAsync<bool>(
            new CommandDefinition(sql, new { IdentityDoc = identityDoc, ExcludeId = excludeId }, cancellationToken: cancellationToken));
    }

    public async Task<Client?> CreateAsync(Client client, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO clients (full_name, identity_doc, address, phone)
            VALUES (@FullName, @IdentityDoc, @Address, @Phone)
            RETURNING id AS Id, full_name AS FullName, identity_doc AS IdentityDoc,
                      address AS Address, phone AS Phone,
                      created_at AS CreatedAt, updated_at AS UpdatedAt,
                      deleted_at AS DeletedAt
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Client>(
            new CommandDefinition(sql, client, cancellationToken: cancellationToken));
    }

    public async Task<Client?> UpdateAsync(Client client, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            UPDATE clients
            SET full_name = @FullName,
                identity_doc = @IdentityDoc,
                address = @Address,
                phone = @Phone,
                updated_at = NOW()
            WHERE id = @Id AND {ActiveFilter}
            RETURNING id AS Id, full_name AS FullName, identity_doc AS IdentityDoc,
                      address AS Address, phone AS Phone,
                      created_at AS CreatedAt, updated_at AS UpdatedAt,
                      deleted_at AS DeletedAt
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Client>(
            new CommandDefinition(sql, client, cancellationToken: cancellationToken));
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            UPDATE clients
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
