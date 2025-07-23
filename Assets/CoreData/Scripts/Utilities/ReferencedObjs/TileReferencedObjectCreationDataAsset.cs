using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Character Creation Data/Tile Referenced Object")]
    [Objforming.Referable]
    public class TileReferencedObjectCreationDataAsset : ObjectCreationDataAsset
    {
        public override Spanning<IMemberSource> StartingItemOptionMemberSources => _startingItemOptionMemberSources;
        private static readonly IMemberSource[] _startingItemOptionMemberSources = new IMemberSource[] { RogueTileMember.SourceInstance };

        public override RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var obj = base.CreateObj(startingItem, location, position, random, stackOption);
            var member = RogueTileMember.GetMember(startingItem);
            if (member.Tile != null) { TileReferenceInfo.SetTo(obj, member.Tile); }

            return obj;
        }
    }
}
