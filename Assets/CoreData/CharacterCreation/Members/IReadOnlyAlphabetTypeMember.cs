using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyAlphabetTypeMember : IReadOnlyMember
    {
        int TypeIndex { get; }
        string Type { get; }
    }
}
