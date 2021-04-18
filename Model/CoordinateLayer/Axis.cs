using System;
using System.Collections.Generic;
using System.Linq;

namespace PCAD.Model
{
    /// <summary>
    /// The axis of a <see cref="CoordinateSystem"/>. Holds all its <see cref="Coordinate"/>s and its <see cref="Anchor"/>
    /// </summary>
    [Serializable]
    public class Axis
    {
        public AnchorCoordinates Anchor { get; }
        public Vec.AxisID Direction;
        public List<Coordinate> Coordinates;
        public Origin Origin;

        public float SmallestValue => Coordinates.Select(c => c.Value).Min();

        private event Action AxisChangedEvent;

        // private const float SnapRadius = 0.01f;
        private const float Epsilon = 0.0001f;

        [Serializable]
        public class SerializableAxis
        {
            public List<Mue.Serialization> Mues = new List<Mue.Serialization>();
            public List<Lambda.Serialization> Lambdas = new List<Lambda.Serialization>();
            public Origin.Serialization Origin;
        }

        public override string ToString()
        {
            var output = $" Axis:{Direction}|";
            // output += $"SnapRadius:{SnapRadius}\n";
            return Coordinates.Aggregate(output, (current, c) => current + $"{c}\n");
        }
        
        public SerializableAxis ToSerializableType()
        {
            var serializableAxis = new SerializableAxis();
            for (var i = 0; i < Coordinates.Count; i++)
            {
                var c = Coordinates[i];

                if (c.IsCurrentlyDrawn)
                    continue;

                if (c is Mue mue)
                    serializableAxis.Mues.Add(mue.ToSerializableType(i));
                else if (c is Lambda lambda)
                    serializableAxis.Lambdas.Add(lambda.ToSerializableType(i));
                else if (c is Origin origin)
                    serializableAxis.Origin = origin.ToSerializableType(i);
                Console.WriteLine($"Could not serialize {c}");
            }

            return serializableAxis;
        }

        public void SetSerializableType(SerializableAxis serializableAxis, List<Parameter> parameters)
        {
            // Generate all coordinates
            // The origin is always the coordinate with index = 0;
            var coordinates = new List<Coordinate>();

            var o = serializableAxis.Origin;
            var origin = new Origin(
                o.ID,
                o.OriginPosition
            );
            Anchor.ResetPrimaryCoordinate();
            Anchor.ResetSecondaryCoordinate();

            coordinates.Add(origin);

            var mues = serializableAxis.Mues;
            var lambdas = serializableAxis.Lambdas;

            var coordinateCount = mues.Count + lambdas.Count + 1;
            for (var i = 1; i < coordinateCount; i++)
            {
                if (mues.Any(c => c.Index == i))
                {
                    var mue = mues.First(m => m.Index == i);
                    var newCoordinate = new Mue(
                        mue.ID,
                        parameters.First(p => p.ID == mue.ParameterID),
                        mue.PointsInNegativeDirection,
                        OnCoordinateDeleted,
                        OnCoordinateChanged,
                        false);
                    coordinates.Add(newCoordinate);
                    continue;
                }

                if (lambdas.Any(l => l.Index == i))
                {
                    var lambda = lambdas.First(l => l.Index == i);
                    var newCoordinate = new Lambda(
                        lambda.ID,
                        0.5f, // todo: make lambdas have parameter references; parameters.First(p=>p.ID == lambda.ParameterID),
                        OnCoordinateDeleted,
                        OnCoordinateChanged,
                        false
                    );
                    coordinates.Add(newCoordinate);
                    continue;
                }

                Console.WriteLine($"Could not find any serialized Coordinate with index {i}.");
            }

            //set parents
            foreach (var mSerial in mues)
            {
                var mue = coordinates[mSerial.Index];


                if (!coordinates.Any(c => c.ID == mSerial.ParentID))
                    Console.WriteLine($"Search for {mSerial.ParentID} in {coordinates.Count} coordinates");

                mue.SetParents(new List<Coordinate>() {coordinates.First(c => c.ID == mSerial.ParentID)});
            }

            foreach (var lSerial in lambdas)
            {
                var lambda = coordinates[lSerial.Index];
                var p0 = coordinates.First(c => c.ID == lSerial.ParentIDs[0]);
                var p1 = coordinates.First(c => c.ID == lSerial.ParentIDs[1]);
                lambda.SetParents(new List<Coordinate>() {p0, p1});
            }

            Coordinates = coordinates;
        }

        public Axis(Action axisChangedCallback, Vec.AxisID direction, float originPosition)
        {
            Origin = new Origin(originPosition);
            Direction = direction;
            Coordinates = new List<Coordinate> {Origin};
            Anchor = new AnchorCoordinates(Origin);
            AxisChangedEvent += axisChangedCallback;
        }

        public void SnapAnchorToClosestCoordinate(float position)
        {
            Anchor.SetPrimaryCoordinate(FindClosestCoordinate(position));
        }

        public Coordinate AddNewMueCoordinateWithParameterValue(float parameterValue, bool pointsInNegativeDirection,
            bool asPreview)
        {
            Console.WriteLine($"{parameterValue} , {pointsInNegativeDirection}");
            var newCoordinate = new Mue(
                parameterValue,
                pointsInNegativeDirection,
                OnCoordinateDeleted,
                OnCoordinateChanged,
                asPreview,
                Anchor.PrimaryCoordinate
            );
            Coordinates.Add(newCoordinate);
            return newCoordinate;
        }

