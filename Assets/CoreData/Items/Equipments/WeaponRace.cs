using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class WeaponRace : EquipmentRace
    {
        [Header("WeaponRace")]

        [SerializeField] private ScriptField<ISkill> _weaponAttack;
        [SerializeField] private ScriptField<ISkill> _weaponThrow;

        public override IEquipmentInfo GetEquipmentInfo(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            var info = new WeaponInfo(this, self);
            return info;
        }

        private class WeaponInfo : EquipmentInfo<WeaponRace>, IWeaponEquipmentInfo
        {
            ISkill IWeaponEquipmentInfo.Attack => Data._weaponAttack.Ref ?? RoguegardSettings.DefaultRaceOption.WeaponAttack;

            ISkill IWeaponEquipmentInfo.Throw => Data._weaponThrow.Ref ?? RoguegardSettings.DefaultRaceOption.WeaponThrow;

            public WeaponInfo(WeaponRace data, RogueObj self)
                : base(data, self)
            {
            }
        }
    }
}
