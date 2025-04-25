using Autodesk.Revit.DB.Architecture;
using RevitCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitCore.Compliance.FireSafety.Rules
{
    public class SimpleDistanceRule(double DistanceLimit) : RuleBase, IFireSafetyRule
    {


        public void ValidateRoom(Room room, List<Door> doors)
        {
  
        }
    }
}
