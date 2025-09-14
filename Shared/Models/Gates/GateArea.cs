using System.Text.Json.Serialization;

namespace ServicesGateManagment.Shared.Gates
{
    public class GateArea
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("guid")]
        public Guid Guid { get; set; }

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
