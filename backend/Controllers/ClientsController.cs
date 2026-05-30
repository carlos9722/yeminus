using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceOrders.Api.Models.Clients;
using ServiceOrders.Api.Services;

namespace ServiceOrders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ClientsController(IClientService clientService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var clients = await clientService.GetAllAsync(cancellationToken);
        return Ok(clients);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var client = await clientService.GetByIdAsync(id, cancellationToken);
        return client is null ? NotFound(new { message = "Cliente no encontrado." }) : Ok(client);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (client, error) = await clientService.CreateAsync(request, cancellationToken);

        if (error is not null)
        {
            return ToErrorResult(error);
        }

        return CreatedAtAction(nameof(GetById), new { id = client!.Id }, client);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClientRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (client, error) = await clientService.UpdateAsync(id, request, cancellationToken);

        if (error is null)
        {
            return Ok(client);
        }

        return ToErrorResult(error);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var (success, error) = await clientService.SoftDeleteAsync(id, cancellationToken);

        if (success)
        {
            return NoContent();
        }

        return ToErrorResult(error!);
    }

    private static IActionResult ToErrorResult(string error)
    {
        if (error == "Cliente no encontrado.")
        {
            return new NotFoundObjectResult(new { message = error });
        }

        if (error.Contains("formato", StringComparison.OrdinalIgnoreCase)
            || error.Contains("obligatorio", StringComparison.OrdinalIgnoreCase))
        {
            return new BadRequestObjectResult(new { message = error });
        }

        return new ConflictObjectResult(new { message = error });
    }
}
