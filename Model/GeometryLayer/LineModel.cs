using System;
using System.Collections.Generic;
using System.Linq;
using PCAD.Helper;

namespace PCAD.Model
{
    /// <summary>
    /// Model for a line geometry.
    /// </summary>
    [Serializable]
    public class LineModel : GeometryModel
    {
        public Vec<Coordinate> P1;

        [Serializable]
        public class Serialization
        {
            public string P0XID;
            public string P0YID;
            public string P0ZID;
            public string P1XID;
            public string P1YID;
            public string P1ZID;
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
            };
        }

        public static LineModel FromSerialization(Serialization serialization, Vec<List<Coordinate>> coordinates)
        {
            var p0Serialized = new Vec<string>(serialization.P0XID, serialization.P0YID, serialization.P0ZID);
            var p0 = new Vec<Coordinate>(axis => coordinates[axis].First(c => c.ID == p0Serialized[axis]));

            var p1Serialized = new Vec<string>(serialization.P1XID, serialization.P1YID, serialization.P1ZID);
            var p1 = new Vec<Coordinate>(axis => coordinates[axis].First(c => c.ID == p1Serialized[axis]));

            return new LineModel()
            {
                P0 = p0,
                P1 = p1,
                IsBaked = true
            };
        }
    }
}