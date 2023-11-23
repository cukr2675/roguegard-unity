using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// <see cref="ScriptableObject"/> �ɂ��� <see cref="ScriptField{T}"/> �ōς܂��邱�Ƃ��ł��邪�A
    /// <see cref="AppearanceOption"/> �� <see cref="IStartingItemOption"/> �Ɠ����悤��
    /// <see cref="ScriptableObject"/> �ň������ق��������̋��ʉ��������߂�
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
