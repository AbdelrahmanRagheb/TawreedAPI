using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Enums;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegionsController : ControllerBase
{
    private readonly IRegionService _regionService;
    private readonly IAppSettingRepository _appSettingRepository;
    private readonly IRegionRepository _regionRepository;

    public RegionsController(
        IRegionService regionService,
        IAppSettingRepository appSettingRepository,
        IRegionRepository regionRepository)
    {
        _regionService = regionService;
        _appSettingRepository = appSettingRepository;
        _regionRepository = regionRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RegionDto>>> GetAll(CancellationToken cancellationToken)
    {
        var regions = await _regionService.GetAllAsync(cancellationToken);
        return Ok(regions);
    }

    [HttpGet("delivery-coverage")]
    public async Task<ActionResult<IReadOnlyList<RegionDto>>> GetDeliveryCoverage(CancellationToken cancellationToken)
    {
        var setting = await _appSettingRepository.GetByKeyAsync("GroupRegionTypes", cancellationToken);
        if (setting is null || string.IsNullOrWhiteSpace(setting.Value))
            return Ok(Array.Empty<RegionDto>());

        List<RegionType> allowedTypes;
        try
        {
            var typeNames = JsonSerializer.Deserialize<List<string>>(setting.Value) ?? [];
            allowedTypes = typeNames
                .Select(n => Enum.TryParse<RegionType>(n, true, out var t) ? t : (RegionType?)null)
                .Where(t => t.HasValue)
                .Select(t => t!.Value)
                .ToList();
        }
        catch
        {
            return Ok(Array.Empty<RegionDto>());
        }

        if (allowedTypes.Count == 0) return Ok(Array.Empty<RegionDto>());

        var allRegions = await _regionRepository.GetAllAsync(cancellationToken);
        var filtered = allRegions
            .Where(r => r.IsActive && allowedTypes.Contains(r.Type))
            .Select(r => new RegionDto(r.Id, r.NameAr, r.NameEn, r.ParentId, r.Type, r.IsActive, r.CreatedAt, r.UpdatedAt))
            .ToList();

        return Ok(filtered);
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
