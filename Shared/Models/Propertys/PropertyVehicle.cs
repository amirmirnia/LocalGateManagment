using System.Text.Json.Serialization;
using ServicesGateManagment.Shared.Enum;

namespace ServicesGateManagment.Shared.Propertys
{
    public class PropertyVehicle
    {
        [JsonPropertyName("propertyId")]
        public int PropertyId { get; set; }

        [JsonPropertyName("property")]
        public Property Property { get; set; } = null!;

        [JsonPropertyName("vehicleId")]
        public int VehicleId { get; set; }

        [JsonPropertyName("type")]
        public PropertyVehicleType Type { get; set; }

        [JsonPropertyName("mobile")]
        public string Mobile { get; set; } = string.Empty;

        [JsonPropertyName("proofFileName")]
        public string ProofFileName { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("expireDate")]
        public DateTime? ExpireDate { get; set; }

        [JsonPropertyName("lastInquireDate")]
        public DateTime? LastInquireDate { get; set; }

        [JsonPropertyName("status")]
        public PropertyVehicleStatus Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("visitorConfirmStatus")]
        public VisitorConfirmStatus? VisitorConfirmStatus { get; set; }

        [JsonPropertyName("calendarId")]
        public int? CalendarId { get; set; }

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
