using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    /// <summary>
    /// 自分に対して敵対している存在と隣接する位置に移動しようとしたら停止する。
    /// </summary>
    public class AdjacentEnemyWalkStopper : IPositionedWalkStopper
    {
        public bool GetStop(RogueObj self, Vector2Int targetPosition)
        {
            var view = self.Get<ViewInfo>();
            for (int j = 0; j < view.VisibleObjCount; j++)
            {
                var viewObj = view.GetVisibleObj(j);
                if (!StatsEffectedValues.AreVS(self, viewObj) || viewObj == self) continue;

                var distance = viewObj.Position - targetPosition;
                if (viewObj != self && Mathf.Max(Mathf.Abs(distance.x), Mathf.Abs(distance.y)) <= 1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
