using System;
using System.Collections.Generic;

namespace PCAD.Model
{
    /// <summary>
    /// Abstract class for all coordinates. 
    /// </summary>
    [Serializable]
    public abstract class Coordinate
    {
        public abstract string Name { get; }
        public string ID;
        public abstract float Value { get; }
        public bool IsCurrentlyDrawn { get; private set; }
        public List<Coordinate> Parents;
        public abstract override string ToString();

        public float ParentValue => Parents[0].Value;
        public abstract (float min, float max) GetBounds();

        private readonly List<Coordinate> _dependentCoordinates = new List<Coordinate>();
        private readonly List<GeometryModel> _attachedGeometry = new List<GeometryModel>();
        private Parameter _parameter;

        protected event Action ChangedEvent;
        protected event Action<Coordinate> DeletedEvent;
        protected Action FireValueChangedEvent => () => ChangedEvent?.Invoke();

        protected Coordinate(bool isCurrentlyDrawn, Action<Coordinate> onDeleted, Action onChanged,
            List<Coordinate> parents)
        {
            Parents = parents;
            IsCurrentlyDrawn = isCurrentlyDrawn;
            ID = Guid.NewGuid().ToString();
            DeletedEvent += onDeleted;
            ChangedEvent += onChanged;
            SetParents(parents);
        }

        // Used during deserialization
        protected Coordinate(string id, bool isCurrentlyDrawn, Action<Coordinate> onDeleted, Action onChanged)
        {
            IsCurrentlyDrawn = isCurrentlyDrawn;
            ID = id;
            DeletedEvent += onDeleted;
            ChangedEvent += onChanged;
        }

        public void SetParents(List<Coordinate> parents)
        {
            Parents = parents;
            foreach (var p in parents)
            {
                p.RegisterCoordinate(this, FireValueChangedEvent);
            }
        }

        public Parameter Parameter
        {
            get => _parameter;
            set
            {
                _parameter = value;
                ChangedEvent?.Invoke();
            }
    }

        public void RegisterCoordinate(Coordinate child, Action onValueChanged)
        {
            _dependentCoordinates.Add(child);
            ChangedEvent += onValueChanged;
        }

        public void UnregisterCoordinate(Coordinate child, Action onValueChanged)
        {
            _dependentCoordinates.Remove(child);
            ChangedEvent -= onValueChanged;
        }

        public void RegisterView(Action onValueChanged)
        {
            ChangedEvent += onValueChanged;
        }

        public void UnregisterView(Action onValueChanged)
        {
            ChangedEvent -= onValueChanged;
            Console.WriteLine(_dependentCoordinates.Count);
        }

        public void UnregisterGeometryAndTryToDelete(GeometryModel geometryToUnregister)
        {
            _attachedGeometry.Remove(geometryToUnregister);
            if (_attachedGeometry.Count == 0)
                Delete();
        }

        public void Delete()
        {
            if (_dependentCoordinates.Count != 0) return;
            DeletedEvent?.Invoke(this);
            foreach (var p in Parents)
            {
                p.UnregisterCoordinate(this, ChangedEvent);
            }
        }

        public void Bake()
        {
            IsCurrentlyDrawn = false;
        }

        public void AddAttachedGeometry(GeometryModel rectangle)
        {
            _attachedGeometry.Add(rectangle);
        }

        public static List<Coordinate> GetPathToOrigin(Coordinate coordinate)
        {
            var pathWithDuplicates = GetPathRecursion(coordinate);
            var cleanPath = new List<Coordinate>();
            foreach (var c in pathWithDuplicates)
            {
                if (!cleanPath.Contains(c))
                    cleanPath.Add(c);
            }

            return cleanPath;

            List<Coordinate> GetPathRecursion(Coordinate c)
            {
                var path = new List<Coordinate>() {c};
                foreach (var parent in c.Parents)
                {
                    path.AddRange(GetPathToOrigin(parent));
                }

                return path;
            }
        }

        [Serializable]
        public class Serialization
        {
            public int Index;
            public string ID;
            public string ParameterID;
        }
    }
}