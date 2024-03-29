using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;

namespace Roguegard
{
    /// <summary>
    /// アイコンやタイルなどのスプライト。
    /// </summary>
    public interface IRogueSprite
    {
        TileBase Tile { get; }
        Color EffectedColor { get; }
    }
}
