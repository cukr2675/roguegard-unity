using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Data/Singleton/Tile Laying")]
    [Objforming.IgnoreRequireRelationalComponent]
    public class TileLayingCreationDataAsset : CharacterCreationDataAsset
    {
        public override Spanning<IMemberSource> StartingItemOptionMemberSources => _startingItemOptionMemberSources;
        private static readonly IMemberSource[] _startingItemOptionMemberSources = new IMemberSource[] { RogueTileMember.SourceInstance };

        protected override bool HasNotInfoSet => true;

        public override RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var member = RogueTileMember.GetMember(startingItem);
            if (member.Tile == null) throw new RogueException("タイルが設定されていません。");

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
