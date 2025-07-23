using OchalikeSprites;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// <see cref="InlineRaceOption"/> がデフォルトで返す値。
    /// </summary>
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Race/Default")]
    public class DefaultRaceOptionAsset : ScriptableObject
    {
        [SerializeField] private RogueGenderListAsset _genders;
        public Spanning<IRogueGender> Genders => _genders ? _genders.Span : null;

        [Space]
        [SerializeField] private KeywordAsset _category;
        public IKeyword Category => _category;

        [Space]
        [SerializeField] private FactionAsset _faction;
        public ISerializableKeyword Faction => _faction != null ? _faction.Faction : null;
        public Spanning<ISerializableKeyword> TargetFactions => _faction != null ? _faction.TargetFactions : Spanning<ISerializableKeyword>.Empty;

        [SerializeField] private RogueMaterialAsset _material;
        public IRogueMaterial Material => _material;

        [Space]
        [SerializeField] private ScriptRef<IActiveRogueMethod> _walk;
        public virtual IActiveRogueMethod Walk => _walk.Ref;

        [SerializeField] private ScriptRef<IActiveRogueMethod> _wait;
        public virtual IActiveRogueMethod Wait => _wait.Ref;

        [SerializeField] private ScriptRef<ISkill> _attack;
        public virtual ISkill Attack => _attack.Ref;

        [SerializeField] private ScriptRef<ISkill> _throw;
        public virtual ISkill Throw => _throw.Ref;

        [SerializeField] private ScriptRef<IActiveRogueMethod> _pickUp;
        public virtual IActiveRogueMethod PickUp => _pickUp.Ref;

        [SerializeField] private ScriptRef<IActiveRogueMethod> _put;
        public virtual IActiveRogueMethod Put => _put.Ref;

        [SerializeField] private ScriptRef<IEatActiveRogueMethod> _eat;
        public virtual IEatActiveRogueMethod Eat => _eat.Ref;



        [Space]
        [SerializeField] private ScriptRef<IAffectRogueMethod> _hit;
        public virtual IAffectRogueMethod Hit => _hit.Ref;

        [SerializeField] private ScriptRef<IAffectRogueMethod> _beDefeated;
        public virtual IAffectRogueMethod BeDefeated => _beDefeated.Ref;

        [SerializeField] private ScriptRef<IChangeStateRogueMethod> _locate;
        public virtual IChangeStateRogueMethod Locate => _locate.Ref;

        [SerializeField] private ScriptRef<IChangeStateRogueMethod> _polymorph;
        public virtual IChangeStateRogueMethod Polymorph => _polymorph.Ref;



        [Space]
        [SerializeField] private ScriptRef<IApplyRogueMethod> _beApplied;
        public virtual IApplyRogueMethod BeApplied => _beApplied.Ref;

        [SerializeField] private ScriptRef<IApplyRogueMethod> _beThrown;
        public virtual IApplyRogueMethod BeThrown => _beThrown.Ref;

        [SerializeField] private ScriptRef<IApplyRogueMethod> _beEaten;
        public virtual IApplyRogueMethod BeEaten => _beEaten.Ref;

        [SerializeField] private ScriptRef<IApplyRogueMethod> _beSteppedOnAsTile;
        public virtual IApplyRogueMethod BeSteppedOnAsTile => _beSteppedOnAsTile.Ref;

        [Space]
        [SerializeField] private ScriptRef<IRaceWeight> _weight;
        [SerializeField] private ScriptRef<IRaceSprite> _sprite;

        [Header("WeaponRace")]
        [SerializeField] private ScriptRef<ISkill> _weaponAttack;
        public virtual ISkill WeaponAttack => _weaponAttack.Ref;

        [SerializeField] private ScriptRef<ISkill> _weaponThrow;
        public virtual ISkill WeaponThrow => _weaponThrow.Ref;

        [Header("AmmoRace")]
        [SerializeField] private ScriptRef<IApplyRogueMethod> _beShot;
        public virtual IApplyRogueMethod BeShot => _beShot.Ref;

        public float GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return _weight.Ref.GetWeight(raceOption, characterCreationData);
        }

        public void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out OchalikeBone mainBone, out AppearanceMorph morph)
        {
            _sprite.Ref.GetSpriteValues(raceOption, characterCreationData, gender, out mainBone, out morph);
        }

        public void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IReadOnlyOchalikeBone mainBone,
            out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            _sprite.Ref.GetObjSprite(raceOption, characterCreationData, gender, self, mainBone, out objSprite, out motionSet);
        }
    }
}
