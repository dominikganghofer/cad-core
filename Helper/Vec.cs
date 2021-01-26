using System;
using System.Collections;
using System.Collections.Generic;

namespace PCAD.Helper
{
    /// <summary>
    /// A generic 3 dimensional vector  
    /// </summary>
    public abstract class Vec
    {
        public enum AxisID
        {
            X,
            Y,
            Z
        };

        public static AxisID[] XYZ => new[] {AxisID.X, AxisID.Y, AxisID.Z};
    
        public static AxisID GetOrthogonalAxis(AxisID axis)
        {
            switch (axis)
            {
                case AxisID.X:
                    return AxisID.Z;
                case AxisID.Y:
                    return AxisID.Y;
                case AxisID.Z:
                default:
                    return AxisID.X;
            }
        }
    }

    [Serializable]
    public class Vec<T> : Vec, IEnumerable<T>
    {
        public T X;
        public T Y;
        public T Z;


        public Vec()
        {
        }

        public Vec(T defaultValue)
        {
            X = defaultValue;
            Y = defaultValue;
            Z = defaultValue;
        }

        public Vec(T x, T y, T z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vec(Func<AxisID, T> generator)
        {
            X = generator.Invoke(AxisID.X);
            Y = generator.Invoke(AxisID.Y);
            Z = generator.Invoke(AxisID.Z);
        }

        public void ForEach(Action<T> function)
        {
            function.Invoke(X);
            function.Invoke(Y);
            function.Invoke(Z);
        }

        public Vec<U> Select<U>(Func<T, U> selection)
        {
            return new Vec<U>(axis => selection(this[axis]));
        }

        public T this[AxisID axis]
        {
            get => GetForAxisID(axis);
            set => SetForAxisID(axis, value);
        }

        public T GetForAxisID(AxisID axis)
        {
            switch (axis)
            {
                case AxisID.X:
                    return X;
                case AxisID.Y:
                    return Y;
                case AxisID.Z:
                    return Z;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        public void SetForAxisID(AxisID axis, T value)
        {
            switch (axis)
            {
                case AxisID.X:
                    X = value;
                    break;
                case AxisID.Y:
                    Y = value;
                    break;
                case AxisID.Z:
                    Z = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            yield return X;
            yield return Y;
            yield return Z;
        }

        public IEnumerator GetEnumerator()
        {
            yield return X;
            yield return Y;
            yield return Z;
        }
    }
}