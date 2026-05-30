using System.Text;
using Dapper;
using ServiceOrders.Api.Helpers;
using ServiceOrders.Api.Infrastructure;
using ServiceOrders.Api.Models.Entities;
using ServiceOrders.Api.Models.Orders;

namespace ServiceOrders.Api.Repositories;

public sealed class ServiceOrderRepository(IDbConnectionFactory connectionFactory) : IServiceOrderRepository
{
    private const string ActiveFilter = "o.deleted_at IS NULL";

    private const string ListSelect = """
        o.id AS Id,
        o.created_at AS CreatedAt,
        o.status::text AS Status,
        o.description AS Description,
        t.full_name AS TechnicianName,
        t.specialty AS TechnicianSpecialty,
        c.full_name AS ClientName,
        c.identity_doc AS ClientIdentityDoc
        """;

    private const string DetailSelect = """
        o.id AS Id,
        o.created_at AS CreatedAt,
        o.status::text AS Status,
        o.description AS Description,
        o.technician_id AS TechnicianId,
        t.full_name AS TechnicianName,
        t.specialty AS TechnicianSpecialty,
        o.client_id AS ClientId,
        c.full_name AS ClientName,
        c.identity_doc AS ClientIdentityDoc,
        o.updated_at AS UpdatedAt
        """;

    private const string OrderSelectColumns = """
        id AS Id,
        created_at AS CreatedAt,
        status::text AS Status,
        description AS Description,
        technician_id AS TechnicianId,
        client_id AS ClientId,
        updated_at AS UpdatedAt,
        deleted_at AS DeletedAt
        """;

    public async Task<IReadOnlyList<OrderListRow>> SearchAsync(
        OrderFilterQuery filter,
        CancellationToken cancellationToken = default)
    {
        var sql = new StringBuilder($"""
            SELECT {ListSelect}
            FROM service_orders o
            INNER JOIN technicians t ON t.id = o.technician_id
            INNER JOIN clients c ON c.id = o.client_id
            WHERE {ActiveFilter}
            """);

        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            sql.AppendLine(" AND o.status = @Status::order_status");
            parameters.Add("Status", filter.Status.Trim());
        }

        var technicianName = FilterTextHelper.ToLikePattern(filter.TechnicianName);
        if (technicianName is not null)
        {
            sql.AppendLine(" AND t.full_name ILIKE @TechnicianName");
            parameters.Add("TechnicianName", technicianName);
        }

        var technicianSpecialty = FilterTextHelper.ToLikePattern(filter.TechnicianSpecialty);
        if (technicianSpecialty is not null)
        {
            sql.AppendLine(" AND t.specialty ILIKE @TechnicianSpecialty");
            parameters.Add("TechnicianSpecialty", technicianSpecialty);
        }

        var clientName = FilterTextHelper.ToLikePattern(filter.ClientName);
        if (clientName is not null)
        {
            sql.AppendLine(" AND c.full_name ILIKE @ClientName");
            parameters.Add("ClientName", clientName);
        }

        var clientDocDigits = FilterTextHelper.ToDigitsOnly(filter.ClientIdentityDoc);
        if (clientDocDigits is not null)
        {
            sql.AppendLine("""
                 AND regexp_replace(c.identity_doc, '[^0-9]', '', 'g')
                     LIKE '%' || @ClientIdentityDocDigits || '%'
                """);
            parameters.Add("ClientIdentityDocDigits", clientDocDigits);
        }

        if (filter.CreatedFrom.HasValue)
        {
            sql.AppendLine(" AND o.created_at::date >= @CreatedFrom");
            parameters.Add("CreatedFrom", filter.CreatedFrom.Value);
        }

        if (filter.CreatedTo.HasValue)
        {
            sql.AppendLine(" AND o.created_at::date <= @CreatedTo");
            parameters.Add("CreatedTo", filter.CreatedTo.Value);
        }

        sql.AppendLine(" ORDER BY o.created_at DESC");

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var result = await connection.QueryAsync<OrderListRow>(
            new CommandDefinition(sql.ToString(), parameters, cancellationToken: cancellationToken));
        return result.AsList();
    }

    public async Task<OrderDetailRow?> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {DetailSelect}
            FROM service_orders o
            INNER JOIN technicians t ON t.id = o.technician_id
            INNER JOIN clients c ON c.id = o.client_id
            WHERE o.id = @Id AND {ActiveFilter}
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<OrderDetailRow>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<ServiceOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {OrderSelectColumns}
            FROM service_orders
            WHERE id = @Id AND {EntitySqlFilters.ActiveOnly}
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<ServiceOrder>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<ServiceOrder?> CreateAsync(ServiceOrder order, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO service_orders (description, technician_id, client_id, status)
            VALUES (@Description, @TechnicianId, @ClientId, @Status::order_status)
            RETURNING id AS Id, created_at AS CreatedAt, status::text AS Status,
                      description AS Description, technician_id AS TechnicianId,
                      client_id AS ClientId, updated_at AS UpdatedAt, deleted_at AS DeletedAt
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<ServiceOrder>(
            new CommandDefinition(sql, order, cancellationToken: cancellationToken));
    }

    public async Task<ServiceOrder?> UpdateAsync(ServiceOrder order, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            UPDATE service_orders
            SET description = @Description,
                technician_id = @TechnicianId,
                client_id = @ClientId,
                status = @Status::order_status,
                updated_at = NOW()
            WHERE id = @Id AND {EntitySqlFilters.ActiveOnly}
            RETURNING id AS Id, created_at AS CreatedAt, status::text AS Status,
                      description AS Description, technician_id AS TechnicianId,
                      client_id AS ClientId, updated_at AS UpdatedAt, deleted_at AS DeletedAt
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<ServiceOrder>(
            new CommandDefinition(sql, order, cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateStatusAsync(int id, string status, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            UPDATE service_orders
            SET status = @Status::order_status,
                updated_at = NOW()
            WHERE id = @Id AND {EntitySqlFilters.ActiveOnly}
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var affected = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id, Status = status }, cancellationToken: cancellationToken));
        return affected > 0;
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            UPDATE service_orders
            SET deleted_at = NOW(),
                updated_at = NOW()
            WHERE id = @Id AND {EntitySqlFilters.ActiveOnly}
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var affected = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        return affected > 0;
    }
}
