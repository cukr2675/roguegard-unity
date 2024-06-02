using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IMemberable
    {
        Spanning<IMemberSource> MemberSources { get; }

        IReadOnlyMember GetMember(IMemberSource source);
    }
}
