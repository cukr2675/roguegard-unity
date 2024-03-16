using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    [ObjectFormer.IgnoreRequireRelationalComponent]
    public class ObjectRaceOption : IRaceOption
    {
        [Header("RaceOption")]

        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;

        [SerializeField] private Color _color;
        public Color Color => _color;

        [SerializeField] private string _caption;
        public string Caption => _caption;

        [SerializeField] private ScriptField<IRogueDetails> _details;
        public IRogueDetails Details => _details.Ref;



        Spanning<IMemberSource> IMemberableOption.MemberSources => Spanning<IMemberSource>.Empty;

        public virtual Spanning<IRaceOption> GrowingOptions => Spanning<IRaceOption>.Empty;



        [Space]
        [SerializeField] private float _cost;
        public float Cost => _cost;

        [SerializeField] private bool _costIsUnknown;
        public bool CostIsUnknown => _costIsUnknown;



        [Space]
        [SerializeField] private RogueGenderList _genders;
        public Spanning<IRogueGender> Genders => _genders ?? RoguegardSettings.DefaultRaceOption.Genders;



        [Space]
        [SerializeField] private KeywordData _category;
        public IKeyword Category => _category ?? RoguegardSettings.DefaultRaceOption.Category;



        [Space]
        [SerializeField] private int _maxHP;
        public int MaxHP => _maxHP;

        [SerializeField] private int _maxMP;
        public int MaxMP => _maxMP;

        [SerializeField] private int _atk;
        public int ATK => _atk;

        [SerializeField] private int _def;
        public int DEF => _def;

        [SerializeField] private float _loadCapacity;
        public float LoadCapacity => _loadCapacity;

        [SerializeField] private ScriptableFaction _faction;
        public ISerializableKeyword Faction => _faction?.Faction ?? RoguegardSettings.DefaultRaceOption.Faction;
        public Spanning<ISerializableKeyword> TargetFactions => _faction != null ? _faction.TargetFactions : RoguegardSettings.DefaultRaceOption.TargetFactions;

        [SerializeField] private MainInfoSetAbility _ability;
        public MainInfoSetAbility Ability => _ability;

        [SerializeField] private RogueMaterial _material;
        public IRogueMaterial Material => _material ?? RoguegardSettings.DefaultRaceOption.Material;

        [SerializeField] private ScriptableStartingItemList[] _lootTable;
        public Spanning<IWeightedRogueObjGeneratorList> LootTable => _lootTable;



        [Space]
        [SerializeField] private ScriptField<IActiveRogueMethod> _walk;
        public virtual IActiveRogueMethod Walk => _walk.Ref ?? RoguegardSettings.DefaultRaceOption.Walk;

        [SerializeField] private ScriptField<IActiveRogueMethod> _wait;
        public virtual IActiveRogueMethod Wait => _wait.Ref ?? RoguegardSettings.DefaultRaceOption.Wait;

        [SerializeField] private ScriptField<ISkill> _attack;
        public virtual ISkill Attack => _attack.Ref ?? RoguegardSettings.DefaultRaceOption.Attack;

        [SerializeField] private ScriptField<ISkill> _throw;
        public virtual ISkill Throw => _throw.Ref ?? RoguegardSettings.DefaultRaceOption.Throw;

        [SerializeField] private ScriptField<IActiveRogueMethod> _pickUp;
        public virtual IActiveRogueMethod PickUp => _pickUp.Ref ?? RoguegardSettings.DefaultRaceOption.PickUp;

        [SerializeField] private ScriptField<IActiveRogueMethod> _put;
        public virtual IActiveRogueMethod Put => _put.Ref ?? RoguegardSettings.DefaultRaceOption.Put;

        [SerializeField] private ScriptField<IEatActiveRogueMethod> _eat;
        public virtual IEatActiveRogueMethod Eat => _eat.Ref ?? RoguegardSettings.DefaultRaceOption.Eat;



        [Space]
        [SerializeField] private ScriptField<IAffectRogueMethod> _hit;
        public virtual IAffectRogueMethod Hit => _hit.Ref ?? RoguegardSettings.DefaultRaceOption.Hit;

        [SerializeField] private ScriptField<IAffectRogueMethod> _beDefeated;
        public virtual IAffectRogueMethod BeDefeated => _beDefeated.Ref ?? RoguegardSettings.DefaultRaceOption.BeDefeated;

        [SerializeField] private ScriptField<IChangeStateRogueMethod> _locate;
        public virtual IChangeStateRogueMethod Locate => _locate.Ref ?? RoguegardSettings.DefaultRaceOption.Locate;

        [SerializeField] private ScriptField<IChangeStateRogueMethod> _polymorph;
        public virtual IChangeStateRogueMethod Polymorph => _polymorph.Ref ?? RoguegardSettings.DefaultRaceOption.Polymorph;



        [Space]
        [SerializeField] private ScriptField<IApplyRogueMethod> _beApplied;
        public virtual IApplyRogueMethod BeApplied => _beApplied.Ref ?? RoguegardSettings.DefaultRaceOption.BeApplied;

        [SerializeField] private ScriptField<IApplyRogueMethod> _beThrown;
        public virtual IApplyRogueMethod BeThrown => _beThrown.Ref ?? RoguegardSettings.DefaultRaceOption.BeThrown;

        [SerializeField] private ScriptField<IApplyRogueMethod> _beEaten;
        public virtual IApplyRogueMethod BeEaten => _beEaten.Ref ?? RoguegardSettings.DefaultRaceOption.BeEaten;



        [Space]
        [SerializeField] private EquipmentStateData _equipmentState;
        [SerializeField] private ScriptField<IRaceOptionWeight> _weight;
        [SerializeField] private ScriptField<IRaceOptionSprite> _sprite;
        [SerializeField] private ScriptField<IOpenEffect>[] _openEffects;
        public Spanning<ScriptField<IOpenEffect>> OpenEffectSources => _openEffects;

        public virtual IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            IRaceOption result = raceOption;
            foreach (var effect in _openEffects)
            {
                result = effect.Ref.Open(self, infoSetType, polymorph2Base, raceOption, characterCreationData);
            }
            return result;
        }

        public virtual void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            foreach (var effect in _openEffects)
            {
                effect.Ref.Close(self, infoSetType, base2Polymorph, raceOption, characterCreationData);
            }
        }

        public virtual IRaceOption Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            IRaceOption result = raceOption;
            foreach (var effect in _openEffects)
            {
                result = effect.Ref.Reopen(self, infoSetType, raceOption, characterCreationData);
            }
            return result;
        }

        public IEquipmentState GetEquipmentState(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            if (_equipmentState != null) return DataEquipmentState.CreateOrReuse(self, _equipmentState);
            else return null;
        }

        public virtual IEquipmentInfo GetEquipmentInfo(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData) => null;

        public float GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            if (_weight.Ref != null)
            {
                return _weight.Ref.GetWeight(raceOption, characterCreationData);
            }
            else
            {
                return RoguegardSettings.DefaultRaceOption.GetWeight(raceOption, characterCreationData);
            }
        }

        public void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out BoneNodeBuilder mainNode, out AppearanceBoneSpriteTable boneSpriteTable)
        {
            if (_sprite.Ref != null)
            {
                _sprite.Ref.GetSpriteValues(raceOption, characterCreationData, gender, out mainNode, out boneSpriteTable);
            }
            else
            {
                RoguegardSettings.DefaultRaceOption.GetSpriteValues(raceOption, characterCreationData, gender, out mainNode, out boneSpriteTable);
            }
        }

        public void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IBoneNode boneNode,
            out RogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            if (_sprite.Ref != null)
            {
                _sprite.Ref.GetObjSprite(raceOption, characterCreationData, gender, self, boneNode, out objSprite, out motionSet);
            }
            else
            {
                RoguegardSettings.DefaultRaceOption.GetObjSprite(raceOption, characterCreationData, gender, self, boneNode, out objSprite, out motionSet);
            }
        }

        void IRaceOption.UpdateMemberRange(IMember member, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            foreach (var effect in _openEffects)
            {
                effect.Ref.InitializeObj(self, raceOption, characterCreationData);
            }
        }
    }
}
