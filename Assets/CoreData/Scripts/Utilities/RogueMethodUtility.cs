using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class RogueMethodUtility
    {
        public static RogueDirection GetTargetDirection(RogueObj self, in RogueMethodArgument arg)
        {
            if (arg.TryGetTargetPosition(out var targetPosition) && RogueDirection.TryFromSign(targetPosition - self.Position, out var direction))
            {
                return direction;
            }
            else
            {
                return self.Main.Stats.Direction;
            }
        }

        /// <summary>
        /// 二つの <see cref="RogueObj"/> が隣接しているかを取得する。
        /// </summary>
        public static bool GetAdjacent(RogueObj left, RogueObj right)
        {
            return (left.Position - right.Position).sqrMagnitude <= 2;
        }
    }
}
