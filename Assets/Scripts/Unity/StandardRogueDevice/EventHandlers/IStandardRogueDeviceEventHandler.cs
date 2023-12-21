using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    internal interface IStandardRogueDeviceEventHandler
    {
        public bool TryHandle(IKeyword keyword, int integer, float number, object obj);
    }
}
