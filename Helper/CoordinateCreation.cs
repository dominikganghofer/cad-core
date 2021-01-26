using PCAD.Model;
using PCAD.UserInput;

namespace PCAD.Helper
{
    /// <summary>
    /// Methods for the creation and removal of <see cref="Coordinate"/>s.
    /// </summary>
    public static class CoordinateCreation
    {
        public static Vec<Coordinate> UpdateCursorPosition(Vec<Coordinate> oldFocusPosition,
            CoordinateSystem cs, NumpadInput.Model keyboardInput)
        {
            oldFocusPosition?.ForEach(c =>
            {
                if (c.IsCurrentlyDrawn) c.Delete();
            });

            return GetOrCreatePositionAtMousePosition(cs, cs.Anchor, true, keyboardInput);
        }

        public static void DeletePositionAtMousePosition(CoordinateSystem cs)
        {
            var p = GetOrCreatePositionAtMousePosition(cs, cs.Anchor);
            p.ForEach(c => c.Delete());
        }

        public static void BakePosition(Vec<Coordinate> modelFocusPosition)
        {
            modelFocusPosition.ForEach(c => c.Bake());
        }

        private static Vec<Coordinate> GetOrCreatePositionAtMousePosition(CoordinateSystem coordinateSystem,
            Anchor anchor,
            bool asPreview = false,
            NumpadInput.Model keyboardInput = null)
        {
            var mousePosition = MouseInput.RaycastPosition;
            var distanceToAnchor = new Vec<float>(a => mousePosition[a] - anchor.PrimaryPosition[a]);

            return
                coordinateSystem.GetParametricPosition(mousePosition, distanceToAnchor, asPreview, keyboardInput);
        }
    }
}