using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IWeightedRogueObjGeneratorList
    {
        float TotalWeight { get; }

        Spanning<IWeightedRogueObjGenerator> Span { get; }

        int MinFrequency { get; }

        int MaxFrequency { get; }
    }
}
