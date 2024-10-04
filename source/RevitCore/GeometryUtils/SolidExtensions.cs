﻿using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using RevitCore.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace RevitCore.GeometryUtils
{
    public static class SolidExtensions
    {
        public static void Visualize(this Solid solid, Document document)
        {
            document.CreateDirectShape(solid);
        }
        public static Solid Union(
            this IEnumerable<Solid> solids)
        {
            return solids
                .Where(x => x.HasVolume())
                .Aggregate((x, y) => BooleanOperationsUtils.ExecuteBooleanOperation(
                        x, y, BooleanOperationsType.Union));
        }
        public static Solid CreateTransformed(
            this Solid solid, Transform transform)
        {
            return SolidUtils.CreateTransformed(solid, transform);
        }

        public static bool HasVolume(
            this Solid solid) => solid.Volume > 0;

        public static bool HasFaces(
            this Solid solid) => solid.Faces.Size > 0;


        public static List<Face> GetFaces(
            this Solid solid)
        {
            return solid.Faces.OfType<Face>().ToList();

        }

        public static List<Curve> GetCurves(
            this Solid solid)
        {
            return solid.GetFaces()
                .SelectMany(x => x.GetEdgesAsCurveLoops())
                .SelectMany(x => x)
                .ToList();
        }

        public static List<XYZ> GetVertices(
            this Solid solid)
        {
            return solid.GetCurves()
                .SelectMany(x => x.Tessellate()).ToList();
        }

        public static Solid GetSolidFromSpatialElement(Document doc, SpatialElement element, SpatialElementBoundaryOptions sebOptions = null)
        {

            if (sebOptions == null)
            {
                sebOptions = new SpatialElementBoundaryOptions
                {
                    SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish,
                };
            }

            SpatialElementGeometryCalculator cal = new SpatialElementGeometryCalculator(doc, sebOptions);
            SpatialElementGeometryResults results = cal.CalculateSpatialElementGeometry(element);
            Solid roomSolid = results.GetGeometry();

            return roomSolid;
        }
    }

}
