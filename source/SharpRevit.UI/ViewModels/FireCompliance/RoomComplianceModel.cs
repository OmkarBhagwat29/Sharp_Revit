
using System.Text.Json.Serialization;


namespace SharpRevit.UI.ViewModels.FireCompliance
{

    [JsonConverter(typeof(JsonStringEnumConverter))] // optional: serialize enum as string
    public enum ComplianceStatus
    {
        Compliant,
        NonCompliant,
        SemiCompliant
    }
    public class RoomComplianceModel
    {
        [JsonPropertyName("level")]
        public string LevelName { get; set; }

        [JsonPropertyName("roomName")]
        public string RoomName { get; set; }

        [JsonPropertyName("roomId")]
        public long RoomId { get; set; }

        [JsonPropertyName("status")]
        public ComplianceStatus Status { get; set; }

        [JsonPropertyName("explanation")]
        public string Explanation { get; set; }
    }
}
