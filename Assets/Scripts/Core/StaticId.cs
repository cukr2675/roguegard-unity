using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public struct StaticId
    {
        private readonly int id;

        private static int currentId;

        public bool IsValid => id == currentId;

        public static StaticId Current => new StaticId(false);

        private StaticId(bool flag)
        {
            id = currentId;
        }

        public static void Next()
        {
            currentId++;
        }
    }
}
