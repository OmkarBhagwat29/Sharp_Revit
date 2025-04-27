

namespace RevitCore.Compliance.FireSafety.Models
{

    public class FireSafetyComplianceModel
    {
        public string LevelName { get; set; }
        public string RoomName { get; set; }

        public string RoomNumber { get; set; }

        public ElementId RoomId { get; set; }

        public ElementId DoorId { get; set; }

        public bool IsComplaint { get; set; }

        public bool IsSemiComplaint { get; set; }

        public string Explanation { get; set; } = "Explanation not available";
    }
}
