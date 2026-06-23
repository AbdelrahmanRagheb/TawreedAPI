using Microsoft.AspNetCore.Mvc;
using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Entities;

namespace Tawreed.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpGet("root")]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetRoot(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetRootAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (category is null)
            return NotFound();
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(Category category, CancellationToken cancellationToken)
    {
        var created = await _categoryService.CreateAsync(category, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> Update(Guid id, Category category, CancellationToken cancellationToken)
    {
        if (id != category.Id)
            return BadRequest("Id mismatch.");

        var updated = await _categoryService.UpdateAsync(category, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _categoryService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
