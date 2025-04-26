using Autodesk.Revit.DB.Architecture;
using RevitCore.Entities;



namespace RevitCore.Compliance.FireSafety
{
    public interface IFireSafetyRule
    {
        /// <summary>
        /// returns True when design fits in rule 
        /// </summary>
        /// <returns></returns>
        public void ValidateRoom(Room room, List<Door> doors);

    }
}
