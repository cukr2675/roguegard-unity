namespace Roguegard.CharacterCreation
{
    public abstract class ToolIntrinsicScript : ScriptIntrinsicOptionAsset.IntrinsicScript
    {
        public override Spanning<IMemberSource> MemberSources => _memberSources;
        private static readonly IMemberSource[] _memberSources = new IMemberSource[] { ItemMember.SourceInstance };

        public sealed override float GetCost(
            ScriptIntrinsicOptionAsset parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, out bool costIsUnknown)
        {
            var toolMember = ItemMember.GetMember(intrinsic);
            var tool = toolMember.Item;
            return GetCost(intrinsic, characterCreationData, tool, out costIsUnknown);
        }
        protected abstract float GetCost(
            IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, IReadOnlyStartingItem tool, out bool costIsUnknown);

        public sealed override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOptionAsset parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            var toolMember = ItemMember.GetMember(intrinsic);
            var tool = toolMember.Item;
            return CreateSortedIntrinsic(intrinsic, characterCreationData, lv, tool);
        }
        protected abstract ISortedIntrinsic CreateSortedIntrinsic(
            IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv, IReadOnlyStartingItem tool);
    }
}
