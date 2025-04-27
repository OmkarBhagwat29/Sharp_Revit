using Autodesk.Revit.DB.Architecture;
using RevitCore.Compliance.FireSafety;
using RevitCore.Compliance.FireSafety.Rules;
using RevitCore.Entities;
using RevitCore.Extensions;
using System.Collections.Generic;


namespace RevitCore.Compliance.FireSafety
{
    public class FireSafetySystem
    {
        public Document Doc;

        public Dictionary<ElementId, List<Room>> RoomCollection = [];
        public Dictionary<ElementId, List<Door>> DoorCollection = [];

        public List<RoomsDoorsRelation> RoomsDoorsRelations = new List<RoomsDoorsRelation>();

        public double DoorDistanceThreshold { get; set; } = 30;

        public XYZ EscapeDirection { get; set; } = null;

        public FireSafetySystem(Document doc)
        {
            Doc = doc;
        }

        public void Clear()
        {
            this.RoomsDoorsRelations.Clear();
            this.RoomCollection.Clear();
            this.DoorCollection.Clear();    
        }

        public ViewPlan FloorView;

        public void Evaluate()
        {
            this.Clear();

            this.FloorView = Doc.GetAnyFloorPlanView();

            this.SetDoorsByLevel();
            this.SetRoomsByLevel();

            this.SetRoomsDoorsRelation();

            this.Validate();
        }

        public void SetRoomsDoorsRelation()
        {
            var rels = new List<RoomsDoorsRelation>();
            foreach (var item in this.RoomCollection)
            {
                var levelId = item.Key;
                var doors = this.DoorCollection[levelId];
                var rooms = item.Value;

                rels.Add(new RoomsDoorsRelation(rooms, doors));
            }

            this.RoomsDoorsRelations = rels;
        }


        public void Validate()
        {
            foreach (var rel in RoomsDoorsRelations)
            {
                rel.FilterDoorsBasedOnDoorDirection(this.FloorView);

                rel.FilterDoorsBasedOnDistance(this.DoorDistanceThreshold);
            }
        }

        public void ValidateEscapeDistance()
        {
            foreach (var rel in RoomsDoorsRelations)
            {
                rel.FilterDoorsBasedOnDistance(this.DoorDistanceThreshold);
            }
        }


        public void SetDoorsByLevel()
        {
            //get all rooms 
            //get doors for each room
            this.DoorCollection = Doc.GetInstancesOfCategory(BuiltInCategory.OST_Doors)
                .Cast<FamilyInstance>()
                .GroupBy(f => f.LevelId)
                .ToDictionary(f => f.Key, f => f.Select(fi => new Door() { Instance = fi }).ToList());
        }


        public void SetRoomsByLevel()
        {
            this.RoomCollection = Doc.GetInstancesOfCategory(BuiltInCategory.OST_Rooms)
                    .Cast<Room>()
                    .Where(r => r.LevelId != null && r.LevelId.Value != -1)
                    .GroupBy(g => g.LevelId)
                    .ToDictionary(g => g.Key, g => g.ToList());
        }

        #region Rules




        #endregion
    }
}
