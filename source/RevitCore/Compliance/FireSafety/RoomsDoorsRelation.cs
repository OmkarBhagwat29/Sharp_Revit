using Autodesk.Revit.DB.Architecture;
using RevitCore.Compliance.FireSafety.Models;
using RevitCore.Entities;


namespace RevitCore.Compliance.FireSafety
{
    public class RoomsDoorsRelation
    {
        public List<Room> Rooms { get; set; } = [];
        public List<Door> Doors { get; set; } = [];

        public List<ElementId> FilteredDoors_ByDirectionIds { get; set; } = [];

        public List<ElementId> FilteredDoors_ByDistanceIds { get; set; } = [];
        public List<FireSafetyComplianceModel> ComplianceData { get; set; } = [];

        /// <summary>
        /// Each room in the rooms list maps to all the doors
        /// </summary>
        /// <param name="_rooms"></param>
        /// <param name="_doors"></param>
        public RoomsDoorsRelation(List<Room>_rooms,List<Door>_doors)
        {
            this.Rooms = _rooms;
            this.Doors = _doors;
        }

        public void FilterDoorsBasedOnDoorDirection(DoorOpening opening, XYZ target = null)
        {
            List<ElementId> outsideDoorIds = [];
            foreach (var door in Doors)
            {
                door.SetDoorOpeningState();

                if (door.OpeningState.IsDoorOnExterior)
                {
                    outsideDoorIds.Add(door.Instance.Id);
                }
            }

            this.FilteredDoors_ByDirectionIds = outsideDoorIds;
        }

        public List<Door> GetFilteredDoorsByDirection()
        {
            return [.. Doors.Where(door => FilteredDoors_ByDirectionIds.Contains(door.Instance.Id))];
        }

        public List<Door> GetFilteredDoorsByDistance()
        {
            return [.. Doors.Where(door => this.FilteredDoors_ByDistanceIds.Contains(door.Instance.Id))];
        }

        public void FilterDoorsBasedOnDistance(double distance)
        {
            this.ComplianceData.Clear();
            var doors = GetFilteredDoorsByDirection();

            var filteredDistanceDoorIds = new List<ElementId>();
           
            foreach (var room in Rooms)
            {

                var model = ValidateBasedOnDistanced(room, doors, distance);

                if (model.IsComplaint)
                {
                    filteredDistanceDoorIds.Add(model.DoorId);
                }

                this.ComplianceData.Add(model);
        
            }
            this.FilteredDoors_ByDistanceIds = filteredDistanceDoorIds;
        }

        private static FireSafetyComplianceModel ValidateBasedOnDistanced(Room room, List<Door> doors,double distance)
        {
            if (room.Name == "Room R156")
            {
                
            }
            var model = new FireSafetyComplianceModel()
            {
                RoomName = room.Name,
                RoomNumber = room.Number,
                RoomId = room.Id,
                LevelName = room.Level.Name
            };
            double minDistance = double.PositiveInfinity;
            ElementId closedDoorId = null;
            foreach (var door in doors)
            {
                var center = ((LocationPoint)door.Instance.Location).Point;

                var roomCenter = ((LocationPoint)room.Location)?.Point;

                if (roomCenter == null)
                {
                    var bbox = room.get_BoundingBox(null);
                    roomCenter = (bbox.Min + bbox.Max) / 2.0;
                }

                double currentDistance = roomCenter.DistanceTo(center);
                currentDistance = UnitUtils.ConvertFromInternalUnits(currentDistance, UnitTypeId.Meters);

                if (currentDistance<minDistance)
                {
                    minDistance = currentDistance;
                    closedDoorId = door.Instance.Id;
                }

                if (currentDistance <= distance)
                {
      
                    if (door.OpeningState.IsDoorFacingOutside)
                    {
                        model.IsComplaint = true;
                        model.DoorId = door.Instance.Id;
                        model.Explanation = $"Complaint with Door Id: {door.Instance.Id} " +
                            $"({Math.Round(currentDistance, 2)} mts away).";

                        break;
                    }
                    else
                    {
                        model.IsComplaint = false;
                        model.DoorId=door.Instance.Id;
                        model.Explanation = $"Door Id: {door.Instance.Id} ({Math.Round(currentDistance, 2)} mts away) " +
                            $"Exit door found but the door does not open outward.";
                    }
                }
            }

            if (model.DoorId == null)
            {
                //not complaint
                model.IsComplaint = false;
                model.Explanation = $"No Exit door found within {distance} mts." +
                    $"Closest Door to the room ->\n" +
                    $"Door ID: {closedDoorId} ({Math.Round(minDistance, 2)} mts away)";
            }

            if (!model.IsComplaint)
            {
                
            }

            return model;
        }


    }
}
