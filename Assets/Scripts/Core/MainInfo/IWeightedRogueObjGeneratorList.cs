using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IWeightedRogueObjGeneratorList //: IReadOnlyList<IWeightedRogueObjGenerator>
    {
        float TotalWeight { get; }

        Spanning<IWeightedRogueObjGenerator> Spanning { get; }

        int MinFrequency { get; }

        int MaxFrequency { get; }
    }
}
