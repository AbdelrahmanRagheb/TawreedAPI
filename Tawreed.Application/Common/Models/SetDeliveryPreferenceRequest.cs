namespace Tawreed.Application.Common.Models;

public class SetDeliveryPreferenceRequest
{
    public string Preference { get; set; } = "None"; // None, OwnDelivery, SystemDelivery, SpecificPerson
    public Guid? PreferredDeliveryPersonId { get; set; }
}
