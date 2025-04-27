using Autodesk.Revit.DB.Architecture;
using RevitCore.Compliance.FireSafety.Rules;
using RevitCore.Extensions;
using RevitCore.Extensions.PointInPoly;
using RevitCore.GeometryUtils;



namespace RevitCore.Entities
{
    public class Door
    {

        public static Document Doc { get; set; }
        public FamilyInstance Instance { get; set; }

        public ElementId HostId { get; set; }

        public DoorOpeningState OpeningState { get; set; } = new DoorOpeningState();

        public List<RuleBase> Rules { get; set; } = [];

        public Door()
        {

        }

        public void SetDoorOpeningState(ViewPlan floorView)
        {

            Curves.Clear();

            var fromRm = Instance.FromRoom;
            var toRm = Instance.ToRoom;

            if (fromRm is not null)
            {
                OpeningState.HasFromRoom = true;
            }

            if (toRm is not null)
            {
                OpeningState.HasToRoom = true;
            }

            if (OpeningState.HasFromRoom && OpeningState.HasToRoom)
            {
                OpeningState.IsDoorFacingOutside = false;
                OpeningState.IsDoorOnExterior = false;
                this.OpeningState.OpeningDirection = Instance.FacingOrientation;

                return;
            }


            if (toRm is not null && fromRm is not null)
                return;

            this.OpeningState.OpeningDirection = this.Instance.FacingOrientation;

            Room testRoom = null;
            if (toRm is null)
            {
                //test with from room
                testRoom = fromRm;
            }
            else
            {
                testRoom = toRm;
            }

           var isIn = CheckDoorSwingInRoom(this.Instance, testRoom,floorView);

            if (isIn)
            {
                this.OpeningState.IsDoorOnExterior = true;
                this.OpeningState.IsDoorFacingOutside = false;
            }
            else
            {
                this.OpeningState.IsDoorOnExterior = true;
                this.OpeningState.IsDoorFacingOutside = true;
            }

        }



        public static List<Solid> Solids = [];
        public static List<Curve> Curves = [];

        public static List<XYZ> Points = [];

        public static bool IsDoorOpeningInsideRoom(FamilyInstance door, Room room)
        {
            // Get door geometry
            Options options = new Options
            {
                ComputeReferences = true,
                IncludeNonVisibleObjects = true,
                DetailLevel = ViewDetailLevel.Fine
            };

            var solids = door.get_Geometry(options)
                .Cast<GeometryInstance>()
                .SelectMany(gI => gI.GetInstanceGeometry().OfType<Solid>())
                .ToList();

            var curves = door.get_Geometry(options)
    .Cast<GeometryInstance>()
    .SelectMany(gI => gI.GetInstanceGeometry().OfType<Curve>())
    .ToList();


            Door.Solids = solids;
            Door.Curves = curves;

            var opt = new SpatialElementBoundaryOptions()
            { SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish };

            //var points = 
            int count = 0;
            foreach (var curve in curves)
            {
                var midPt = curve.Evaluate(0.5, true);

                var inside = room.RoomContains(midPt, opt);

               //room.IsPointInRoom(midPt);
                if (inside)
                {
                    count++;
                    continue;
                }
            }

            // If no arc found, you could decide to assume false or handle differently
            return false;
        }

        public static bool CheckDoorSwingInRoom(FamilyInstance door, Room room,ViewPlan floorView)
        {
            Options options = new Options();
            options.IncludeNonVisibleObjects = true;
            options.View = floorView; // Current Plan View

            Transform doorTransform = door.GetTotalTransform();

            var facingFlipped = door.FacingFlipped;
            var handFlipped = door.HandFlipped;
            // Apply Facing Flip
            if (door.FacingFlipped && !door.HandFlipped)
            {
                // Mirror across X axis (local family coordinates)
                Plane planeX = Plane.CreateByNormalAndOrigin(XYZ.BasisY, XYZ.Zero);
                Transform mirrorFacing = Transform.CreateReflection(planeX);
                doorTransform = doorTransform.Multiply(mirrorFacing);
            }
            else if(!door.FacingFlipped && handFlipped)
            {
                Plane planeX = Plane.CreateByNormalAndOrigin(XYZ.BasisY, XYZ.Zero);
                Transform mirrorFacing = Transform.CreateReflection(planeX);
                doorTransform = doorTransform.Multiply(mirrorFacing);
            }



                FamilySymbol symbol = door.Symbol;
            // Get Symbolic Geometry
            GeometryElement geomElement = symbol.get_Geometry(options);
            bool isIn = false;
            bool isOutside = false;
            foreach (GeometryObject geomObj in geomElement)
            {
                if (geomObj is Curve curve)
                {
                    if (curve is Arc arc)
                    {
                        // 1. Transform the arc into model space
                        Curve transformedArc = arc.CreateTransformed(doorTransform);
                        Curves.Add(transformedArc);


                        XYZ curverCenter = transformedArc.Evaluate(0.5, true);

                        var isInside = room.IsPointInRoom(curverCenter);

                        if (isInside)
                        {
                            isIn = true;

                        }
                        else
                        {
                            isOutside = true;
                        }
                    }

                }

            }


            if (isIn && isOutside)
            {
                //it is double side opening door and valid for safety
                return false;
            }

                return isIn;
        }

        #region Debug
        public static void BakeDoorsDirectionLine(Document doc, List<Door> doors)
        {
            doc.UseTransaction(() =>
            {

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
