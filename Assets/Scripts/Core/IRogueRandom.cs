using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.RequireRelationalComponent]
    public interface IRogueRandom
    {
        int Next(int minInclusive, int maxExclusive);
        float NextFloat(float min, float max);
    }
}
