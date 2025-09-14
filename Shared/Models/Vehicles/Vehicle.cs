using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ServicesGateManagment.Shared.Enum;
using ServicesGateManagment.Shared.Propertys;

namespace ServicesGateManagment.Shared.Vehicles
{
    public class Vehicle
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("platePart1")]
        public int PlatePart1 { get; set; }

        [JsonPropertyName("letter")]
        public VehiclePlateLetter Letter { get; set; }

        [JsonPropertyName("platePart2")]
        public int PlatePart2 { get; set; }

        [JsonPropertyName("platePart3")]
        public int PlatePart3 { get; set; }

        [JsonPropertyName("type")]
        public VehicleType Type { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; } = string.Empty;

        [JsonPropertyName("createdBy")]
        public string? CreatedBy { get; set; }

        [JsonPropertyName("createdUtc")]
        public DateTime CreatedUtc { get; set; }

        [JsonPropertyName("lastModifiedBy")]
        public string? LastModifiedBy { get; set; }

        [JsonPropertyName("lastModifiedUtc")]
        public DateTime LastModifiedUtc { get; set; }

        [JsonPropertyName("properties")]
        public ICollection<PropertyVehicle> Properties { get; set; } = new List<PropertyVehicle>();

        [JsonPropertyName("isEntranceBlacklisted")]
        public bool IsEntranceBlacklisted { get; set; } = false;

        [JsonPropertyName("isExitBlacklisted")]
        public bool IsExitBlacklisted { get; set; } = false;
    }
}
