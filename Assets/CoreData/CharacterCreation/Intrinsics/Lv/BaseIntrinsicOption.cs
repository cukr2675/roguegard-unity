using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public abstract class BaseIntrinsicOption : ReferableScript, IIntrinsicOption
    {
        public abstract string Name { get; }
        public virtual Sprite Icon => null;
        public virtual Color Color => Color.white;
        public virtual string Caption => null;
        public virtual object Details => null;
        protected abstract float Cost { get; }
        protected virtual bool CostIsUnknown => false;

        protected abstract int Lv { get; }

        public Spanning<IMemberSource> MemberSources => Spanning<IMemberSource>.Empty;

        void IIntrinsicOption.UpdateMemberRange(IMember member, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData)
        {
        }

        int IIntrinsicOption.GetLv(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData)
        {
            return Lv;
        }

        float IIntrinsicOption.GetCost(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, out bool costIsUnknown)
        {
            var cost = Cost;
            costIsUnknown = CostIsUnknown;
            return cost;
        }

        ISortedIntrinsic IIntrinsicOption.CreateSortedIntrinsic(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData)
        {
            return CreateSortedIntrinsic(intrinsic, characterCreationData, Lv);
        }
        protected abstract ISortedIntrinsic CreateSortedIntrinsic(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv);
    }
}
