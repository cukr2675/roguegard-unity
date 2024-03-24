using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface IRogueTileInfo : IRogueTile, IRogueDescription
    {
        Sprite Sprite { get; }
        IKeyword Category { get; }

        RogueTileLayer Layer { get; }
        bool HasCollider { get; }
        bool HasSightCollider { get; }

        IAffectRogueMethod Hit { get; }
        IAffectRogueMethod BeDefeated { get; }
        IApplyRogueMethod BeApplied { get; }
    }
}
