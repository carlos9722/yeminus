using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceOrders.Api.Models.Technicians;
using ServiceOrders.Api.Services;

namespace ServiceOrders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TechniciansController(ITechnicianService technicianService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var technicians = await technicianService.GetAllAsync(cancellationToken);
        return Ok(technicians);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var technician = await technicianService.GetByIdAsync(id, cancellationToken);
        return technician is null ? NotFound(new { message = "Técnico no encontrado." }) : Ok(technician);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTechnicianRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (technician, error) = await technicianService.CreateAsync(request, cancellationToken);
        if (error is not null)
        {
            return ToErrorResult(error);
        }

        return CreatedAtAction(nameof(GetById), new { id = technician!.Id }, technician);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTechnicianRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (technician, error) = await technicianService.UpdateAsync(id, request, cancellationToken);
        if (error is null)
        {
            return Ok(technician);
        }

        return ToErrorResult(error);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var (success, error) = await technicianService.SoftDeleteAsync(id, cancellationToken);
        return success ? NoContent() : ToErrorResult(error!);
    }

    private static IActionResult ToErrorResult(string error)
    {
        if (error == "Técnico no encontrado.")
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
