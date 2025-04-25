using RevitCore.Compliance.FireSafety;


namespace SharpRevit.UI.ViewModels.FireCompliance
{
    public static class FCHelper
    {
        public static FireSafetySystem Sys;

        public static void Evaluate()
        {
            try
            {
                Sys.Evaluate();
            }
            catch
            {

            }

        }

        public static void Reset()
        {
            Sys.Clear();
        }

        public static void EvaluateTravelDistance(double distance)
        {
            Sys.DoorDistanceThreshold = distance;

            Sys.ValidateEscapeDistance();
        }

        public static List<RoomComplianceModel> GetReport()
        {
            var models = new List<RoomComplianceModel>();
            foreach (var rel in Sys.RoomsDoorsRelations)
            {
                var data = rel.ComplianceData;

                foreach (var item in data)
                {
                    var model = new RoomComplianceModel()
                    {
                        RoomName = item.RoomName,
                        LevelName = item.LevelName,
                        RoomId = item.RoomId.Value,
                        Status = item.IsComplaint?ComplianceStatus.Compliant:ComplianceStatus.NonCompliant,
                        Explanation = item.Explanation
                    };

                    models.Add(model);
                }

            }

            return models;
        }
    }
}
