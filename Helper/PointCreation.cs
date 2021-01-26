using PCAD.Model;

namespace PCAD.Helper
{
    /// <summary>
    /// Methods for the creation of point geometry.
    /// </summary>
    public static class PointCreation
    {
        public static PointModel NewPoint(
            Vec<Coordinate> focusPosition)
        {
            var nextPoint = new PointModel {P0 = focusPosition, IsBaked = true};
            focusPosition.ForEach(c => c.AddAttachedGeometry(nextPoint));
            return nextPoint;
        }
    }
}