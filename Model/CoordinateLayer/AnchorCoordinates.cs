namespace PCAD.Model
{
    /// <summary>
    /// The coordinates of the <see cref="Anchor"/>. This class is used to connect the primary and secondary anchor.
    /// The coordinate is always set for the primary anchor while the secondary one always holds the primaries last
    /// coordinate.
    /// </summary>
    public class AnchorCoordinates
    {
        public Coordinate PrimaryCoordinate { get; private set; }

        public Coordinate SecondaryCoordinate { get; private set; }

        public bool AnchorsMatch => PrimaryCoordinate == SecondaryCoordinate;

        public AnchorCoordinates(Origin origin)
        {
            _origin = origin;
            PrimaryCoordinate = origin;
            SecondaryCoordinate = origin;
        }

        public void SetPrimaryCoordinate(Coordinate c)
        {
            SecondaryCoordinate = PrimaryCoordinate;
            PrimaryCoordinate = c;
        }

        public void ResetPrimaryCoordinate()
        {
            PrimaryCoordinate = _origin;
        }

        public void ResetSecondaryCoordinate()
        {
            SecondaryCoordinate = _origin;
        }

        private readonly Origin _origin;

        public override string ToString()
        {
            return $"AnchorCoordinate: Primary:{PrimaryCoordinate}, Secondary{SecondaryCoordinate}";
        }
    }
}