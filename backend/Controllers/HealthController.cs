using Dapper;
using Microsoft.AspNetCore.Mvc;
using ServiceOrders.Api.Infrastructure;

namespace ServiceOrders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController(IDbConnectionFactory connectionFactory) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        var dbTime = await connection.QuerySingleAsync<DateTime>(
            "SELECT NOW() AT TIME ZONE 'UTC'");

        return Ok(new
        {
            status = "ok",
            database = "connected",
            serverTimeUtc = dbTime
        });
    }
}
