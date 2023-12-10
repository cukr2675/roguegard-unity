using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IMemberable
    {
        IMemberableOption MemberableOption { get; }

        IReadOnlyMember GetMember(IMemberSource source);
    }
}
