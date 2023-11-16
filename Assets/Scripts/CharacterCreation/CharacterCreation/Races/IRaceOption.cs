using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IRaceOption : IRogueDescription, IMemberableOption
    {
        /// <summary>
        /// この <see cref="IRaceOption"/> の進化・退化先。 <see cref="CharacterCreationInfoSet"/> のキャッシュに必要。
        /// 進化なしの場合は null にする。
        /// </summary>
        Spanning<IRaceOption> GrowingOptions { get; }

        float Cost { get; }
        bool CostIsUnknown { get; }

        Spanning<IRogueGender> Genders { get; }

        IKeyword Category { get; }

        int MaxHP { get; }
        int MaxMP { get; }
        int ATK { get; }
        int DEF { get; }
        float LoadCapacity { get; }
        ISerializableKeyword Faction { get; }
        Spanning<ISerializableKeyword> TargetFactions { get; }

        MainInfoSetAbility Ability { get; }
        IRogueMaterial Material { get; }

        Spanning<IWeightedRogueObjGeneratorList> LootTable { get; }

        IActiveRogueMethod Walk { get; }
        IActiveRogueMethod Wait { get; }
        ISkill Attack { get; }
        ISkill Throw { get; }
        IActiveRogueMethod PickUp { get; }
        IActiveRogueMethod Put { get; }
        IEatActiveRogueMethod Eat { get; }

        IAffectRogueMethod Hit { get; }
        IAffectRogueMethod BeDefeated { get; }
        IChangeStateRogueMethod Locate { get; }
        IChangeStateRogueMethod Polymorph { get; }

        IApplyRogueMethod BeApplied { get; }
        IApplyRogueMethod BeThrown { get; }
        IApplyRogueMethod BeEaten { get; }

        /// <summary>
        /// <see cref="IReadOnlyRace.Gender"/> は外部で設定しているため、このメソッドで設定する必要はない。
        /// ObjectRaceOption と RaceOption に共通の引数を渡すため、 <see cref="IRaceOption"/> を引数にとる
        /// </summary>
        IRaceOption Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData);

        /// <summary>
        /// <see cref="IReadOnlyRace.Gender"/> は外部で設定しているため、このメソッドで設定する必要はない。
        /// </summary>
        void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData);

        IRaceOption Reopen(RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData);

        IEquipmentState GetEquipmentState(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData);

        IEquipmentInfo GetEquipmentInfo(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData);

        float GetWeight(IRaceOption raceOption, ICharacterCreationData characterCreationData);

        void GetSpriteValues(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender,
            out BoneNodeBuilder mainNode, out AppearanceBoneSpriteTable boneSpriteTable);

        void GetObjSprite(
            IRaceOption raceOption, ICharacterCreationData characterCreationData, IRogueGender gender, RogueObj self, IBoneNode boneNode,
            out RogueObjSprite objSprite, out IMotionSet motionSet);

        void UpdateMemberRange(IMember member, IRaceOption raceOption, ICharacterCreationData characterCreationData);

        void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData);
    }
}
