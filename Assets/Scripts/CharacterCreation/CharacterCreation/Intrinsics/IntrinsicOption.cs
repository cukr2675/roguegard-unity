using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// <see cref="ScriptableObject"/> ‚É‚¹‚¸ <see cref="ScriptField{T}"/> ‚ÅÏ‚Ü‚¹‚é‚±‚Æ‚à‚Å‚«‚é‚ªA
    /// <see cref="AppearanceOption"/> ‚â <see cref="IStartingItemOption"/> ‚Æ“¯‚¶‚æ‚¤‚É
    /// <see cref="ScriptableObject"/> ‚Åˆµ‚¦‚½‚Ù‚¤‚ªˆ—‚Ì‹¤’Ê‰»‚ªŒ©‚ß‚é
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
