using PCAD.Model;

namespace PCAD.Helper
{
    /// <summary>
    /// Methods for the creation of line geometry.
    /// </summary>
    public static class LineCreation
    {
        public static LineModel StartNewLine(
            Vec<Coordinate> focusPosition)
        {
            var nextLine = new LineModel {P0 = focusPosition};
            focusPosition.ForEach(c => c.AddAttachedGeometry(nextLine));
            return nextLine;
        }

        public static void CompleteLine(LineModel nextLine, Vec<Coordinate> focusPosition)
        {
            nextLine.P1 = focusPosition;
            nextLine.IsBaked = true;
        }

        public static void UpdateLine(LineModel nextLine, Vec<Coordinate> focusPosition)
        {
            nextLine.P1 = focusPosition;
        }

        public static void AbortLine(LineModel nextLine)
        {
            nextLine.P0.ForEach(c=>c.UnregisterGeometryAndTryToDelete(nextLine));
        }
    }
}