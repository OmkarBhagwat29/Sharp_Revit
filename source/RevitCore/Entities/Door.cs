using Autodesk.Revit.DB.Architecture;
using RevitCore.Compliance.FireSafety;
using RevitCore.Compliance.FireSafety.Rules;
using RevitCore.Extensions;


namespace RevitCore.Entities
{
    public class Door
    {
        public FamilyInstance Instance { get; set; }

        public ElementId HostId { get; set; }

        public DoorOpeningState OpeningState { get; set; } = new DoorOpeningState();

        public List<RuleBase> Rules { get; set; } = [];

        public Door()
        {
           
        }

        public void SetDoorOpeningState()
        {
           var fromRm =  this.Instance.FromRoom;
            var toRm = this.Instance.ToRoom;

            if (fromRm is not null)
            {
                this.OpeningState.HasFromRoom = true;
            }

            if (toRm is not null)
            {
                this.OpeningState.HasToRoom = true;
            }

            if (this.OpeningState.HasFromRoom && this.OpeningState.HasToRoom)
            {
                this.OpeningState.IsDoorFacingOutside = false;
                this.OpeningState.IsDoorOnExterior = false;
            }
            else if (this.OpeningState.HasFromRoom && !this.OpeningState.HasToRoom)
            {
                this.OpeningState.IsDoorFacingOutside = true;
                this.OpeningState.IsDoorOnExterior = true;
            }
            else if (!this.OpeningState.HasFromRoom && this.OpeningState.HasToRoom)
            {
                this.OpeningState.IsDoorFacingOutside = false;
                this.OpeningState.IsDoorOnExterior = true;
            }

            if (this.OpeningState.IsDoorFacingOutside)
                this.OpeningState.OpeningDirection = this.Instance.FacingOrientation;
            else if(this.OpeningState.IsDoorOnExterior)
                this.OpeningState.OpeningDirection = -this.Instance.FacingOrientation;

        }



        #region Debug
        public static void BakeDoorsDirectionLine(Document doc,List<Door> doors)
        {
            doc.UseTransaction(() => {

                foreach (var door in doors)
                {
                    var loc = door.Instance.Location as LocationPoint;
                    var origin = loc.Point;

                    var line = doc.BakeLine(origin, door.OpeningState.OpeningDirection, 10);
                }

            }, "Door Opening Lines");
        }
        #endregion

    }
}
