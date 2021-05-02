using System;
using System.Collections.Generic;
using PCAD.UI.DragInteraction;

namespace PCAD.Model
{
    /// <summary>
    /// A <see cref="Coordinate"/> with a parent and a distance <see cref="Parameter"/>
    /// </summary>
    [Serializable]
    public class Mue : Coordinate
    {
        public bool PointsInNegativeDirection;
        public override string Name => $"M({Parameter})";
        
        public override float Value =>
            PointsInNegativeDirection ? ParentValue - Parameter.Value : ParentValue + Parameter.Value;

        public override string ToString() => PointsInNegativeDirection ?  $"M[-{Parameter}]" :  $"M[{Parameter}]";

        public override (float min, float max) GetBounds()
        {
            var min = Math.Min(ParentValue, Value);
            var max = Math.Max(ParentValue, Value);
            return (min, max);
        }

        public Mue(
            float mue,
            bool pointsInNegativeDirection,
            Action<Coordinate> onDeleted,
            Action onChanged,
            bool isCurrentlyDrawn,
            Coordinate parent)
            : base(isCurrentlyDrawn, onDeleted, onChanged, new List<Coordinate>() {parent})
        {
            PointsInNegativeDirection = pointsInNegativeDirection;
            var id = Guid.NewGuid().ToString();
            if (Parameter == null)
                Parameter = new Parameter(id, mue);
            else
                Parameter.Value = mue;
        }

        public Mue(
            Parameter parameterReference,
            bool pointsInNegativeDirection,
            Action<Coordinate> onDeleted,
            Action onChanged,
            bool isCurrentlyDrawn,
            Coordinate parent)
            : base(isCurrentlyDrawn, onDeleted, onChanged, new List<Coordinate>() {parent})
        {
            PointsInNegativeDirection = pointsInNegativeDirection;
            Parameter = parameterReference;
        }


        /// <summary>
        /// This constructor should be used during deserialization, where the parent is set in later step, and coordinate must have a certain id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameterReference"></param>
        /// <param name="pointsInNegativeDirection"></param>
        /// <param name="onDeleted"></param>
        /// <param name="onChanged"></param>
        /// <param name="isCurrentlyDrawn"></param>
        public Mue(
            string id,
            Parameter parameterReference,
            bool pointsInNegativeDirection,
            Action<Coordinate> onDeleted,
            Action onChanged,
            bool isCurrentlyDrawn)
            : base(id, isCurrentlyDrawn, onDeleted, onChanged)
        {
            PointsInNegativeDirection = pointsInNegativeDirection;
            Parameter = parameterReference;
        }

        [Serializable]
        public new class Serialization : Coordinate.Serialization
        {
            public bool PointsInNegativeDirection;
            public string ParentID;
        }

        public Serialization ToSerializableType(int index)
        {
            return new Serialization
            {
                Index = index, ParentID = Parents[0].ID, ParameterID = Parameter.ID, ID = ID,
                PointsInNegativeDirection = PointsInNegativeDirection
            };
        }
    }
}