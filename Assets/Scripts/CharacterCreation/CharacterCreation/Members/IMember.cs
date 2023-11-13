using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IMember : IReadOnlyMember
    {
        IMember Clone();
    }
}
