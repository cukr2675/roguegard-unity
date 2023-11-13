using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class LinearRaceWeight : ReferableScript, IRaceOptionWeight
    {
        [SerializeField] private float _slope = 0f;
        public float Slope => _slope;

        [SerializeField] private float _intercept = 1f;
        public float Intercept => _intercept;

        public float GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            var standardMember = StandardRaceMember.GetMember(characterCreationData.Race);
            return Slope * standardMember.Size + Intercept;
        }
    }
}
