using OchalikeSprites;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public abstract class BaseRaceOptionAsset : RaceOptionAsset
    {
        // Sources プロパティなどは _base ではなくこのクラスの継承によって変更させるため、 _base の型は変更不可にする。
        [SerializeField] private InlineRaceOption _base = null;

        public override string Name => _base.Name;
        public override Sprite Icon => _base.Icon;
        public override Color Color => _base.Color;
        public override string Caption => _base.Caption;
        public override IRogueDetails Details => _base.Details;
        public override Spanning<IKeyword> Tags => _base.Tags;

        public override float Cost => _base.Cost;
        public override bool CostIsUnknown => _base.CostIsUnknown;

        public override Spanning<IRogueGender> Genders => _base != null ? _base.Genders : Spanning<IRogueGender>.Empty;

        public override IKeyword Category => _base.Category;

        public override int MaxHp => _base.MaxHp;
        public override int MaxMp => _base.MaxMp;
        public override int Atk => _base.Atk;
        public override int Def => _base.Def;
        public override float LoadCapacity => _base.LoadCapacity;
        public override ISerializableKeyword Faction => _base.Faction;
        public override Spanning<ISerializableKeyword> TargetFactions => _base.TargetFactions;
        public override MainInfoSetAbility Ability => _base.Ability;
        public override IRogueMaterial Material => _base.Material;
        public override Spanning<IWeightedRogueObjGeneratorList> LootTable => Spanning<IWeightedRogueObjGeneratorList>.Empty;

        public override IActiveRogueMethod Walk => _base.Walk;
        public override IActiveRogueMethod Wait => _base.Wait;
        public override ISkill Attack => _base.Attack;
        public override ISkill Throw => _base.Throw;
        public override IActiveRogueMethod PickUp => _base.PickUp;
        public override IActiveRogueMethod Put => _base.Put;
        public override IEatActiveRogueMethod Eat => _base.Eat;

        public override IAffectRogueMethod Hit => _base.Hit;
        public override IAffectRogueMethod BeDefeated => _base.BeDefeated;
        public override IChangeStateRogueMethod Locate => _base.Locate;
        public override IChangeStateRogueMethod Polymorph => _base.Polymorph;

        public override IApplyRogueMethod BeApplied => _base.BeApplied;
        public override IApplyRogueMethod BeThrown => _base.BeThrown;
        public override IApplyRogueMethod BeEaten => _base.BeEaten;
        public override IApplyRogueMethod BeSteppedOnAsTile => _base.BeSteppedOnAsTile;

        public override IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return _base.Open(self, infoSetType, polymorph2Base, raceOption, characterCreationData);
        }

        public override void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            _base.Close(self, infoSetType, base2Polymorph, raceOption, characterCreationData);
        }

        public override IRaceOption Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return _base.Reopen(self, infoSetType, raceOption, characterCreationData);
        }

        public override IEquipmentState GetEquipmentState(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => _base.GetEquipmentState(self, raceOption, characterCreationData);

        public override IEquipmentInfo GetEquipmentInfo(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => _base.GetEquipmentInfo(self, raceOption, characterCreationData);

        public override float GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => _base.GetWeight(raceOption, characterCreationData);

        public override void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out OchalikeBone mainBone, out AppearanceMorph morph)
            => _base.GetSpriteValues(raceOption, characterCreationData, gender, out mainBone, out morph);

        public override void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self,
            IReadOnlyOchalikeBone mainBone, out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
            => _base.GetObjSprite(raceOption, characterCreationData, gender, self, mainBone, out objSprite, out motionSet);

        public override void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => _base.InitializeObj(self, raceOption, characterCreationData);
    }
}
