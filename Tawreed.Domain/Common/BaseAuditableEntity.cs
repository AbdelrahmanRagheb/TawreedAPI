namespace Tawreed.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public abstract class BaseSoftDeletableEntity : BaseAuditableEntity
{
    public bool IsDeleted { get; set; }
}
