using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public struct StaticID
    {
        private readonly int id;

        private static int currentID;

        public bool IsValid => id == currentID;

        public static StaticID Current => new StaticID(false);

        private StaticID(bool flag)
        {
            id = currentID;
        }

        public static void Next()
        {
            currentID++;
        }
    }
}
