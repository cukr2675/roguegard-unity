using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public abstract class BaseRaceOption : RaceOption
    {
        // Sources プロパティなどは _main ではなくこのクラスの継承によって変更させるため、 _main の型は変更不可にする。
        [SerializeField] private ObjectRaceOption _main = null;

        public override string Name => _main.Name;
        public override Sprite Icon => _main.Icon;
        public override Color Color => _main.Color;
        public override string Caption => _main.Caption;
        public override IRogueDetails Details => _main.Details;

        public override float Cost => _main.Cost;
        public override bool CostIsUnknown => _main.CostIsUnknown;

        public override Spanning<IRogueGender> Genders => _main != null ? _main.Genders : Spanning<IRogueGender>.Empty;

        public override IKeyword Category => _main.Category;

        public override int MaxHP => _main.MaxHP;
        public override int MaxMP => _main.MaxMP;
        public override int ATK => _main.ATK;
        public override int DEF => _main.DEF;
        public override float LoadCapacity => _main.LoadCapacity;
        public override ISerializableKeyword Faction => _main.Faction;
        public override Spanning<ISerializableKeyword> TargetFactions => _main.TargetFactions;
        public override MainInfoSetAbility Ability => _main.Ability;
        public override IRogueMaterial Material => _main.Material;
        public override Spanning<IWeightedRogueObjGeneratorList> LootTable => Spanning<IWeightedRogueObjGeneratorList>.Empty;

        public override IActiveRogueMethod Walk => _main.Walk;
        public override IActiveRogueMethod Wait => _main.Wait;
        public override ISkill Attack => _main.Attack;
        public override ISkill Throw => _main.Throw;
        public override IActiveRogueMethod PickUp => _main.PickUp;
        public override IActiveRogueMethod Put => _main.Put;
        public override IEatActiveRogueMethod Eat => _main.Eat;

        public override IAffectRogueMethod Hit => _main.Hit;
        public override IAffectRogueMethod BeDefeated => _main.BeDefeated;
        public override IChangeStateRogueMethod Locate => _main.Locate;
        public override IChangeStateRogueMethod Polymorph => _main.Polymorph;

        public override IApplyRogueMethod BeApplied => _main.BeApplied;
        public override IApplyRogueMethod BeThrown => _main.BeThrown;
        public override IApplyRogueMethod BeEaten => _main.BeEaten;

        public override IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return _main.Open(self, infoSetType, polymorph2Base, raceOption, characterCreationData);
        }

        public override void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            _main.Close(self, infoSetType, base2Polymorph, raceOption, characterCreationData);
        }

        public override IRaceOption Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return _main.Reopen(self, infoSetType, raceOption, characterCreationData);
        }

        public override IEquipmentState GetEquipmentState(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => _main.GetEquipmentState(self, raceOption, characterCreationData);

        public override IEquipmentInfo GetEquipmentInfo(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => _main.GetEquipmentInfo(self, raceOption, characterCreationData);

        public override float GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => _main.GetWeight(raceOption, characterCreationData);

        public override void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out BoneNodeBuilder mainNode, out AppearanceBoneSpriteTable boneSpriteTable)
            => _main.GetSpriteValues(raceOption, characterCreationData, gender, out mainNode, out boneSpriteTable);

        public override void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IBoneNode boneNode,
            out RogueObjSprite objSprite, out IMotionSet motionSet)
            => _main.GetObjSprite(raceOption, characterCreationData, gender, self, boneNode, out objSprite, out motionSet);

        public override void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => _main.InitializeObj(self, raceOption, characterCreationData);
    }
}
