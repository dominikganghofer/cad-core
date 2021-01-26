using PCAD.Helper;

namespace PCAD.Model
{
    /// <summary>
    /// The anchor that is used as a reference while drawing. Has a primary and a secondary position.
    /// </summary>
    public class Anchor
    {
        public Anchor(AnchorCoordinates x, AnchorCoordinates y, AnchorCoordinates z)
        {
            _coordinates = new Vec<AnchorCoordinates>(x, y, z);
        }

        public Vec<float> PrimaryPosition => _coordinates.Select(coordinate => coordinate.PrimaryCoordinate.Value);

        public Vec<float> SecondaryPosition => _coordinates.Select(coordinate => coordinate.SecondaryCoordinate.Value);

        private readonly Vec<AnchorCoordinates> _coordinates;
    }
}