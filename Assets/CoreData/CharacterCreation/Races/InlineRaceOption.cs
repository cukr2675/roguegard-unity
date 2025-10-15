using OchalikeSprites;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    [Objforming.IgnoreRequireRelationalComponent]
    public class InlineRaceOption : IRaceOption
    {
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;

        [SerializeField] private Color _color;
        public Color Color => _color;

        [SerializeField] private string _caption;
        public string Caption => _caption;

        [SerializeField] private ScriptRef<IRogueDetails> _details;
        public IRogueDetails Details => _details.Ref;

        [SerializeField] private KeywordAsset[] _tags;
        public Spanning<IKeyword> Tags => _tags;



        Spanning<IMemberSource> IRaceOption.MemberSources => Spanning<IMemberSource>.Empty;

        public virtual Spanning<IRaceOption> GrowingOptions => Spanning<IRaceOption>.Empty;



        [Space]
        [SerializeField] private float _cost;
        public float Cost => _cost;

        [SerializeField] private bool _costIsUnknown;
        public bool CostIsUnknown => _costIsUnknown;



        [Space]
        [SerializeField] private RogueGenderListAsset _genders;
        public Spanning<IRogueGender> Genders => _genders ? _genders.Span : RoguegardSettings.DefaultRaceOption.Genders;



        [Space]
        [SerializeField] private KeywordAsset _category;
        public IKeyword Category => _category ? _category : RoguegardSettings.DefaultRaceOption.Category;



        [Space]
        [SerializeField] private int _maxHp;
        public int MaxHp => _maxHp;

        [SerializeField] private int _maxMp;
        public int MaxMp => _maxMp;

        [SerializeField] private int _atk;
        public int Atk => _atk;

        [SerializeField] private int _def;
        public int Def => _def;

        [SerializeField] private float _loadCapacity;
        public float LoadCapacity => _loadCapacity;

        [SerializeField] private FactionAsset _faction;
        public ISerializableKeyword Faction => _faction ? _faction.Faction : RoguegardSettings.DefaultRaceOption.Faction;
        public Spanning<ISerializableKeyword> TargetFactions => _faction ? _faction.TargetFactions : RoguegardSettings.DefaultRaceOption.TargetFactions;

        [SerializeField] private MainInfoSetAbility _ability;
        public MainInfoSetAbility Ability => _ability;

        [SerializeField] private RogueMaterialAsset _material;
        public IRogueMaterial Material => _material ? _material : RoguegardSettings.DefaultRaceOption.Material;

        [SerializeField] private AssetStartingItemList[] _lootTable;
        public Spanning<IWeightedRogueObjGeneratorList> LootTable => _lootTable;



        [Space]
        [SerializeField] private ScriptRef<IActiveRogueMethod> _walk;
        public virtual IActiveRogueMethod Walk => _walk.Ref ?? RoguegardSettings.DefaultRaceOption.Walk;

        [SerializeField] private ScriptRef<IActiveRogueMethod> _wait;
        public virtual IActiveRogueMethod Wait => _wait.Ref ?? RoguegardSettings.DefaultRaceOption.Wait;

        [SerializeField] private ScriptRef<ISkill> _attack;
        public virtual ISkill Attack => _attack.Ref ?? RoguegardSettings.DefaultRaceOption.Attack;

        [SerializeField] private ScriptRef<ISkill> _throw;
        public virtual ISkill Throw => _throw.Ref ?? RoguegardSettings.DefaultRaceOption.Throw;

        [SerializeField] private ScriptRef<IActiveRogueMethod> _pickUp;
        public virtual IActiveRogueMethod PickUp => _pickUp.Ref ?? RoguegardSettings.DefaultRaceOption.PickUp;

        [SerializeField] private ScriptRef<IActiveRogueMethod> _put;
        public virtual IActiveRogueMethod Put => _put.Ref ?? RoguegardSettings.DefaultRaceOption.Put;

        [SerializeField] private ScriptRef<IEatActiveRogueMethod> _eat;
        public virtual IEatActiveRogueMethod Eat => _eat.Ref ?? RoguegardSettings.DefaultRaceOption.Eat;



        [Space]
        [SerializeField] private ScriptRef<IAffectRogueMethod> _hit;
        public virtual IAffectRogueMethod Hit => _hit.Ref ?? RoguegardSettings.DefaultRaceOption.Hit;

        [SerializeField] private ScriptRef<IAffectRogueMethod> _beDefeated;
        public virtual IAffectRogueMethod BeDefeated => _beDefeated.Ref ?? RoguegardSettings.DefaultRaceOption.BeDefeated;

        [SerializeField] private ScriptRef<IChangeStateRogueMethod> _locate;
        public virtual IChangeStateRogueMethod Locate => _locate.Ref ?? RoguegardSettings.DefaultRaceOption.Locate;

        [SerializeField] private ScriptRef<IChangeStateRogueMethod> _polymorph;
        public virtual IChangeStateRogueMethod Polymorph => _polymorph.Ref ?? RoguegardSettings.DefaultRaceOption.Polymorph;



        [Space]
        [SerializeField] private ScriptRef<IApplyRogueMethod> _beApplied;
        public virtual IApplyRogueMethod BeApplied => _beApplied.Ref ?? RoguegardSettings.DefaultRaceOption.BeApplied;

        [SerializeField] private ScriptRef<IApplyRogueMethod> _beThrown;
        public virtual IApplyRogueMethod BeThrown => _beThrown.Ref ?? RoguegardSettings.DefaultRaceOption.BeThrown;

        [SerializeField] private ScriptRef<IApplyRogueMethod> _beEaten;
        public virtual IApplyRogueMethod BeEaten => _beEaten.Ref ?? RoguegardSettings.DefaultRaceOption.BeEaten;

        [SerializeField] private ScriptRef<IApplyRogueMethod> _beSteppedOnAsTile;
        public virtual IApplyRogueMethod BeSteppedOnAsTile => _beSteppedOnAsTile.Ref ?? RoguegardSettings.DefaultRaceOption.BeSteppedOnAsTile;



        [Space]
        [SerializeField] private EquipmentStateAsset _equipmentState;
        [SerializeField] private ScriptRef<IRaceWeight> _weight;
        [SerializeField] private ScriptRef<IRaceSprite> _sprite;
        [SerializeField] private ScriptRef<IOpenEffect>[] _openEffects;
        public Spanning<ScriptRef<IOpenEffect>> OpenEffectSources => _openEffects;

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
            out OchalikeBone mainBone, out AppearanceMorph morph)
        {
            if (_sprite.Ref != null)
            {
                _sprite.Ref.GetSpriteValues(
                    raceOption, characterCreationData, gender, out mainBone, out morph);
            }
            else
            {
                RoguegardSettings.DefaultRaceOption.GetSpriteValues(
                    raceOption, characterCreationData, gender, out mainBone, out morph);
            }
        }

        public void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self,
            IReadOnlyOchalikeBone mainBone, out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            if (_sprite.Ref != null)
            {
                _sprite.Ref.GetObjSprite(raceOption, characterCreationData, gender, self, mainBone, out objSprite, out motionSet);
            }
            else
            {
                RoguegardSettings.DefaultRaceOption.GetObjSprite(raceOption, characterCreationData, gender, self, mainBone, out objSprite, out motionSet);
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
