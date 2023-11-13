using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// <see cref="RogueObj"/> の当たり判定などの初期値を指定する列挙型。
    /// bool を複数持つ構造体でも良いが、エディタでの見やすさとエディタ拡張スクリプト数の増大を抑えられる点から列挙型にする。
    /// </summary>
    [System.Flags]
    public enum MainInfoSetAbility //: byte
    {
        AsTile = 1,
        HasCollider = 2,
        HasTileCollider = 4,
        HasSightCollider = 8,
        Movable = 16,

        /// <summary>
        /// エディタのセパレーター
        /// </summary>
        _ = 0,

        Object
            = HasTileCollider,

        WallObject
            = HasCollider | HasTileCollider,

        Character
            = HasCollider | HasTileCollider | Movable,

        Ghost
            = HasCollider | Movable,

        FloorTile
            = AsTile,

        TrapTile
            = AsTile | HasTileCollider,

        WallTile
            = AsTile | HasCollider | HasSightCollider,

        ClearWallTile
            = AsTile | HasCollider
    }
}
