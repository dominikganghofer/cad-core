using System;
using System.Collections.Generic;
using System.Linq;

namespace PCAD.Model
{
    /// <summary>
    /// The model of a rectangle geometry.
    /// </summary>
    [Serializable]
    public class RectangleModel : GeometryModel
    {
        public Vec<Coordinate> P1;
        public GeometryColor Color;

        [Serializable]
        public class Serialization
        {
            public string P0XID;
            public string P0YID;
            public string P0ZID;
            public string P1XID;
            public string P1YID;
            public string P1ZID;
            public string Color;
        }

        public Serialization ToSerialization()
        {
            return new Serialization()
            {
                P0XID = P0.X.ID,
                P0YID = P0.Y.ID,
                P0ZID = P0.Z.ID,
                P1XID = P1.X.ID,
                P1YID = P1.Y.ID,
                P1ZID = P1.Z.ID,
                Color = Color.ToString(),
            };
        }

        public static RectangleModel FromSerialization(Serialization serialization, Vec<List<Coordinate>> coordinates)
        {
            var p0Serialized = new Vec<string>(serialization.P0XID, serialization.P0YID, serialization.P0ZID);
            var p0 = new Vec<Coordinate>(axis => coordinates[axis].First(c => c.ID == p0Serialized[axis]));

            var p1Serialized = new Vec<string>(serialization.P1XID, serialization.P1YID, serialization.P1ZID);
            var p1 = new Vec<Coordinate>(axis => coordinates[axis].First(c => c.ID == p1Serialized[axis]));

            GeometryColor color;
            if (serialization.Color == GeometryColor.White.ToString())
                color = GeometryColor.White;
            else if (serialization.Color == GeometryColor.Black.ToString())
                color = GeometryColor.Black;
            else // (serialization.Color == GeometryColor.Grey.ToString())
                color = GeometryColor.Grey;

            return new RectangleModel()
            {
                P0 = p0,
                P1 = p1,
                IsBaked = true,
                Color = color,
            };
        }
    }
}