using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    [ObjectFormer.IgnoreRequireRelationalComponent]
    public class AmmoRace : EquipmentRace
    {
        [Header("AmmoRace")]

        [SerializeField] private KeywordData _ammoCategory;

        [SerializeField] private ScriptField<IApplyRogueMethod> _beShot;

        public override IEquipmentInfo GetEquipmentInfo(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            var info = new AmmoInfo(this, self);
            return info;
        }

        private class AmmoInfo : EquipmentInfo<AmmoRace>, IAmmoEquipmentInfo
        {
            public IKeyword AmmoCategory => Data._ammoCategory;

            public IApplyRogueMethod BeShot => Data._beShot.Ref ?? RoguegardSettings.DefaultRaceOption.BeShot;

            public AmmoInfo(AmmoRace data, RogueObj self)
                : base(data, self)
            {
            }
        }
    }
}
