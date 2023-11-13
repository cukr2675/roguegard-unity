using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IWeightedRogueObjGenerator : IRogueObjGenerator
    {
        float Weight { get; }
    }
}
