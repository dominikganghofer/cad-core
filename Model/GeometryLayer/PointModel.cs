using System;
using System.Collections.Generic;
using System.Linq;
using PCAD.Helper;

namespace PCAD.Model
{
    /// <summary>
    /// Model for a point geometry.
    /// </summary>
    [Serializable]
    public class PointModel : GeometryModel
    {
        [Serializable]
        public class Serialization
        {
            public string P0XID;
            public string P0YID;
            public string P0ZID;
        }

        public Serialization ToSerialization()
        {
            return new Serialization()
            {
                P0XID = P0.X.ID,
                P0YID = P0.Y.ID,
                P0ZID = P0.Z.ID,
            };
        }

        public static PointModel FromSerialization(Serialization serialization, Vec<List<Coordinate>> coordinates)
        {
            var p0Serialized = new Vec<string>(serialization.P0XID, serialization.P0YID, serialization.P0ZID);
            var p0 = new Vec<Coordinate>(axis => coordinates[axis].First(c => c.ID == p0Serialized[axis]));

            return new PointModel()
            {
                P0 = p0,
                IsBaked = true
            };
        }
    }
}