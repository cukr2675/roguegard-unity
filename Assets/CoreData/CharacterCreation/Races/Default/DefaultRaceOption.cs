using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// <see cref="ObjectRaceOption"/> がデフォルトで返す値。
    /// </summary>
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Race/Default")]
    public class DefaultRaceOption : ScriptableObject
    {
        [SerializeField] private RogueGenderList _genders;
        public Spanning<IRogueGender> Genders => _genders;

        [Space]
        [SerializeField] private KeywordData _category;
        public IKeyword Category => _category;

        [Space]
        [SerializeField] private ScriptableFaction _faction;
        public ISerializableKeyword Faction => _faction?.Faction;
        public Spanning<ISerializableKeyword> TargetFactions => _faction != null ? _faction.TargetFactions : Spanning<ISerializableKeyword>.Empty;

        [SerializeField] private RogueMaterial _material;
        public IRogueMaterial Material => _material;

        [Space]
        [SerializeField] private ScriptField<IActiveRogueMethod> _walk;
        public virtual IActiveRogueMethod Walk => _walk.Ref;

        [SerializeField] private ScriptField<IActiveRogueMethod> _wait;
        public virtual IActiveRogueMethod Wait => _wait.Ref;

        [SerializeField] private ScriptField<ISkill> _attack;
        public virtual ISkill Attack => _attack.Ref;

        [SerializeField] private ScriptField<ISkill> _throw;
        public virtual ISkill Throw => _throw.Ref;

        [SerializeField] private ScriptField<IActiveRogueMethod> _pickUp;
        public virtual IActiveRogueMethod PickUp => _pickUp.Ref;

        [SerializeField] private ScriptField<IActiveRogueMethod> _put;
        public virtual IActiveRogueMethod Put => _put.Ref;

        [SerializeField] private ScriptField<IEatActiveRogueMethod> _eat;
        public virtual IEatActiveRogueMethod Eat => _eat.Ref;



        [Space]
        [SerializeField] private ScriptField<IAffectRogueMethod> _hit;
        public virtual IAffectRogueMethod Hit => _hit.Ref;

        [SerializeField] private ScriptField<IAffectRogueMethod> _beDefeated;
        public virtual IAffectRogueMethod BeDefeated => _beDefeated.Ref;

        [SerializeField] private ScriptField<IChangeStateRogueMethod> _locate;
        public virtual IChangeStateRogueMethod Locate => _locate.Ref;

        [SerializeField] private ScriptField<IChangeStateRogueMethod> _polymorph;
        public virtual IChangeStateRogueMethod Polymorph => _polymorph.Ref;



        [Space]
        [SerializeField] private ScriptField<IApplyRogueMethod> _beApplied;
        public virtual IApplyRogueMethod BeApplied => _beApplied.Ref;

        [SerializeField] private ScriptField<IApplyRogueMethod> _beThrown;
        public virtual IApplyRogueMethod BeThrown => _beThrown.Ref;

        [SerializeField] private ScriptField<IApplyRogueMethod> _beEaten;
        public virtual IApplyRogueMethod BeEaten => _beEaten.Ref;

        [Space]
        [SerializeField] private ScriptField<IRaceOptionWeight> _weight;
        [SerializeField] private ScriptField<IRaceOptionSprite> _sprite;

        [Header("WeaponRace")]
        [SerializeField] private ScriptField<ISkill> _weaponAttack;
        public virtual ISkill WeaponAttack => _weaponAttack.Ref;

        [SerializeField] private ScriptField<ISkill> _weaponThrow;
        public virtual ISkill WeaponThrow => _weaponThrow.Ref;

        [Header("AmmoRace")]
        [SerializeField] private ScriptField<IApplyRogueMethod> _beShot;
        public virtual IApplyRogueMethod BeShot => _beShot.Ref;

        public float GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return _weight.Ref.GetWeight(raceOption, characterCreationData);
        }

        public void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out NodeBone mainNode, out AppearanceBoneSpriteTable boneSpriteTable)
        {
            _sprite.Ref.GetSpriteValues(raceOption, characterCreationData, gender, out mainNode, out boneSpriteTable);
        }

        public void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IReadOnlyNodeBone nodeBone,
            out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            _sprite.Ref.GetObjSprite(raceOption, characterCreationData, gender, self, nodeBone, out objSprite, out motionSet);
        }
    }
}
