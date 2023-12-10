using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Data/Singleton/TileLaying")]
    [ObjectFormer.IgnoreRequireRelationalComponent]
    public class TileLayingCreationData : ItemCreationData
    {
        public override Spanning<IMemberSource> StartingItemOptionMemberSources => _startingItemOptionMemberSources;
        private static readonly IMemberSource[] _startingItemOptionMemberSources = new IMemberSource[] { RogueTileMember.SourceInstance };

        public override RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var member = RogueTileMember.GetMember(startingItem);
            if (member.Tile == null) throw new RogueException("�^�C�����ݒ肳��Ă��܂���B");

            location.Space.TrySet(member.Tile, position);
            return null;
        }
    }
}
