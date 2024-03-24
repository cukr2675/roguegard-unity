using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class RogueRandom : IRogueRandom
    {
        [System.NonSerialized] private System.Random random;

        private readonly int seed;
        private int position;

        public static IRogueRandom Primary { get; set; }

        public RogueRandom()
        {
            var random = new System.Random();
            seed = random.Next(int.MinValue, int.MaxValue);
            position = 0;
        }

        public RogueRandom(int seed)
        {
            this.seed = seed;
            position = 0;
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            if (random == null) { Initialize(); }

            return random.Next(minInclusive, maxExclusive);
        }

        public float NextFloat(float min, float max)
        {
            if (random == null) { Initialize(); }

            var size = (double)max - min; // float.MaxValue を超えないように double にキャストしてから計算する
            return (float)(min + random.NextDouble() * size);
        }

        private void Initialize()
        {
            random = new System.Random(seed);
            for (int i = 0; i < position; i++)
            {
                random.Next();
            }
        }
    }
}
