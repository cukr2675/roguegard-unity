using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IMemberable
    {
        IReadOnlyMember GetMember(IMemberSource source);
    }
}
