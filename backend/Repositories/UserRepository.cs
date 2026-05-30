using Dapper;
using ServiceOrders.Api.Infrastructure;
using ServiceOrders.Api.Models.Entities;

namespace ServiceOrders.Api.Repositories;

public sealed class UserRepository(IDbConnectionFactory connectionFactory) : IUserRepository
{
    public async Task<User?> ValidateCredentialsAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT id AS Id, username AS Username, full_name AS FullName
            FROM users
            WHERE username = @Username
              AND is_active = TRUE
              AND password_hash = crypt(@Password, password_hash)
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<User>(
            new CommandDefinition(sql, new { Username = username, Password = password }, cancellationToken: cancellationToken));
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT id AS Id, username AS Username, full_name AS FullName
            FROM users
            WHERE id = @Id AND is_active = TRUE
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<User>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}
