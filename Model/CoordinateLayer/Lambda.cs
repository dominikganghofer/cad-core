using System;
using System.Collections.Generic;
using System.Linq;

namespace PCAD.Model
{
    /// <summary>
    /// A barycentric <see cref="Coordinate"/> with two parent. 
    /// </summary>
    [Serializable]
    public class Lambda : Coordinate
    {
        public override string Name => "Lambda";
        public override float Value => (1f - Parameter.Value) * ParentValue + Parameter.Value * SecondaryParentValue;
        public float SecondaryParentValue => Parents[1].Value;

        public Lambda(
            float lambda,
            Action<Coordinate> onDeleted,
            Action onChanged,
            bool isCurrentlyDrawn,
            (Coordinate parent0, Coordinate parent1) parents //during deserialization the parents are not yet known
        )
            : base(isCurrentlyDrawn, onDeleted, onChanged, new List<Coordinate> {parents.parent0, parents.parent1})
        {
            Parameter = new Parameter(Guid.NewGuid().ToString(), lambda);
        }

        // used during deserialization
        public Lambda(
            string id,
            float lambda,
            Action<Coordinate> onDeleted,
            Action onChanged,
            bool isCurrentlyDrawn//during deserialization the parents are not yet known
        )
            : base(id, isCurrentlyDrawn, onDeleted, onChanged)
        {
            Parameter = new Parameter(Guid.NewGuid().ToString(), lambda);
        }


        public override (float min, float max) GetBounds()
        {
            var min = Math.Min(ParentValue, SecondaryParentValue);
            var max = Math.Max(ParentValue, SecondaryParentValue);
            return (min, max);
        }

        [Serializable]
        public new class Serialization : Coordinate.Serialization
        {
            public List<string> ParentIDs;
        }

        public Serialization ToSerializableType(int index)
        {
            return new Serialization
            {
                Index = index, ParentIDs = Parents.Select(p => p.ID).ToList(), ParameterID = Parameter.ID, ID = this.ID
            };
        }
        public override string ToString() =>  $"L({Parameter})" ;

    }
}