using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Character Creation Data/Info Set Referenced Object")]
    [Objforming.Referable]
    public class InfoSetReferencedObjectCreationDataAsset : ObjectCreationDataAsset
    {
        public override Spanning<IMemberSource> StartingItemOptionMemberSources => _startingItemOptionMemberSources;
        private static readonly IMemberSource[] _startingItemOptionMemberSources = new IMemberSource[] { SingleItemMember.SourceInstance };

        public override RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var obj = base.CreateObj(startingItem, location, position, random, stackOption);
            var member = SingleItemMember.GetMember(startingItem);
            if (member.ItemOption != null) { InfoSetReferenceInfo.SetTo(obj, member.ItemOption.InfoSet); }

            return obj;
        }
    }
}
