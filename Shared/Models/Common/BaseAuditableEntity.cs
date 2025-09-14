namespace ServicesGateManagment.Shared.Common;

public abstract class BaseAuditableEntity
{
    public int Id { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime CreatedUtc { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime LastModifiedUtc { get; set; }
}
