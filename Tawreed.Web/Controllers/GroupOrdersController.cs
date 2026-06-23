using Microsoft.AspNetCore.Mvc;
using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Entities;

namespace Tawreed.Web.Controllers;

[ApiController]
[Route("api/group-orders")]
public class GroupOrdersController : ControllerBase
{
    private readonly IGroupOrderService _groupOrderService;

    public GroupOrdersController(IGroupOrderService groupOrderService)
    {
        _groupOrderService = groupOrderService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GroupOrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var orders = await _groupOrderService.GetAllAsync(cancellationToken);
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GroupOrderDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var order = await _groupOrderService.GetByIdAsync(id, cancellationToken);
        if (order is null)
            return NotFound();
        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<GroupOrderDto>> Create(GroupOrder groupOrder, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _groupOrderService.CreateAsync(groupOrder, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<GroupOrderDto>> UpdateStatus(Guid id, [FromBody] string status, CancellationToken cancellationToken)
    {
        var updated = await _groupOrderService.UpdateStatusAsync(id, status, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _groupOrderService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
