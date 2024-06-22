using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// このアイテム・キャラクターが <see cref="IRaceOption"/> として参照できることを示すクラス
    /// </summary>
    public abstract class RaceOptionalCreationData : ScriptableCharacterCreationData, IRaceOption
    {
        Spanning<IRaceOption> IRaceOption.GrowingOptions => Race.Option.GrowingOptions;

        Spanning<IRogueGender> IRaceOption.Genders => Race.Option.Genders;

        IKeyword IRaceOption.Category => Race.Option.Category;

        int IRaceOption.MaxHP => Race.Option.MaxHP;
        int IRaceOption.MaxMP => Race.Option.MaxMP;
        int IRaceOption.ATK => Race.Option.ATK;
        int IRaceOption.DEF => Race.Option.DEF;
        float IRaceOption.LoadCapacity => Race.Option.LoadCapacity;
        ISerializableKeyword IRaceOption.Faction => Race.Option.Faction;
        Spanning<ISerializableKeyword> IRaceOption.TargetFactions => Race.Option.TargetFactions;

        MainInfoSetAbility IRaceOption.Ability => Race.Option.Ability;
        IRogueMaterial IRaceOption.Material => Race.Option.Material;

        Spanning<IWeightedRogueObjGeneratorList> IRaceOption.LootTable => Race.Option.LootTable;

        IActiveRogueMethod IRaceOption.Walk => Race.Option.Walk;
        IActiveRogueMethod IRaceOption.Wait => Race.Option.Wait;
        ISkill IRaceOption.Attack => Race.Option.Attack;
        ISkill IRaceOption.Throw => Race.Option.Throw;
        IActiveRogueMethod IRaceOption.PickUp => Race.Option.PickUp;
        IActiveRogueMethod IRaceOption.Put => Race.Option.Put;
        IEatActiveRogueMethod IRaceOption.Eat => Race.Option.Eat;

        IAffectRogueMethod IRaceOption.Hit => Race.Option.Hit;
        IAffectRogueMethod IRaceOption.BeDefeated => Race.Option.BeDefeated;
        IChangeStateRogueMethod IRaceOption.Locate => Race.Option.Locate;
        IChangeStateRogueMethod IRaceOption.Polymorph => Race.Option.Polymorph;

        IApplyRogueMethod IRaceOption.BeApplied => Race.Option.BeApplied;
        IApplyRogueMethod IRaceOption.BeThrown => Race.Option.BeThrown;
        IApplyRogueMethod IRaceOption.BeEaten => Race.Option.BeEaten;

        public virtual Spanning<IMemberSource> RaceOptionMemberSources => Spanning<IMemberSource>.Empty;
        Spanning<IMemberSource> IRaceOption.MemberSources => RaceOptionMemberSources;

        IRaceOption IRaceOption.Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Race.Option.Open(self, infoSetType, polymorph2Base, raceOption, characterCreationData);

        IRaceOption IRaceOption.Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Race.Option.Reopen(self, infoSetType, raceOption, characterCreationData);

        void IRaceOption.Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Race.Option.Close(self, infoSetType, base2Polymorph, raceOption, characterCreationData);

        IEquipmentState IRaceOption.GetEquipmentState(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Race.Option.GetEquipmentState(self, raceOption, characterCreationData);

        IEquipmentInfo IRaceOption.GetEquipmentInfo(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Race.Option.GetEquipmentInfo(self, raceOption, characterCreationData);

        float IRaceOption.GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Race.Option.GetWeight(raceOption, characterCreationData);

        void IRaceOption.GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out NodeBone mainNode, out AppearanceBoneSpriteTable boneSpriteTable)
            => Race.Option.GetSpriteValues(raceOption, characterCreationData, gender, out mainNode, out boneSpriteTable);

        void IRaceOption.GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IReadOnlyNodeBone nodeBone,
            out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
            => Race.Option.GetObjSprite(raceOption, characterCreationData, gender, self, nodeBone, out objSprite, out motionSet);

        void IRaceOption.UpdateMemberRange(IMember member, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Race.Option.UpdateMemberRange(member, raceOption, characterCreationData);

        void IRaceOption.InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Race.Option.InitializeObj(self, raceOption, characterCreationData);
    }
}
