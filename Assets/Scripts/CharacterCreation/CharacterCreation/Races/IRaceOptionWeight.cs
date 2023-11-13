using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IRaceOptionWeight
    {
        float GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData);
    }
}
