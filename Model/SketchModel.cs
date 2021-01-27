using System;
using System.Collections.Generic;
using System.Linq;
using PCAD.Model;

namespace PCAD.Model
{
    /// <summary>
    /// Consists of the sketch's <see cref="coordinateSystem"/> and the <see cref="GeometryModel"/>. 
    /// </summary>
    [Serializable]
    public struct SketchModel
    {
        public CoordinateSystem coordinateSystem;
        public List<GeometryModel> geometries;

        public ModelSerialization Serialize()
        {
            return new ModelSerialization()
            {
                cs = coordinateSystem.GetSerializableType(),
                points = geometries
                    .Where(g => g is PointModel)
                    .Select(p => (p as PointModel).ToSerialization())
                    .ToList(),
                lines = geometries
                    .Where(g => g is LineModel)
                    .Select(p => (p as LineModel).ToSerialization())
                    .ToList(),

                rectangles = geometries
                    .Where(g => g is RectangleModel)
                    .Select(p => (p as RectangleModel).ToSerialization())
                    .ToList(),
            };
        }

        public void SetSerialization(ModelSerialization serialization)
        {
            coordinateSystem.SetSerialization(serialization.cs);

            var axes = coordinateSystem.Axes;
            var coordinates = new Vec<List<Coordinate>>(axis => axes[axis].Coordinates);

            geometries = new List<GeometryModel>();
            geometries.AddRange(serialization.points.Select(p => PointModel.FromSerialization(p, coordinates)));
            geometries.AddRange(serialization.lines.Select(l => LineModel.FromSerialization(l, coordinates)));
            geometries.AddRange(serialization.rectangles.Select(r => RectangleModel.FromSerialization(r, coordinates)));
        }
    }
}