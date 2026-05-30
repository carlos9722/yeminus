using Npgsql;

namespace ServiceOrders.Api.Infrastructure;

public interface IDbConnectionFactory
{
    Task<NpgsqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
