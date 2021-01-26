using System;
using PCAD.Helper;

namespace PCAD.Model
{
    /// <summary>
    /// Abstract base class for all geometry models. 
    /// </summary>
    [Serializable]
    public abstract class GeometryModel
    {
        public Vec<Coordinate> P0;
        public bool IsBaked;
    }
}