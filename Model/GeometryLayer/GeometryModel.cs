using System;
using PCAD.UI.DragInteraction;

namespace PCAD.Model
{
    /// <summary>
    /// Abstract base class for all geometry models. 
    /// </summary>
    [Serializable]
    public abstract class GeometryModel : DragInteraction.IDraggable
    {
        public Vec<Coordinate> P0;
        public bool IsBaked;

        public abstract override string ToString();
    }
}