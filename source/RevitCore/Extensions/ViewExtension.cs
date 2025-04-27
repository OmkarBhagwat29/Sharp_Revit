

namespace RevitCore.Extensions
{
    public enum View3DType
    {
        Perspective,
        Isometric
    }
    public static class ViewExtension
    {
        public static ViewPlan CreateViewPlan(this Document doc, ElementId viewFamilyTypeId,
            ElementId levelId, string viewPlanName)
        {
            var vP = ViewPlan.Create(doc, viewFamilyTypeId, levelId);
            vP.Name = viewPlanName;

            return vP;
        }

        public static bool TryGetNameView(this Document doc, string viewName, out View view)
        {
            view = new FilteredElementCollector(doc)
                        .OfClass(typeof(View))
                        .Cast<View>()
                        .FirstOrDefault(v => v.Name.Equals(viewName, System.StringComparison.OrdinalIgnoreCase));


            if (view == null)
                return false;

            return true;
        }


        public static ViewPlan CreateFloorPlan(this Document doc, ElementId levelId, string viewPlanName)
        {
            // Find a valid ViewFamilyType for Floor Plans
            var viewFamilyType = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault(vft => vft.ViewFamily == ViewFamily.FloorPlan);

            if (viewFamilyType == null)
                throw new System.Exception("No Floor Plan ViewFamilyType found in the project.");

            ViewPlan newFloorPlan = ViewPlan.Create(doc, viewFamilyType.Id, levelId);
            newFloorPlan.Name = viewPlanName;
            
            return newFloorPlan;
        }

        public static ViewPlan GetAnyFloorPlanView(this Document doc, string viewName = null, ElementId levelId = null)
        {
            var collector = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewPlan))
                .Cast<ViewPlan>()
                .Where(vp => vp.ViewType == ViewType.FloorPlan);

            if (!string.IsNullOrEmpty(viewName))
            {
                collector = collector.Where(vp => vp.Name.Equals(viewName, StringComparison.OrdinalIgnoreCase));
            }

            if (levelId != null)
            {
                collector = collector.Where(vp => vp.GenLevel?.Id == levelId);
            }

            return collector.FirstOrDefault();
        }

        public static View3D CreateView3D(this Document doc, View3DType view3dType, string viewName)
        {
            var viewFamilyType = doc.GetElements<ViewFamilyType>(e => e.ViewFamily == ViewFamily.ThreeDimensional)
                .FirstOrDefault();

            if (viewFamilyType == null) throw new ArgumentNullException(nameof(viewFamilyType));

                View3D view;

                if (view3dType == View3DType.Perspective)
                    view = View3D.CreatePerspective(doc, viewFamilyType.Id);
                else
                    view = View3D.CreateIsometric(doc, viewFamilyType.Id);

                view.Name = viewName;

                return view;
        }

        public static ViewDrafting CreateDraftingView(this Document doc, string viewName)
        {

            var viewFamilyType = doc.GetElements<ViewFamilyType>(e => e.ViewFamily == ViewFamily.Drafting)
            .FirstOrDefault();

            if (viewFamilyType == null) throw new ArgumentNullException(nameof(viewFamilyType));

                ViewDrafting view = ViewDrafting.Create(doc, viewFamilyType.Id);

                view.Name = viewName;

                return view;

        }

        public static View CreateLegendView(this Document doc, View existingLegendView, string viewName)
        {
            View newLegendView = doc.GetElement(existingLegendView.Duplicate(ViewDuplicateOption.Duplicate)) as View;
            if(newLegendView == null) throw new ArgumentNullException(nameof(newLegendView));

            newLegendView.Name = viewName;
            return newLegendView;
        }
    }
}
