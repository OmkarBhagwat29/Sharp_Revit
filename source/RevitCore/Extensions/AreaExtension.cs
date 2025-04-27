using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitCore.Extensions
{
    public static class AreaExtension
    {
        private static ViewPlan CreateAreaPlan(this Document doc, AreaScheme areaScheme, string viewName)
        {
            Level level = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .FirstOrDefault();

            ViewPlan areaPlan = null;

                areaPlan = ViewPlan.CreateAreaPlan(doc, areaScheme.Id, level.Id);
                areaPlan.Name = viewName;

            return areaPlan;
        }
    }
}
