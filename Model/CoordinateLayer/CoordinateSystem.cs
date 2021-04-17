using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PCAD.Model
{
    /// <summary>
    /// The coordinate system holds all components of the coordinate layer. 
    /// </summary>
    [Serializable]
    public class CoordinateSystem
    {
        //todo: subscribe and update only on change
        public event Action CoordinateSystemChangedEvent;
        public Vec<Axis> Axes { get; private set; }
        public Anchor Anchor { get; }
        public Vec<Parameter> SnappedParameter = new Vec<Parameter>();
        public Vec<Coordinate> SnappedCoordinate = new Vec<Coordinate>();

        [Serializable]
        public class SerializableCoordinateSystem
        {
            public List<Axis.SerializableAxis> Axes;
            public List<Parameter> Parameters;
        }

        public SerializableCoordinateSystem GetSerializableType()
        {
            return new SerializableCoordinateSystem
            {
                Axes = Axes.Select(a => a.ToSerializableType()).ToList(), Parameters = GetAllParameters()
            };
        }

        public void SetSerialization(SerializableCoordinateSystem serialCS)
        {
            Debug.Log("Coordinate System - Set Serialization");
            SnappedCoordinate = new Vec<Coordinate>();
            SnappedParameter = new Vec<Parameter>();
            Axes.X.SetSerializableType(serialCS.Axes[0], serialCS.Parameters);
            Axes.Y.SetSerializableType(serialCS.Axes[1], serialCS.Parameters);
            Axes.Z.SetSerializableType(serialCS.Axes[2], serialCS.Parameters);
        }

        public CoordinateSystem(Vec<float> mousePositionAsOrigin)
        {
            Axes = new Vec<Axis>
            {
                X = new Axis(OnAxisChanged, Vec.AxisID.X, mousePositionAsOrigin.X),
                Y = new Axis(OnAxisChanged, Vec.AxisID.Y, mousePositionAsOrigin.Y),
                Z = new Axis(OnAxisChanged, Vec.AxisID.Z, mousePositionAsOrigin.Z)
            };


            var xAnchorCoordinate = Axes[Vec.AxisID.X].Anchor;
            var yAnchorCoordinate = Axes[Vec.AxisID.Y].Anchor;
            var zAnchorCoordinate = Axes[Vec.AxisID.Z].Anchor;

            Anchor = new Anchor(xAnchorCoordinate, yAnchorCoordinate, zAnchorCoordinate);
        }


        public Axis AxisThatContainsCoordinate(Coordinate c)
        {
            foreach (Axis a in Axes)
            {
                if (a.Coordinates.Contains(c))
                    return a;
            }

            Console.WriteLine($"No Axis contains the coordiante {c}");
            return null;
        }

        public void SetAnchorPosition(Vec<float> position)
        {
            foreach (var a in Vec.XYZ)
            {
                Axes[a].SnapAnchorToClosestCoordinate(position[a]);
            }
        }

        public List<Parameter> GetAllParameters()
        {
            var output = new List<Parameter>();
            foreach (var a in Vec.XYZ)
            {
                output.AddRange(
                    Axes[a].Coordinates
                        .Where(c => c.GetType() != typeof(Origin))
                        .Select(c => c.Parameter)
                );
            }

            var distinctList = new List<Parameter>();
            foreach (var parameter in output)
            {
                if (distinctList.Any(p => p.ID == parameter.ID))
                    continue;
                distinctList.Add(parameter);
            }

            return distinctList.OrderBy(p => p.Value).ToList();
        }

        private void OnAxisChanged()
        {
            CoordinateSystemChangedEvent?.Invoke();
        }


        public override string ToString()
        {
            var output = "";
            output += $"Anchor: {Anchor}\n";
            output += $"SnappedParameter: {SnappedParameter}\n";
            output += $"SnappedCoordinate: {SnappedCoordinate}\n";
            output += Axes[Vec.AxisID.X].ToString();
            output += Axes[Vec.AxisID.Z].ToString();
            return output;
        }
    }
}