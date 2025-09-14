using System.Text.Json.Serialization;
using ServicesGateManagment.Shared.Enum;

namespace ServicesGateManagment.Shared.Propertys;

    public class Property
    {
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("buildingArea")]
    public double BuildingArea { get; set; }

    [JsonPropertyName("arenaArea")]
    public double ArenaArea { get; set; }

    [JsonPropertyName("typeId")]
    public int TypeId { get; set; }

    [JsonPropertyName("type")]
    public PropertyType Type { get; set; } = null!;

    [JsonPropertyName("status")]
    public PropertyStatus Status { get; set; }

    [JsonPropertyName("canHaveVisitors")]
    public bool CanHaveVisitors { get; set; }

    [JsonPropertyName("visitorsNeedConfirm")]
    public bool VisitorsNeedConfirm { get; set; }

    [JsonPropertyName("taxiEntranceLimitedDate")]
    public DateTime? TaxiEntranceLimitedDate { get; set; }

    [JsonPropertyName("postalCode")]
    public string PostalCode { get; set; } = string.Empty;

    [JsonPropertyName("postalAddress")]
    public string PostalAddress { get; set; } = string.Empty;

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public PropertyState State { get; set; }

    [JsonPropertyName("unitsCount")]
    public int UnitsCount { get; set; }

    [JsonPropertyName("parentId")]
    public int? ParentId { get; set; }

    [JsonPropertyName("hasExclusiveLimits")]
    public bool HasExclusiveLimits { get; set; }

    [JsonPropertyName("createdBy")]
    public string? CreatedBy { get; set; }

    [JsonPropertyName("createdUtc")]
    public DateTime CreatedUtc { get; set; }

    [JsonPropertyName("lastModifiedBy")]
    public string? LastModifiedBy { get; set; }

    [JsonPropertyName("lastModifiedUtc")]
    public DateTime LastModifiedUtc { get; set; }
}

