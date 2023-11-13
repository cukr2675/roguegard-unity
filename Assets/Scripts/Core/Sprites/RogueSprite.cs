using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;

namespace Roguegard
{
    /// <summary>
    /// アイコンやタイルなどのスプライト。
    /// </summary>
    public abstract class RogueSprite : TileBase
    {
        public abstract Sprite IconSprite { get; }
        public abstract Color IconColor { get; }
    }
}
