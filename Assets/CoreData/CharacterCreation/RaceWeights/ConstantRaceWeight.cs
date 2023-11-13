using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class ConstantRaceWeight : ReferableScript, IRaceOptionWeight
    {
        [SerializeField] private float _weight = 1f;

        public float GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return _weight;
        }
    }
}
