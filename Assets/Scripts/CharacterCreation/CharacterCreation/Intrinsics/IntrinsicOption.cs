using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// <see cref="ScriptableObject"/> にせず <see cref="ScriptField{T}"/> で済ませることもできるが、
    /// <see cref="AppearanceOption"/> や <see cref="IStartingItemOption"/> と同じように
    /// <see cref="ScriptableObject"/> で扱えたほうが処理の共通化が見込める
    /// </summary>
    public abstract class IntrinsicOption : RogueDescriptionData, IIntrinsicOption
    {
        public virtual Spanning<IMemberSource> MemberSources => Spanning<IMemberSource>.Empty;

        public virtual void UpdateMemberRange(IMember member, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData)
        {
        }

        public abstract int GetLv(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData);

        public abstract float GetCost(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, out bool costIsUnknown);

        public abstract ISortedIntrinsic CreateSortedIntrinsic(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData);
    }
}
