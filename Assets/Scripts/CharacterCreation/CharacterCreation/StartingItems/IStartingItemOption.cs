using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.RequireRelationalComponent]
    public interface IStartingItemOption : IRogueDescription, IMemberableOption
    {
        MainInfoSet InfoSet { get; }
        int Lv { get; }
        Spanning<IWeightedRogueObjGeneratorList> StartingItemTable { get; }

        void UpdateMemberRange(IMember member, IReadOnlyStartingItem startingItem, ICharacterCreationData characterCreationData);

        float GetCost(IReadOnlyStartingItem startingItem, out bool costIsUnknown);

        RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random,
            StackOption stackOption = StackOption.Default);
    }
}
