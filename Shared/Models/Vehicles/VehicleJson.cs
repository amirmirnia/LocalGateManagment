using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ServicesGateManagment.Shared.Common;
using ServicesGateManagment.Shared.Enum;
using ServicesGateManagment.Shared.Propertys;

namespace ServicesGateManagment.Shared.Vehicles
{
    public class VehicleJson 
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("platePart1")]
        public int PlatePart1 { get; set; }

        [JsonPropertyName("letterId")]
        public string LetterId { get; set; } = string.Empty;

        [JsonPropertyName("letter")]
        public VehiclePlateLetter Letter { get; set; } = VehiclePlateLetter.Alef;

        [JsonPropertyName("letterTitle")]
        public string LetterTitle => VehiclePlateLetterDescription(Letter);

        [JsonPropertyName("platePart2")]
        public int PlatePart2 { get; set; }

        [JsonPropertyName("platePart3")]
        public int PlatePart3 { get; set; }

        [JsonPropertyName("displayPlate")]
        public string DisplayPlate =>
            $"{PlatePart3} | {PlatePart2} {VehiclePlateLetterDescription(Letter)} {PlatePart1}";

        [JsonPropertyName("typeId")]
        public int TypeId { get; set; }


        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("color")]
        public string Color { get; set; } = string.Empty;


        private string VehiclePlateLetterDescription(VehiclePlateLetter e)
        {
            var type = typeof(VehiclePlateLetter);
            var member = type.GetMember(e.ToString());
            var attributes = member[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            return ((DescriptionAttribute)attributes[0]).Description;
        }
    }
}
