using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.RequireRelationalComponent]
    public interface IIntrinsicOption : IRogueDescription
    {
        Spanning<IMemberSource> MemberSources { get; }

        void UpdateMemberRange(IMember member, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData);

        int GetLv(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData);

        float GetCost(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, out bool costIsUnknown);

        ISortedIntrinsic CreateSortedIntrinsic(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData);
    }
}
