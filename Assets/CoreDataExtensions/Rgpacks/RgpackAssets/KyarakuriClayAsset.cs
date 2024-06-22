using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;
using Roguegard.CharacterCreation;

namespace Roguegard.Rgpacks
{
    public class KyarakuriClayAsset : IReadOnlyRace
    {
        public KyarakuriClayReference Reference { get; }

        private readonly KyarakuriClayInfo info;
        private readonly PropertiedCmnReference raceWeightCmn;
        private readonly PropertiedCmnReference raceSpriteCmn;

        IRaceOption IReadOnlyRace.Option => Reference;
        string IReadOnlyRace.OptionName => null;
        Color IReadOnlyRace.BodyColor => Color;
        string IReadOnlyRace.OptionCaption => null;
        IRogueDetails IReadOnlyRace.OptionDetails => null;
        Spanning<IMemberSource> IMemberable.MemberSources => RaceOptionMemberSources;

        public int Lv => 0;
        public IRogueGender Gender => RoguegardSettings.DefaultRaceOption.Genders[0];
        public string HPName => null;
        public string MPName => null;
        private MemberList _members = new MemberList();

        public string Name => info.Name;
        public Sprite Icon => null;
        public Color Color => Color.white;
        public string Caption => null;
        public IRogueDetails Details => null;

        public IKeyword Category => null;

        public int MaxHP => info.MaxHP;
        public int MaxMP => info.MaxMP;
        public int ATK => info.ATK;
        public int DEF => info.DEF;
        public float LoadCapacity => info.LoadCapacity;

        public ISerializableKeyword Faction => null;
        public Spanning<ISerializableKeyword> TargetFactions => Spanning<ISerializableKeyword>.Empty;

        public MainInfoSetAbility Ability => MainInfoSetAbility.Character;
        public IRogueMaterial Material => null;

        public float Cost => 0f;
        public bool CostIsUnknown => true;

        public Spanning<IWeightedRogueObjGeneratorList> LootTable => Spanning<IWeightedRogueObjGeneratorList>.Empty;

        public IActiveRogueMethod Walk { get; }
        public IActiveRogueMethod Wait { get; }
        public ISkill Attack { get; }
        public ISkill Throw { get; }
        public IActiveRogueMethod PickUp { get; }
        public IActiveRogueMethod Put { get; }
        public IEatActiveRogueMethod Eat => null;

        public IAffectRogueMethod Hit { get; }
        public IAffectRogueMethod BeDefeated { get; }
        public IChangeStateRogueMethod Locate { get; }
        public IChangeStateRogueMethod Polymorph { get; }

        public IApplyRogueMethod BeApplied { get; }
        public IApplyRogueMethod BeThrown { get; }
        public IApplyRogueMethod BeEaten { get; }

        public Spanning<IRaceOption> GrowingOptions => Spanning<IRaceOption>.Empty;
        public Spanning<IRogueGender> Genders => RoguegardSettings.DefaultRaceOption.Genders;
        public Spanning<IMemberSource> RaceOptionMemberSources => Spanning<IMemberSource>.Empty;
        public Spanning<IMemberSource> StartingItemOptionMemberSources => Spanning<IMemberSource>.Empty;

        public KyarakuriClayAsset(KyarakuriClayInfo info, string envRgpackID, string fullID)
        {
            this.info = info;
            Reference = new KyarakuriClayReference(fullID, envRgpackID);
            if (info.RaceWeight != null) { raceWeightCmn = new PropertiedCmnReference(info.RaceWeight, envRgpackID); }
            if (info.RaceSprite != null) { raceSpriteCmn = new PropertiedCmnReference(info.RaceSprite, envRgpackID); }

            Walk = RogueMethod.Create(info.Walk, envRgpackID, RoguegardSettings.DefaultRaceOption.Walk);
            Wait = RogueMethod.Create(info.Wait, envRgpackID, RoguegardSettings.DefaultRaceOption.Wait);
            Attack = PropertiedCmnSkill.Create(info.Attack, envRgpackID, RoguegardSettings.DefaultRaceOption.Attack);
            Throw = PropertiedCmnSkill.Create(info.Throw, envRgpackID, RoguegardSettings.DefaultRaceOption.Throw);
            PickUp = RogueMethod.Create(info.PickUp, envRgpackID, RoguegardSettings.DefaultRaceOption.PickUp);
            Put = RogueMethod.Create(info.Put, envRgpackID, RoguegardSettings.DefaultRaceOption.Put);

            Hit = RogueMethod.Create(info.Hit, envRgpackID, RoguegardSettings.DefaultRaceOption.Hit);
            BeDefeated = RogueMethod.Create(info.BeDefeated, envRgpackID, RoguegardSettings.DefaultRaceOption.BeDefeated);
            Locate = RogueMethod.Create(info.Locate, envRgpackID, RoguegardSettings.DefaultRaceOption.Locate);
            Polymorph = RogueMethod.Create(info.Polymorph, envRgpackID, RoguegardSettings.DefaultRaceOption.Polymorph);

            BeApplied = RogueMethod.Create(info.BeApplied, envRgpackID, RoguegardSettings.DefaultRaceOption.BeApplied);
            BeThrown = RogueMethod.Create(info.BeThrown, envRgpackID, RoguegardSettings.DefaultRaceOption.BeThrown);
            BeEaten = RogueMethod.Create(info.BeEaten, envRgpackID, RoguegardSettings.DefaultRaceOption.BeEaten);
        }

