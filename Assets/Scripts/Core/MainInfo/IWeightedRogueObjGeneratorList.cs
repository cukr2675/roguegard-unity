using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IWeightedRogueObjGeneratorList
    {
        float TotalWeight { get; }

        Spanning<IWeightedRogueObjGenerator> Spanning { get; }

        int MinFrequency { get; }

        int MaxFrequency { get; }
    }
}
