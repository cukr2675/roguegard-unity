using OchalikeSprites;
using Roguegard.CharacterCreation;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public class KyarakuriClayAsset : IReadOnlyRace
    {
        public KyarakuriClayReference Reference { get; }

        private readonly KyarakuriClayInfo info;
        private readonly PropertiedCmnReference raceWeightCmn;
        private readonly PropertiedCmnReference raceSpriteCmn;

        IRaceOption IReadOnlyRace.Option => Reference;
        string IReadOnlyRace.CustomName => null;
        Color IReadOnlyRace.BodyColor => Color;
        string IReadOnlyRace.CustomCaption => null;
        IRogueDetails IReadOnlyRace.CustomDetails => null;
        Spanning<IMemberSource> IReadOnlyMemberable.MemberSources => RaceOptionMemberSources;

        public int Lv => 0;
        public IRogueGender Gender => RoguegardSettings.DefaultRaceOption.Genders[0];
        public string HpName => null;
        public string MpName => null;
        private readonly MemberList _members = new();

        public string Name => info.Name;
        public Sprite Icon => null;
        public Color Color => Color.white;
        public string Caption => null;
        public IRogueDetails Details => null;
        public Spanning<IKeyword> Tags => Spanning<IKeyword>.Empty;

        public IKeyword Category => null;

        public int MaxHp => info.MaxHp;
        public int MaxMp => info.MaxMp;
        public int Atk => info.Atk;
        public int Def => info.Def;
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
        public IApplyRogueMethod BeSteppedOnAsTile { get; }

        public Spanning<IRaceOption> GrowingOptions => Spanning<IRaceOption>.Empty;
        public Spanning<IRogueGender> Genders => RoguegardSettings.DefaultRaceOption.Genders;
        public Spanning<IMemberSource> RaceOptionMemberSources => Spanning<IMemberSource>.Empty;
        public Spanning<IMemberSource> StartingItemOptionMemberSources => Spanning<IMemberSource>.Empty;

        public KyarakuriClayAsset(KyarakuriClayInfo info, string envRgpackId, string fullId)
        {
            this.info = info;
            Reference = new KyarakuriClayReference(fullId, envRgpackId);
            if (info.RaceWeight != null) { raceWeightCmn = info.RaceWeight.ToReference(envRgpackId); }
            if (info.RaceSprite != null) { raceSpriteCmn = info.RaceSprite.ToReference(envRgpackId); }

            Walk = RogueMethod.Create(info.Walk, envRgpackId, RoguegardSettings.DefaultRaceOption.Walk);
            Wait = RogueMethod.Create(info.Wait, envRgpackId, RoguegardSettings.DefaultRaceOption.Wait);
            Attack = PropertiedCmnSkill.Create(info.Attack, envRgpackId, RoguegardSettings.DefaultRaceOption.Attack);
            Throw = PropertiedCmnSkill.Create(info.Throw, envRgpackId, RoguegardSettings.DefaultRaceOption.Throw);
            PickUp = RogueMethod.Create(info.PickUp, envRgpackId, RoguegardSettings.DefaultRaceOption.PickUp);
            Put = RogueMethod.Create(info.Put, envRgpackId, RoguegardSettings.DefaultRaceOption.Put);

            Hit = RogueMethod.Create(info.Hit, envRgpackId, RoguegardSettings.DefaultRaceOption.Hit);
            BeDefeated = RogueMethod.Create(info.BeDefeated, envRgpackId, RoguegardSettings.DefaultRaceOption.BeDefeated);
            Locate = RogueMethod.Create(info.Locate, envRgpackId, RoguegardSettings.DefaultRaceOption.Locate);
            Polymorph = RogueMethod.Create(info.Polymorph, envRgpackId, RoguegardSettings.DefaultRaceOption.Polymorph);

            BeApplied = RogueMethod.Create(info.BeApplied, envRgpackId, RoguegardSettings.DefaultRaceOption.BeApplied);
            BeThrown = RogueMethod.Create(info.BeThrown, envRgpackId, RoguegardSettings.DefaultRaceOption.BeThrown);
            BeEaten = RogueMethod.Create(info.BeEaten, envRgpackId, RoguegardSettings.DefaultRaceOption.BeEaten);
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
            out OchalikeBone mainBone, out AppearanceMorph morph)
        {
            if (raceSpriteCmn != null)
            {
                var tuple = ((object, object))raceSpriteCmn.Invoke();
                mainBone = (OchalikeBone)tuple.Item1;
                morph = (AppearanceMorph)tuple.Item2;
            }
            else
            {
                RoguegardSettings.DefaultRaceOption.GetSpriteValues(raceOption, characterCreationData, gender, out mainBone, out morph);
            }
        }

        public void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IReadOnlyOchalikeBone mainBone,
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
                RoguegardSettings.DefaultRaceOption.GetObjSprite(raceOption, characterCreationData, gender, self, mainBone, out objSprite, out motionSet);
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

        IReadOnlyMember IReadOnlyMemberable.GetMember(IMemberSource source)
        {
            foreach (var member in _members.Span)
            {
                if (member.Source == source) return member;
            }
            throw new System.ArgumentException($"{source} の {nameof(IMember)} が見つかりません。");
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
            public int RequiredMp => 0;
            public Spanning<IKeyword> AmmoCategories => Spanning<IKeyword>.Empty;

            private RogueMethod(PropertiedCmnData data, string envRgpackId)
            {
                reference = data.ToReference(envRgpackId);
            }

            public static IActiveRogueMethod Create(PropertiedCmnData data, string envRgpackId, IActiveRogueMethod defaultMethod)
            {
                return string.IsNullOrWhiteSpace(data.Cmn) ? defaultMethod : new RogueMethod(data, envRgpackId);
            }

            public static IApplyRogueMethod Create(PropertiedCmnData data, string envRgpackId, IApplyRogueMethod defaultMethod)
            {
                return string.IsNullOrWhiteSpace(data.Cmn) ? defaultMethod : new RogueMethod(data, envRgpackId);
            }

            public static IAffectRogueMethod Create(PropertiedCmnData data, string envRgpackId, IAffectRogueMethod defaultMethod)
            {
                return string.IsNullOrWhiteSpace(data.Cmn) ? defaultMethod : new RogueMethod(data, envRgpackId);
            }

            public static IChangeStateRogueMethod Create(PropertiedCmnData data, string envRgpackId, IChangeStateRogueMethod defaultMethod)
            {
                return string.IsNullOrWhiteSpace(data.Cmn) ? defaultMethod : new RogueMethod(data, envRgpackId);
            }

            public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                var result = reference.Invoke();
                return result == null || result is bool boolean && boolean == true;
            }

            public int GetAtk(RogueObj self, out bool additionalEffect)
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
