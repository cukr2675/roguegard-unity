using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Data/Singleton/TileLaying")]
    [Objforming.IgnoreRequireRelationalComponent]
    public class TileLayingCreationData : ScriptableCharacterCreationData
    {
        public override Spanning<IMemberSource> StartingItemOptionMemberSources => _startingItemOptionMemberSources;
        private static readonly IMemberSource[] _startingItemOptionMemberSources = new IMemberSource[] { RogueTileMember.SourceInstance };

        protected override bool HasNotInfoSet => true;

        public override RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var member = RogueTileMember.GetMember(startingItem);
            if (member.Tile == null) throw new RogueException("É^ÉCÉãÇ™ê›íËÇ≥ÇÍÇƒÇ¢Ç‹ÇπÇÒÅB");

            location.Space.TrySet(member.Tile, position);
            return null;
        }

        protected override void GetCost(out float cost, out bool costIsUnknown)
        {
            cost = 0f;
            costIsUnknown = true;
        }
    }
}
