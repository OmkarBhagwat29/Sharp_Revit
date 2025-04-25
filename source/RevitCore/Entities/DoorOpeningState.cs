

namespace RevitCore.Entities
{
    public class DoorOpeningState
    {
        public bool HasFromRoom { get; set; }
        public bool HasToRoom { get; set; }

        public bool IsDoorOnExterior { get; set; }

        public bool IsDoorFacingOutside { get; set; }

        public XYZ OpeningDirection { get; set; } = XYZ.Zero;
    }
}
