using Autodesk.Revit.DB.Structure;
using RevitCore.ResidentialApartments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRevit.UI
{
    public class ApartmentTypeInfoWrapper
    {
        public ApartmentType ApartmentType { get; set; }
        public List<string> RoomTypeNames { get; set; } = [];

        public string ApartmentMinimumRequiredArea { get; set; }


    }
}
