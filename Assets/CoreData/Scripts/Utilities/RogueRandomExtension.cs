using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class RogueRandomExtension
    {
        public static T Choice<T>(this IRogueRandom random, T a, T b)
        {
            var index = random.Next(0, 2);
            return index == 0 ? a : b;
        }

        public static T Choice<T>(this IRogueRandom random, params T[] array)
        {
            var index = random.Next(0, array.Length);
            return array[index];
        }

        public static T Choice<T>(this IRogueRandom random, List<T> list)
        {
            var index = random.Next(0, list.Count);
            return list[index];
        }
    }
}
