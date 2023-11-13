using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity.Tests
{
    public class DebugRogueRandom : IRogueRandom
    {
        private readonly RogueRandom random;
        private readonly Dictionary<int, float> values = new Dictionary<int, float>();
        private int index;

        public DebugRogueRandom(int seed)
        {
            random = new RogueRandom(seed);
            index = 0;
        }

        /// <summary>
        /// 指定の乱数生成回数のとき <paramref name="value"/> を返すように設定する
        /// </summary>
        public void Add(int index, float value)
        {
            values.Add(index, value);
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            var value = random.Next(minInclusive, maxExclusive);
            if (values.TryGetValue(index, out var injectedValue))
            {
                Debug.Log($"DebugRogueRandom[{index}] = {(int)injectedValue} (injected from {value}) [{minInclusive}, {maxExclusive})");
                value = (int)injectedValue;
            }
            else
            {
                Debug.Log($"DebugRogueRandom[{index}] = {value} [{minInclusive}, {maxExclusive})");
            }
            index++;
            return value;
        }

        public float NextFloat(float min, float max)
        {
            var value = random.NextFloat(min, max);
            if (values.TryGetValue(index, out var injectedValue))
            {
                Debug.Log($"DebugRogueRandom[{index}] = {injectedValue} (injected from {value}) [{min}, {max}]");
                value = injectedValue;
            }
            else
            {
                Debug.Log($"DebugRogueRandom[{index}] = {value} [{min}, {max}]");
            }
            index++;
            return value;
        }
    }
}
