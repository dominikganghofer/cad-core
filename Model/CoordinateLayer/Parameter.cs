using System;
using System.Linq;

namespace PCAD.Model
{
    /// <summary>
    /// A coordinate parameter. Can be used by multiple coordinates. 
    /// </summary>
    [Serializable]
    public class Parameter
    {
        public string ID;
        public float Value;
        public override string ToString() => $"{Value:###.##}";

        public Parameter(string id, float value)
        {
            ID = id;
            Value = value;
        }
    }
}