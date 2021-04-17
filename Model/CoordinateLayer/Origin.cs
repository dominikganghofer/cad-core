using System;
using System.Collections.Generic;

namespace PCAD.Model
{
    /// <summary>
    /// The root <see cref="Coordinate"/> that exists only once per <see cref="Axis"/>.
    /// </summary>
    [Serializable]
    public class Origin : Coordinate
    {
        public override string Name => "Origin";
        public override float Value => _originPosition;

        private float _originPosition;

        public override (float min, float max) GetBounds()
        {
            return
                (float.NegativeInfinity,
                    float.PositiveInfinity); // make origin block the layout row. Todo: move this closer to its usage
        }

        public Origin(float originPosition) : base(false, null, null, new List<Coordinate>())
        {
            _originPosition = originPosition;
            Parameter = new Parameter(Guid.NewGuid().ToString(), 0f);
        }

        /// <summary>
        /// This constructor should be used only during deserialization.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="originPosition"></param>
        public Origin(string id, float originPosition) : base(id, false, null, null)
        {
            _originPosition = originPosition;
            Parameter = new Parameter(Guid.NewGuid().ToString(), 0f);
        }

        [Serializable]
        public new class Serialization : Coordinate.Serialization
        {
            public float OriginPosition;
        }

        public Serialization ToSerializableType(int index)
        {
            return new Serialization
            {
                Index = index, ParameterID = Parameter.ID, ID = ID,
                OriginPosition = _originPosition
            };
        }
        public override string ToString() =>  $"O({Value:###.##})" ;

    }
}