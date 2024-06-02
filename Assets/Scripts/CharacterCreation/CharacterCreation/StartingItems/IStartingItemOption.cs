using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.RequireRelationalComponent]
    public interface IStartingItemOption : IRogueDescription
    {
        MainInfoSet InfoSet { get; }
        int Lv { get; }
        Spanning<IWeightedRogueObjGeneratorList> StartingItemTable { get; }

        Spanning<IMemberSource> MemberSources { get; }

        void UpdateMemberRange(IMember member, IReadOnlyStartingItem startingItem, ICharacterCreationData characterCreationData);

        float GetCost(IReadOnlyStartingItem startingItem, out bool costIsUnknown);

        RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random,
            StackOption stackOption = StackOption.Default);
    }
}