        public Coordinate AddNewMueCoordinateWithParameterReference(Parameter parameterReference,
            bool pointsInNegativeDirection, bool asPreview)
        {
            var newCoordinate =
                new Mue(parameterReference, pointsInNegativeDirection, OnCoordinateDeleted,
                    OnCoordinateChanged, asPreview, Anchor.PrimaryCoordinate);
            Coordinates.Add(newCoordinate);
            return newCoordinate;
        }

        public Coordinate AddNewMueCoordinate(float position, bool asPreview)
        {
            var delta = position - Anchor.PrimaryCoordinate.Value;
            var pointsInNegativeDirection = delta < 0f;
            if (pointsInNegativeDirection)
                delta *= -1f;

            var newCoordinate = new Mue(
                delta,
                pointsInNegativeDirection,
                OnCoordinateDeleted,
                OnCoordinateChanged,
                asPreview,
                Anchor.PrimaryCoordinate
            );
            Coordinates.Add(newCoordinate);
            return newCoordinate;
        }

        public Coordinate AddNewMueCoordinate(Parameter parameter, bool pointsInNegativeDirection, bool asPreview)
        {
            var newCoordinate = new Mue(
                parameter,
                pointsInNegativeDirection,
                OnCoordinateDeleted,
                OnCoordinateChanged,
                asPreview,
                Anchor.PrimaryCoordinate
            );
            Coordinates.Add(newCoordinate);
            return newCoordinate;
        }

        public Coordinate TryToSnapToExistingCoordinate(float position, bool isPreview, float snapRadius)
        {
            var closestCoordinate = FindClosestCoordinate(position);
            var distanceToClosestCoordinate = Math.Abs(position - closestCoordinate.Value);
            var distanceToPotentialLambdaCoordinate = DistancePotentialToLambdaCoordinate(position);

            if (distanceToClosestCoordinate > snapRadius && distanceToPotentialLambdaCoordinate > snapRadius)
                return null;

            if (distanceToClosestCoordinate < distanceToPotentialLambdaCoordinate + Epsilon)
                return closestCoordinate;

            return AddLambdaCoordinateBetweenAnchors(isPreview);
        }


        public (Parameter parameter, bool pointsInNegativeDirection)? TryToSnapToExistingParameter(
            float parameterValue, List<Parameter> allParameters, float snapRadius)
        {
            if (Coordinates.Count == 0)
                return null;

            var parametersWithDistance = allParameters.Select(p => GenerateDistanceToValue(p, parameterValue));

            (Parameter p, float distance, bool pointsInNegativeDirection)? candidate = null;
            foreach (var p in parametersWithDistance)
            {
                if (!candidate.HasValue)
                {
                    candidate = p;
                    continue;
                }

                if (p.distance < candidate.Value.distance)
                    candidate = p;
            }

            if (!candidate.HasValue || candidate.Value.distance > snapRadius)
                return null;

            return (candidate.Value.p, candidate.Value.pointsInNegativeDirection);

            ( Parameter p, float distance, bool pointsInNegativeDirection) GenerateDistanceToValue(Parameter p,
                float inputValue)
            {
                var distanceToParameter = Math.Abs(inputValue - p.Value);
                var distanceToNegativeParameter = Math.Abs(inputValue + p.Value);

                return distanceToParameter < distanceToNegativeParameter
                    ? (p, distanceToParameter, false)
                    : (p, distanceToNegativeParameter, true);
            }
        }

        private void OnCoordinateChanged()
        {
            AxisChangedEvent?.Invoke();
        }

        private void OnCoordinateDeleted(Coordinate deletedCoordinate)
        {
            Coordinates.Remove(deletedCoordinate);
            if (Anchor.PrimaryCoordinate == deletedCoordinate) Anchor.ResetPrimaryCoordinate();
            if (Anchor.SecondaryCoordinate == deletedCoordinate) Anchor.ResetSecondaryCoordinate();
        }

        private Lambda AddLambdaCoordinateBetweenAnchors(bool isPreview)
        {
            var newLambda = new Lambda(
                0.5f,
                OnCoordinateDeleted,
                OnCoordinateChanged,
                isPreview,
                (Anchor.PrimaryCoordinate, Anchor.SecondaryCoordinate)
            );
            Coordinates.Add(newLambda);
            return newLambda;
        }

        private float DistancePotentialToLambdaCoordinate(float position)
        {
            //todo: quick fix to catch undefined lambda if anchors match
            if (Anchor.AnchorsMatch) return float.PositiveInfinity;

            var lambdaPosition = (Anchor.PrimaryCoordinate.Value + Anchor.SecondaryCoordinate.Value) / 2f;
            return Math.Abs(position - lambdaPosition);
        }

        private Coordinate FindClosestCoordinate(float position)
        {
            Coordinate closestCoordinate = Origin;
            var closestDistance = Math.Abs(Origin.Value - position);
            foreach (var c in Coordinates)
            {
                var distance = Math.Abs(c.Value - position);
                if (distance < closestDistance)
                {
                    closestCoordinate = c;
                    closestDistance = distance;
                }
            }

            return closestCoordinate;
        }

       
    }
}