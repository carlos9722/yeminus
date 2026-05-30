using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceOrders.Api.Models.Orders;
using ServiceOrders.Api.Services;

namespace ServiceOrders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class OrdersController(IServiceOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] OrderFilterQuery filter, CancellationToken cancellationToken)
    {
        var orders = await orderService.SearchAsync(filter, cancellationToken);
        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var order = await orderService.GetByIdAsync(id, cancellationToken);
        return order is null ? NotFound(new { message = "Orden no encontrada." }) : Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (order, error) = await orderService.CreateAsync(request, cancellationToken);
        if (error is not null)
        {
            return ToErrorResult(error);
        }

        return CreatedAtAction(nameof(GetById), new { id = order!.Id }, order);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (order, error) = await orderService.UpdateAsync(id, request, cancellationToken);
        if (error is null)
        {
            return Ok(order);
        }

        return ToErrorResult(error);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(
        int id,
        [FromBody] ChangeOrderStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (success, error) = await orderService.ChangeStatusAsync(id, request, cancellationToken);
        return success ? NoContent() : ToErrorResult(error!);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var (success, error) = await orderService.SoftDeleteAsync(id, cancellationToken);
        return success ? NoContent() : ToErrorResult(error!);
    }

    private static IActionResult ToErrorResult(string error)
    {
        if (error == "Orden no encontrada.")
        {
            return new NotFoundObjectResult(new { message = error });
        }

        if (error.Contains("válido", StringComparison.OrdinalIgnoreCase)
            || error.Contains("obligatorio", StringComparison.OrdinalIgnoreCase)
            || error.Contains("descripción", StringComparison.OrdinalIgnoreCase)
            || error.Contains("existe", StringComparison.OrdinalIgnoreCase)
            || error.Contains("activo", StringComparison.OrdinalIgnoreCase))
        {
            return new BadRequestObjectResult(new { message = error });
        }

        return new ConflictObjectResult(new { message = error });
    }
}
