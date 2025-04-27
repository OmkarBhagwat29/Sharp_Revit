
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using RevitCore.Compliance.FireSafety;
using RevitCore.Extensions;
using RevitCore.Extensions.Filters;

namespace SharpRevit.UI.ViewModels.FireCompliance
{
    public static class FCHelper
    {
        public static FireSafetySystem Sys;


        public static List<ViewPlan> Views = [];

        public static void Evaluate(Document doc)
        {
            try
            {
                Views.Clear();

                Sys.Evaluate();

                doc.UseTransaction(() =>
                {
                    CreateColorSchemePlanViews(doc, "Department");
                }, "created plans"
                );
            }
            catch
            {

            }

        }

        private static void CreateColorSchemePlanViews(Document doc, string colorSchemeName)
        {

            var tag = GetRoomTag(doc);
            int floorCount = Sys.RoomCollection.Count;

            var colorScheme = new FilteredElementCollector(doc)
                .OfClass(typeof(ColorFillScheme))
                .Cast<ColorFillScheme>()
                .FirstOrDefault(cs => cs.Name.Equals(colorSchemeName,
                System.StringComparison.OrdinalIgnoreCase));

            List<ViewPlan> plans = new List<ViewPlan>();
            bool viewExists = true;
            for (int i = 0; i < floorCount; i++)
            {
                var rel = Sys.RoomsDoorsRelations[i];
                var level = rel.Rooms[0].Level;
                string viewName = $"Fire Safety_{level.Name}";
                if (!doc.TryGetNameView(viewName, out View view))
                {
                    view = doc.CreateFloorPlan(level.Id, viewName);
                    viewExists = false;
                }
                var viewPlan = (ViewPlan)view;

                int j = 0;
                foreach (var room in rel.Rooms)
                {
                    var model = rel.ComplianceData[j];

                    string status = "";
                    if (model.IsComplaint)
                    {
                        status = "Compliant";
                    }
                    else if (model.IsSemiComplaint)
                    {
                        status = "Semi-Compliant";
                    }
                    else
                    {
                        status = "Non-Compliant";
                    }
                    var param = room.LookupParameter("Department");
                    var success = param.Set(status);

                    j++;

                    //place room tags
                    if (!viewExists)
                    {
                        LocationPoint location = room.Location as LocationPoint;
                        XYZ tagLocation = null;
                        if (location == null)
                        {
                            tagLocation = GetRoomCenter(room);
                        }
                        else
                        {
                            tagLocation = location.Point;
                        }
                        IndependentTag.Create(
                       doc,
                       tag.Id,
                       viewPlan.Id,
                       new Reference(room),
                       false,   // not a leader
                       TagOrientation.Horizontal,
                       tagLocation
                       );
                    }

                }

                if (colorScheme != null && viewPlan is not null)
                {

                    var supportedCategories = viewPlan.SupportedColorFillCategoryIds().ToList();
                    foreach (var categoryId in supportedCategories)
                    {
                        var canApply = viewPlan.CanApplyColorFillScheme(categoryId, colorScheme.Id);

                        if (canApply)
                        {
                            viewPlan.SetColorFillSchemeId(categoryId, colorScheme.Id);
                            break;
                        }
                    }
                }
                Views.Add(viewPlan);
            }
        }

        public static void Reset(Document doc)
        {

            doc.UseTransaction(() =>
            {

                Sys.RoomsDoorsRelations.ForEach((rel) =>
                {
                    rel.Rooms.ForEach(room => room.LookupParameter("Department").Set(""));
                });

                foreach (var view in Views)
                {
                    if (doc.ActiveView.Id == view.Id)
                        continue;

                    doc.Delete(view.Id);
                }
            });

            Sys.Clear();

            Views.Clear();
        }

        public static void EvaluateTravelDistance(Document doc, double distance)
        {
            Sys.DoorDistanceThreshold = distance;

            Sys.ValidateEscapeDistance();

            doc.UseTransaction(() =>
            {

                foreach (var rel in Sys.RoomsDoorsRelations)
                {
                    int j = 0;
                    foreach (var room in rel.Rooms)
                    {
                        var model = rel.ComplianceData[j];

                        string status = "";
                        if (model.IsComplaint)
                        {
                            status = "Compliant";
                        }
                        else if (model.IsSemiComplaint)
                        {
                            status = "Semi-Compliant";
                        }
                        else
                        {
                            status = "Non-Compliant";
                        }
                        var param = room.LookupParameter("Department");
                        var success = param.Set(status);

                        j++;
                    }
                }
            });

        }

        public static List<RoomComplianceModel> GetReport()
        {
            var models = new List<RoomComplianceModel>();
            foreach (var rel in Sys.RoomsDoorsRelations)
            {
                var data = rel.ComplianceData;

                foreach (var item in data)
                {
                    var status = ComplianceStatus.NonCompliant;

                    if (item.IsSemiComplaint)
                    {
                        status = ComplianceStatus.SemiCompliant;
                    }
                    else if(item.IsComplaint)
                    {
                        status = ComplianceStatus.Compliant;
                    }


                    var model = new RoomComplianceModel()
                    {
                        RoomName = item.RoomName,
                        LevelName = item.LevelName,
                        RoomId = item.RoomId.Value,
                        Status = status,
                        Explanation = item.Explanation
                    };

                    models.Add(model);
                }

            }

            return models.OrderBy(m => m.RoomName).ToList();
        }

        public static Element GetRoomTag(Document doc)
        {

            string familyTypeName = "Fire_Safety_Tag";


            FamilySymbol roomTagSymbol = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_RoomTags)
                .WhereElementIsElementType()
                .Cast<FamilySymbol>()
                .Where(f => f.Name == familyTypeName)
                .FirstOrDefault();



            return roomTagSymbol;
        }



        private static XYZ GetRoomCenter(Room room)
        {
            var bbox = room.get_BoundingBox(null);
            return (bbox.Min + bbox.Max) / 2.0;
        }
    }
}
