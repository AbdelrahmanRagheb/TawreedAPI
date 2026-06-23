using Microsoft.AspNetCore.Mvc;
using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;

namespace Tawreed.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegionsController : ControllerBase
{
    private readonly IRegionService _regionService;

    public RegionsController(IRegionService regionService)
    {
        _regionService = regionService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RegionDto>>> GetAll(CancellationToken cancellationToken)
    {
        var regions = await _regionService.GetAllAsync(cancellationToken);
        return Ok(regions);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<RegionSearchDto>>> Search([FromQuery] string q, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Ok(Array.Empty<RegionSearchDto>());

        var regions = await _regionService.SearchAsync(q, cancellationToken);
        return Ok(regions);
    }

    [HttpGet("root")]
    public async Task<ActionResult<IReadOnlyList<RegionDto>>> GetRoot(CancellationToken cancellationToken)
    {
        var regions = await _regionService.GetRootAsync(cancellationToken);
        return Ok(regions);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RegionDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var region = await _regionService.GetByIdAsync(id, cancellationToken);
        if (region is null)
            return NotFound();
        return Ok(region);
    }

    [HttpGet("{id:guid}/children")]
    public async Task<ActionResult<IReadOnlyList<RegionDto>>> GetChildren(Guid id, CancellationToken cancellationToken)
    {
        var children = await _regionService.GetChildrenAsync(id, cancellationToken);
        return Ok(children);
    }
}
