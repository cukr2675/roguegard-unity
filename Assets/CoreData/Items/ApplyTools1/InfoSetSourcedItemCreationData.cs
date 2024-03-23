using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Data/InfoSetSourcedItem")]
    [ObjectFormer.Referable]
    public class InfoSetSourcedItemCreationData : ItemCreationData
    {
        public override Spanning<IMemberSource> StartingItemOptionMemberSources => _startingItemOptionMemberSources;
        private static readonly IMemberSource[] _startingItemOptionMemberSources = new IMemberSource[] { SingleItemMember.SourceInstance };

        public override RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var obj = base.CreateObj(startingItem, location, position, random, stackOption);
            var member = SingleItemMember.GetMember(startingItem);
            if (member.ItemOption != null) { InfoSetSourcedEffect.SetSource(obj, member.ItemOption.InfoSet); }

            return obj;
        }
    }
}
