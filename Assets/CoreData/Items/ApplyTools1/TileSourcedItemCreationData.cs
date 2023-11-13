using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Data/TileSourcedItem")]
    [ObjectFormer.Referable]
    public class TileSourcedItemCreationData : ItemCreationData
    {
        public override Spanning<IMemberSource> StartingItemOptionMemberSources => _startingItemOptionMemberSources;
        private static readonly IMemberSource[] _startingItemOptionMemberSources = new IMemberSource[] { RogueTileMember.SourceInstance };

        public override RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var obj = base.CreateObj(startingItem, location, position, random, stackOption);
            var member = RogueTileMember.GetMember(startingItem);
            if (member.Tile != null) { TileSourcedEffect.SetSource(obj, member.Tile); }

            return obj;
        }
    }
}
