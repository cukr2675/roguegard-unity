using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class SpanningExtension
    {
        public static bool Contains<T>(this Spanning<T> spanning, T value)
        {
            var index = spanning.IndexOf(value);
            return index != -1;
        }

        public static int IndexOf<T>(this Spanning<T> spanning, T value)
        {
            for (int i = 0; i < spanning.Count; i++)
            {
                if (spanning[i].Equals(value)) return i;
            }
            return -1;
        }
    }
}
