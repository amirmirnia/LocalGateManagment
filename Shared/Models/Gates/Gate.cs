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

        [JsonPropertyName("guid")]
        public Guid Guid { get; set; } = Guid.NewGuid();

        [JsonPropertyName("type")]
        public GateType Type { get; set; }

        [JsonPropertyName("objectType")]
        public GateObjectType ObjectType { get; set; }

        [JsonPropertyName("inquireType")]
        public InquireType InquireType { get; set; }

        [JsonPropertyName("armActionOnSuccess")]
        public string ArmActionOnSuccess { get; set; } = string.Empty;

        [JsonPropertyName("armActionOnFailed")]
        public string ArmActionOnFailed { get; set; } = string.Empty;

        [JsonPropertyName("checkUnauthorizedEntry")]
        public bool CheckUnauthorizedEntry { get; set; }

        [JsonPropertyName("connectivityStatus")]
        public int ConnectivityStatus { get; set; } = 2;

        [JsonPropertyName("gateAreaId")]
        public int? GateAreaId { get; set; }
    }
}
