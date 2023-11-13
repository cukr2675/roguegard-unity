using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyRogueTileMember : IReadOnlyMember
    {
        IRogueTile Tile { get; }
    }
}
