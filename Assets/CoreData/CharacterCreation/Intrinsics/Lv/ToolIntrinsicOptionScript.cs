using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public abstract class ToolIntrinsicOptionScript : ScriptIntrinsicOption.Script
    {
        public override Spanning<IMemberSource> MemberSources => _memberSources;
        private static readonly IMemberSource[] _memberSources = new IMemberSource[] { ItemMember.SourceInstance };

        public sealed override float GetCost(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, out bool costIsUnknown)
        {
            var toolMember = ItemMember.GetMember(intrinsic);
            var tool = toolMember.Item;
            return GetCost(intrinsic, characterCreationData, tool, out costIsUnknown);
        }
        protected abstract float GetCost(
            IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, IReadOnlyStartingItem tool, out bool costIsUnknown);

        public sealed override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            var toolMember = ItemMember.GetMember(intrinsic);
            var tool = toolMember.Item;
            return CreateSortedIntrinsic(intrinsic, characterCreationData, lv, tool);
        }
        protected abstract ISortedIntrinsic CreateSortedIntrinsic(
            IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv, IReadOnlyStartingItem tool);
    }
}
