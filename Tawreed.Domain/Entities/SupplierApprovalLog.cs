using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class SupplierApprovalLog : BaseAuditableEntity
{
    public Guid SupplierId { get; set; }
    public string Action { get; set; } = string.Empty;
    public Guid ActorId { get; set; }
    public string? Reason { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public User Actor { get; set; } = null!;
}
