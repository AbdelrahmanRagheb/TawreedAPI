namespace Tawreed.Application.Common.Models;

public class ApproveDeliveryFeeRequest
{
    public bool IsApproved { get; set; }
    public string? Reason { get; set; }
}
