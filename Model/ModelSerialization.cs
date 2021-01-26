using System;
using System.Collections.Generic;

namespace PCAD.Model
{
    [Serializable]
    public class ModelSerialization
    {
        public CoordinateSystem.SerializableCoordinateSystem cs;
        public List<PointModel.Serialization> points;
        public List<LineModel.Serialization> lines;
        public List<RectangleModel.Serialization> rectangles;
    }
}