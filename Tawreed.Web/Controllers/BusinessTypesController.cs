using Microsoft.AspNetCore.Mvc;

namespace Tawreed.Web.Controllers;

[ApiController]
[Route("api/business-types")]
public class BusinessTypesController : ControllerBase
{
    private static readonly List<BusinessTypeDto> Types =
    [
        new("restaurant", "مطعم", "Restaurant"),
        new("supermarket", "سوبر ماركت", "Supermarket"),
        new("cafe", "كافيه", "Cafe"),
        new("hotel", "فندق", "Hotel"),
        new("bakery", "مخبز", "Bakery"),
        new("sports-club", "نادي رياضي", "Sports Club"),
        new("hospital", "مستشفى", "Hospital"),
    ];

    [HttpGet]
    public ActionResult<IReadOnlyList<BusinessTypeDto>> GetAll()
    {
        return Ok(Types);
    }
}

public record BusinessTypeDto(string Id, string NameAr, string NameEn);
