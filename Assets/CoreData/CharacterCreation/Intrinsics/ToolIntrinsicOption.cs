using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public abstract class ToolIntrinsicOption : ReferableScript, IIntrinsicOption
    {
        public abstract string Name { get; }
        public virtual Sprite Icon => null;
        public virtual Color Color => Color.white;
        public virtual string Caption => null;
        public virtual object Details => null;

        protected abstract int Lv { get; }

        public Spanning<IMemberSource> MemberSources => _memberSources;
        private static readonly IMemberSource[] _memberSources = new IMemberSource[] { ItemMember.SourceInstance };

        void IIntrinsicOption.UpdateMemberRange(IMember member, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData)
        {
        }

        int IIntrinsicOption.GetLv(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData)
        {
            return Lv;
        }

        float IIntrinsicOption.GetCost(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, out bool costIsUnknown)
        {
            var toolMember = ItemMember.GetMember(intrinsic);
            var tool = toolMember.Item;
            return GetCost(intrinsic, characterCreationData, tool, out costIsUnknown);
        }
        public abstract float GetCost(
            IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, IReadOnlyStartingItem tool, out bool costIsUnknown);

        ISortedIntrinsic IIntrinsicOption.CreateSortedIntrinsic(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData)
        {
            var toolMember = ItemMember.GetMember(intrinsic);
            var tool = toolMember.Item;
            return CreateSortedIntrinsic(intrinsic, characterCreationData, Lv, tool);
        }
        protected abstract ISortedIntrinsic CreateSortedIntrinsic(
            IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv, IReadOnlyStartingItem tool);
    }
}
