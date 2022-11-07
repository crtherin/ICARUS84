using System;
using UnityEngine;

namespace Miscellaneous
{
    [Serializable]
    public class MinMaxFloat
    {
        [SerializeField] private float min;
        [SerializeField] private float max;

        public float Min => min;
        public float Max => max;

        public float Random => UnityEngine.Random.Range(min, max);

        public MinMaxFloat(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public float Clamp(float value)
        {
            return Mathf.Clamp(value, min, max);
        }
        
        public bool Check(float value)
        {
            if (value < min)
                return false;

            if (value > max)
                return false;

            return true;
        }
    }
}