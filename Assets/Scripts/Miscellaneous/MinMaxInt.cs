using System;
using UnityEngine;

namespace Miscellaneous
{
    [Serializable]
    public struct MinMaxInt
    {
        [SerializeField] private int min;
        [SerializeField] private int max;

        public int Min => min;
        public int Max => max;
    
        public int Random => UnityEngine.Random.Range(min, max + 1);

        public MinMaxInt(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public int Clamp(int value)
        {
            return Mathf.Clamp(value, min, max);
        }
    }
}