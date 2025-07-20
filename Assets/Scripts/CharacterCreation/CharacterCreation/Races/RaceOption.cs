using OchalikeSprites;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public abstract class RaceOption : ScriptableObject, IRaceOption
    {
        public virtual Spanning<IMemberSource> MemberSources => Spanning<IMemberSource>.Empty;

        public virtual Spanning<IRaceOption> GrowingOptions => Spanning<IRaceOption>.Empty;

        public abstract float Cost { get; }
        public abstract bool CostIsUnknown { get; }
        public abstract Spanning<IRogueGender> Genders { get; }
        public abstract IKeyword Category { get; }
        public abstract int MaxHp { get; }
        public abstract int MaxMp { get; }
        public abstract int Atk { get; }
        public abstract int Def { get; }
        public abstract float LoadCapacity { get; }
        public abstract ISerializableKeyword Faction { get; }
        public abstract Spanning<ISerializableKeyword> TargetFactions { get; }
        public abstract MainInfoSetAbility Ability { get; }
        public abstract IRogueMaterial Material { get; }
        public abstract Spanning<IWeightedRogueObjGeneratorList> LootTable { get; }
        public abstract IActiveRogueMethod Walk { get; }
        public abstract IActiveRogueMethod Wait { get; }
        public abstract ISkill Attack { get; }
        public abstract ISkill Throw { get; }
        public abstract IActiveRogueMethod PickUp { get; }
        public abstract IActiveRogueMethod Put { get; }
        public abstract IEatActiveRogueMethod Eat { get; }
        public abstract IAffectRogueMethod Hit { get; }
        public abstract IAffectRogueMethod BeDefeated { get; }
        public abstract IChangeStateRogueMethod Locate { get; }
        public abstract IChangeStateRogueMethod Polymorph { get; }
        public abstract IApplyRogueMethod BeApplied { get; }
        public abstract IApplyRogueMethod BeThrown { get; }
        public abstract IApplyRogueMethod BeEaten { get; }
        public abstract IApplyRogueMethod BeSteppedOnAsTile { get; }
        public abstract string Name { get; }
        public abstract Sprite Icon { get; }
        public abstract Color Color { get; }
        public abstract string Caption { get; }
        public abstract IRogueDetails Details { get; }

        public abstract IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData);
        public abstract void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData);
        public abstract IRaceOption Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData);

        public abstract IEquipmentInfo GetEquipmentInfo(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData);

        public abstract IEquipmentState GetEquipmentState(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData);

        public abstract float GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData);

        public abstract void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out OchalikeBone mainBone, out AppearanceMorph morph);

        public abstract void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IReadOnlyOchalikeBone mainBone,
            out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet);

        public abstract void UpdateMemberRange(IMember member, IRaceOption raceOption, ICharacterCreationData characterCreationData);

        public abstract void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData);
    }
}
