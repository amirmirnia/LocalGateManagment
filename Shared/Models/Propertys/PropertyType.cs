using ServicesGateManagment.Shared.Common;


namespace ServicesGateManagment.Shared.Propertys;

public class PropertyType : BaseAuditableEntity
{
    public string Title { get; set; }
    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
