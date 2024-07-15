using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;
using Roguegard.CharacterCreation;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class KyarakuriClayReference : RgpackReference<KyarakuriClayAsset>, ICharacterCreationData, IRaceOption, IStartingItemOption
    {
        [System.NonSerialized] private GrowingInfoSetTable _infoSets;
        [System.NonSerialized] private ISortedIntrinsicList _sortedIntrinsics;

        private GrowingInfoSetTable InfoSets => _infoSets ??= new GrowingInfoSetTable(this);
        IReadOnlyRace ICharacterCreationData.Race => Asset;
        Spanning<IReadOnlyAppearance> ICharacterCreationData.Appearances => Spanning<IReadOnlyAppearance>.Empty;
        ISortedIntrinsicList ICharacterCreationData.SortedIntrinsics => _sortedIntrinsics ??= new SortedIntrinsicList(new IReadOnlyIntrinsic[0], this);
        Spanning<IWeightedRogueObjGeneratorList> ICharacterCreationData.StartingItemTable => Spanning<IWeightedRogueObjGeneratorList>.Empty;

        string IRogueDescription.Name => Asset.Name;
        Sprite IRogueDescription.Icon => Asset.Icon;
        Color IRogueDescription.Color => Asset.Color;
        string IRogueDescription.Caption => Asset.Caption;
        IRogueDetails IRogueDescription.Details => Asset.Details;

        float ICharacterCreationData.Cost => Asset.Cost;
        float IRaceOption.Cost => Asset.Cost;
        bool ICharacterCreationData.CostIsUnknown => Asset.CostIsUnknown;
        bool IRaceOption.CostIsUnknown => Asset.CostIsUnknown;

        Spanning<IRaceOption> IRaceOption.GrowingOptions => Asset.GrowingOptions;

        Spanning<IRogueGender> IRaceOption.Genders => Asset.Genders;

        IKeyword IRaceOption.Category => Asset.Category;

        int IRaceOption.MaxHP => Asset.MaxHP;
        int IRaceOption.MaxMP => Asset.MaxMP;
        int IRaceOption.ATK => Asset.ATK;
        int IRaceOption.DEF => Asset.DEF;
        float IRaceOption.LoadCapacity => Asset.LoadCapacity;
        ISerializableKeyword IRaceOption.Faction => Asset.Faction;
        Spanning<ISerializableKeyword> IRaceOption.TargetFactions => Asset.TargetFactions;

        MainInfoSetAbility IRaceOption.Ability => Asset.Ability;
        IRogueMaterial IRaceOption.Material => Asset.Material;

        Spanning<IWeightedRogueObjGeneratorList> IRaceOption.LootTable => Asset.LootTable;

        IActiveRogueMethod IRaceOption.Walk => Asset.Walk;
        IActiveRogueMethod IRaceOption.Wait => Asset.Wait;
        ISkill IRaceOption.Attack => Asset.Attack;
        ISkill IRaceOption.Throw => Asset.Throw;
        IActiveRogueMethod IRaceOption.PickUp => Asset.PickUp;
        IActiveRogueMethod IRaceOption.Put => Asset.Put;
        IEatActiveRogueMethod IRaceOption.Eat => Asset.Eat;

        IAffectRogueMethod IRaceOption.Hit => Asset.Hit;
        IAffectRogueMethod IRaceOption.BeDefeated => Asset.BeDefeated;
        IChangeStateRogueMethod IRaceOption.Locate => Asset.Locate;
        IChangeStateRogueMethod IRaceOption.Polymorph => Asset.Polymorph;

        IApplyRogueMethod IRaceOption.BeApplied => Asset.BeApplied;
        IApplyRogueMethod IRaceOption.BeThrown => Asset.BeThrown;
        IApplyRogueMethod IRaceOption.BeEaten => Asset.BeEaten;

        Spanning<IMemberSource> IRaceOption.MemberSources => Asset.RaceOptionMemberSources;
        Spanning<IMemberSource> IStartingItemOption.MemberSources => Asset.StartingItemOptionMemberSources;

        int IStartingItemOption.Lv => Asset.Lv;

        public IMainInfoSet PrimaryInfoSet
        {
            get
            {
                var primaryGender = Asset.Gender ?? Asset.Genders[0];
                return InfoSets[this, primaryGender];
            }
        }
        IMainInfoSet IStartingItemOption.InfoSet => PrimaryInfoSet;

        public virtual Spanning<IWeightedRogueObjGeneratorList> StartingItemTable => Spanning<IWeightedRogueObjGeneratorList>.Empty;

        private KyarakuriClayReference() { }

        public KyarakuriClayReference(string id, string envRgpackID)
            : base(id, envRgpackID)
        {
        }

        IRaceOption IRaceOption.Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Asset.Open(self, infoSetType, polymorph2Base, raceOption, characterCreationData);

        IRaceOption IRaceOption.Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Asset.Reopen(self, infoSetType, raceOption, characterCreationData);

        void IRaceOption.Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Asset.Close(self, infoSetType, base2Polymorph, raceOption, characterCreationData);

        IEquipmentState IRaceOption.GetEquipmentState(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Asset.GetEquipmentState(self, raceOption, characterCreationData);

        IEquipmentInfo IRaceOption.GetEquipmentInfo(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Asset.GetEquipmentInfo(self, raceOption, characterCreationData);

        float IRaceOption.GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Asset.GetWeight(raceOption, characterCreationData);

        void IRaceOption.GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out NodeBone mainNode, out AppearanceBoneSpriteTable boneSpriteTable)
            => Asset.GetSpriteValues(raceOption, characterCreationData, gender, out mainNode, out boneSpriteTable);

        void IRaceOption.GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IReadOnlyNodeBone nodeBone,
            out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
            => Asset.GetObjSprite(raceOption, characterCreationData, gender, self, nodeBone, out objSprite, out motionSet);

        void IRaceOption.UpdateMemberRange(IMember member, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Asset.RaceOptionUpdateMemberRange(member, raceOption, characterCreationData);

        void IRaceOption.InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
            => Asset.InitializeObj(self, raceOption, characterCreationData);

        void IStartingItemOption.UpdateMemberRange(IMember member, IReadOnlyStartingItem startingItem, ICharacterCreationData characterCreationData)
            => Asset.StartingItemOptionUpdateMemberRange(member, startingItem, characterCreationData);

        float IStartingItemOption.GetCost(IReadOnlyStartingItem startingItem, out bool costIsUnknown)
        {
            costIsUnknown = Asset.CostIsUnknown;
            return Asset.Cost;
        }

        bool ICharacterCreationData.TryGetGrowingInfoSet(IRaceOption raceOption, IRogueGender gender, out IMainInfoSet growingInfoSet)
        {
            if (raceOption == null) throw new System.ArgumentNullException(nameof(raceOption));
            if (gender == null) throw new System.ArgumentNullException(nameof(gender));

            var result = InfoSets.TryGetValue(raceOption, gender, out var infoSet);
            growingInfoSet = infoSet;
            return result;
        }

        public RogueObj CreateObj(RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            if (!InfoSets.TryGetValue(this, Asset.Gender, out var infoSet)) throw new RogueException();

            return infoSet.CreateObj(location, position, random, stackOption);
        }

        RogueObj IStartingItemOption.CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption)
        {
            return CreateObj(location, position, random, stackOption);
        }

        public override bool Equals(object obj)
        {
            return obj is KyarakuriClayReference reference && reference.FullID == FullID;
        }

        public override int GetHashCode()
        {
            return FullID.GetHashCode();
        }
    }
}
