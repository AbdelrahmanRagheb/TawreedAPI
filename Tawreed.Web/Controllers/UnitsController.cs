using Microsoft.AspNetCore.Mvc;

namespace Tawreed.Web.Controllers;

[ApiController]
[Route("api/units")]
public class UnitsController : ControllerBase
{
    private static readonly List<UnitDto> Units =
    [
        new("kg", "كيلو جرام", "Kilogram", "kg"),
        new("g", "جرام", "Gram", "g"),
        new("L", "لتر", "Liter", "L"),
        new("ml", "ملي لتر", "Milliliter", "ml"),
        new("pc", "قطعة", "Piece", "pc"),
        new("pkt", "علبة", "Packet", "pkt"),
        new("ctn", "كرتونة", "Carton", "ctn"),
        new("btl", "زجاجة", "Bottle", "btl"),
        new("pk", "رزمة", "Pack", "pk"),
    ];

    [HttpGet]
    public ActionResult GetAll() => Ok(Units);
}

public record UnitDto(string Id, string NameAr, string NameEn, string Symbol);
