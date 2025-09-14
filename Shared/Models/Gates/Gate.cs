using System.Text.Json.Serialization;
using ServicesGateManagment.Shared.Enum;

namespace ServicesGateManagment.Shared.Gates
{
    public class Gate
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("needValidAccess")]
        public bool NeedValidAccess { get; set; }

        [JsonPropertyName("checkUnauthorizedEntry")]
        public bool CheckUnauthorizedEntry { get; set; }

        [JsonPropertyName("guid")]
        public Guid Guid { get; set; }

        [JsonPropertyName("armActionOnSuccess")]
        public GateArmAction ArmActionOnSuccess { get; set; }

        [JsonPropertyName("armActionOnFailed")]
        public GateArmAction ArmActionOnFailed { get; set; }

        [JsonPropertyName("inquireType")]
        public InquireType InquireType { get; set; }

        [JsonPropertyName("type")]
        public GateType Type { get; set; }

        [JsonPropertyName("objectType")]
        public GateObjectType ObjectType { get; set; }

        [JsonPropertyName("gateAreaId")]
        public int? GateAreaId { get; set; }

        [JsonPropertyName("gateArea")]
        public GateArea? GateArea { get; set; }

        [JsonPropertyName("createdBy")]
        public string? CreatedBy { get; set; }

        [JsonPropertyName("createdUtc")]
        public DateTime CreatedUtc { get; set; }

        [JsonPropertyName("lastModifiedBy")]
        public string? LastModifiedBy { get; set; }

        [JsonPropertyName("lastModifiedUtc")]
        public DateTime LastModifiedUtc { get; set; }
    }
}
