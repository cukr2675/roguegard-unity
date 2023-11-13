using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// 方向指定 + 単体効果の <see cref="IRogueMethodRange"/> 。主に飛び道具で使用される。
    /// </summary>
    public interface IProjectileRogueMethodRange : IRogueMethodRange
    {
        bool Raycast(
            RogueObj space, Vector2Int origin, RogueDirection direction, bool collide, bool tileCollide,
            out RogueObj hitObj, out Vector2Int hitPosition, out Vector2Int dropPosition);
    }
}
