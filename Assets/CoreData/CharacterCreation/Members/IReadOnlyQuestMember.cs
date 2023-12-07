using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyQuestMember : IReadOnlyMember
    {
        Spanning<IWeightedRogueObjGenerator> Targets { get; }
        int TargetFloor { get; }
    }
}
