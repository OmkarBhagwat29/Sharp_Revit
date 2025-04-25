using Autodesk.Revit.DB.Architecture;
using RevitCore.Entities;


namespace RevitCore.Compliance.FireSafety.Rules
{
    public class DoorDirectionRule : RuleBase
    {
        public bool IsSuccess { get; private set; }

        public string Message { get; set; } = "";

        public DoorOpening OpeningSide { get; set; }

        public ElementId DoorId { get; set; }

        public XYZ RuleDirection { get; set; }

        

        public DoorDirectionRule(DoorOpening _opening, XYZ _ruleDirection = null)
        {
            this.OpeningSide = _opening;
            this.RuleDirection = _ruleDirection;
        }

        public void Evaluate(Door door)
        {
            
        }
    }
}