        public IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return Reference;
        }

        public IRaceOption Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return Reference;
        }

        public void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        public IEquipmentState GetEquipmentState(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return null;
        }

        public IEquipmentInfo GetEquipmentInfo(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return null;
        }

        public float GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return System.Convert.ToSingle(raceWeightCmn.Invoke());
        }

        public void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out NodeBone mainNode, out AppearanceBoneSpriteTable boneSpriteTable)
        {
            if (raceSpriteCmn != null)
            {
                var tuple = ((object, object))raceSpriteCmn.Invoke();
                mainNode = (NodeBone)tuple.Item1;
                boneSpriteTable = (AppearanceBoneSpriteTable)tuple.Item2;
            }
            else
            {
                RoguegardSettings.DefaultRaceOption.GetSpriteValues(raceOption, characterCreationData, gender, out mainNode, out boneSpriteTable);
            }
        }

        public void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IReadOnlyNodeBone nodeBone,
            out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            if (raceSpriteCmn != null)
            {
                var tuple = ((object, object))raceSpriteCmn.Invoke();
                objSprite = (IRogueObjSprite)tuple.Item1;
                motionSet = (ISpriteMotionSet)tuple.Item2;
            }
            else
            {
                RoguegardSettings.DefaultRaceOption.GetObjSprite(raceOption, characterCreationData, gender, self, nodeBone, out objSprite, out motionSet);
            }
        }

        public void RaceOptionUpdateMemberRange(IMember member, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        public void StartingItemOptionUpdateMemberRange(IMember member, IReadOnlyStartingItem startingItem, ICharacterCreationData characterCreationData)
        {
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        IReadOnlyMember IMemberable.GetMember(IMemberSource source)
        {
            var members = (Spanning<IMember>)_members;
            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                if (member.Source == source) return member;
            }
            throw new System.ArgumentException($"{source} ‚Ì {nameof(IMember)} ‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñB");
        }

        private class RogueMethod : IActiveRogueMethod, IApplyRogueMethod, IAffectRogueMethod, IChangeStateRogueMethod
        {
            private readonly PropertiedCmnReference reference;

            public string Name => null;
            public Sprite Icon => null;
            public Color Color => Color.white;
            public string Caption => null;
            public IRogueDetails Details => null;

            public IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
            public IRogueMethodRange Range => FrontRogueMethodRange.Instance;
            public int RequiredMP => 0;
            public Spanning<IKeyword> AmmoCategories => Spanning<IKeyword>.Empty;

            private RogueMethod(PropertiedCmnData data, string envRgpackID)
            {
                reference = new PropertiedCmnReference(data, envRgpackID);
            }

            public static IActiveRogueMethod Create(PropertiedCmnData data, string envRgpackID, IActiveRogueMethod defaultMethod)
            {
                return string.IsNullOrWhiteSpace(data.Cmn) ? defaultMethod : new RogueMethod(data, envRgpackID);
            }

            public static IApplyRogueMethod Create(PropertiedCmnData data, string envRgpackID, IApplyRogueMethod defaultMethod)
            {
                return string.IsNullOrWhiteSpace(data.Cmn) ? defaultMethod : new RogueMethod(data, envRgpackID);
            }

            public static IAffectRogueMethod Create(PropertiedCmnData data, string envRgpackID, IAffectRogueMethod defaultMethod)
            {
                return string.IsNullOrWhiteSpace(data.Cmn) ? defaultMethod : new RogueMethod(data, envRgpackID);
            }

            public static IChangeStateRogueMethod Create(PropertiedCmnData data, string envRgpackID, IChangeStateRogueMethod defaultMethod)
            {
                return string.IsNullOrWhiteSpace(data.Cmn) ? defaultMethod : new RogueMethod(data, envRgpackID);
            }

            public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                var result = reference.Invoke();
                return result == null || result is bool boolean && boolean == true;
            }

            public int GetATK(RogueObj self, out bool additionalEffect)
            {
                additionalEffect = false;
                return 0;
            }

            public bool Equals(ISkill other)
            {
                return other == this;
            }
        }
    }
}
