using Microsoft.AspNetCore.Mvc;
using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Entities;

namespace Tawreed.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product is null)
            return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(Product product, CancellationToken cancellationToken)
    {
        var created = await _productService.CreateAsync(product, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, Product product, CancellationToken cancellationToken)
    {
        if (id != product.Id)
            return BadRequest("Id mismatch.");

        var updated = await _productService.UpdateAsync(product, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _productService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
