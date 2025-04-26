using Autodesk.Revit.DB.Architecture;

using RevitCore.Entities;


namespace RevitCore.Compliance.FireSafety.Rules
{
    public class SimpleDistanceRule(double DistanceLimit) : RuleBase, IFireSafetyRule
    {


        public void ValidateRoom(Room room, List<Door> doors)
        {
  
        }
    }
}
