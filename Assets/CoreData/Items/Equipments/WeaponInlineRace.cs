using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    [Objforming.IgnoreRequireRelationalComponent]
    public class WeaponRace : EquipmentInlineRace
    {
        [Header("WeaponRace")]

        [SerializeField] private ScriptRef<ISkill> _weaponAttack;
        [SerializeField] private ScriptRef<ISkill> _weaponThrow;

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
