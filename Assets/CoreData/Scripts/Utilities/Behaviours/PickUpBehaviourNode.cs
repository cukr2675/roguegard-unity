using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class PickUpBehaviourNode : IRogueBehaviourNode
    {
        public int DistanceThreshold { get; set; }
        public IPathBuilder PathBuilder { get; set; }

        private static readonly PickUpObjCommand pickUp = new PickUpObjCommand();

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (!ViewInfo.TryGet(self, out var view)) return RogueObjUpdaterContinueType.Continue;

            view.ReadyView(self.Location);
            view.AddView(self);

            var loadCapacity = StatsEffectedValues.GetLoadCapacity(self);
            var selfWeight = WeightCalculator.Get(self);

            RogueObj nearestItem = null;
            var nearestSqrDistance = DistanceThreshold * DistanceThreshold;
            for (int i = 0; i < view.VisibleObjCount; i++)
            {
                var obj = view.GetVisibleObj(i);
                if (obj == null) continue;
                if (obj.HasCollider || obj.AsTile) continue;

                // 最大重量を超過するアイテムは拾わない
                var weight = WeightCalculator.Get(obj);
                if (selfWeight.SpaceWeight + weight.TotalWeight > loadCapacity) continue;

                var sqrDistance = (obj.Position - self.Position).sqrMagnitude;
                if (sqrDistance < nearestSqrDistance)
                {
                    // 最短距離を更新したとき、敵を設定する
                    nearestSqrDistance = sqrDistance;
                    nearestItem = obj;
                }
            }

            if (nearestItem != null)
            {
                if (nearestItem.Position == self.Position)
                {
                    // アイテムの上についたら拾う
                    if (pickUp.CommandInvoke(self, null, activationDepth, new(tool: nearestItem)))
                    {
                        return RogueObjUpdaterContinueType.Break;
                    }
                }

                // アイテムを見つけたらそこまで移動
                if (!PathBuilder.UpdatePath(self, nearestItem.Position)) return RogueObjUpdaterContinueType.Continue;
                if (!PathBuilder.TryGetNextDirection(self, out var nextDirection)) return RogueObjUpdaterContinueType.Continue;

                default(IActiveRogueMethodCaller).Walk(self, nextDirection, activationDepth);
                return RogueObjUpdaterContinueType.Break;
            }
            return RogueObjUpdaterContinueType.Continue;
        }
    }
}
