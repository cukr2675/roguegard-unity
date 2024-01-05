using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class ChaseBehaviourNode : IRogueBehaviourNode
    {
        public int DistanceThreshold { get; set; }
        public IPathBuilder PathBuilder { get; set; }

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (self.Location == null || self.Main.Stats.TargetObj == null) return RogueObjUpdaterContinueType.Continue;

            // 近づく経路が生成できない場合は何もしない
            if (!TryUpdatePath(self)) return RogueObjUpdaterContinueType.Continue;

            var result = false;
            if (PathBuilder.TryGetNextPosition(self, out var nextDirection))
            {
                result = default(IActiveRogueMethodCaller).Walk(self, nextDirection, activationDepth);
            }

            if (result)
            {
                // 移動後の位置でパスを更新する
                TryUpdatePath(self);
                return RogueObjUpdaterContinueType.Break;
            }
            else
            {
                // 追跡できなかったときターゲットを外す
                self.Main.Stats.TargetObj = null;
                return RogueObjUpdaterContinueType.Continue;
            }
        }

        private bool TryUpdatePath(RogueObj self)
        {
            if (!self.Location.Space.TryGetRoomView(self.Position, out var room, out _)) { room = new RectInt(); }

            // 敵がプレイヤーを壁越しに察知して近づいてしまわないように視界距離は固定
            var visibleRadius = RoguegardSettings.DefaultVisibleRadius;
            var sqrVisibleRadius = visibleRadius * visibleRadius;

            var targetObj = self.Main.Stats.TargetObj;

            var sqrDistance = (targetObj.Position - self.Position).sqrMagnitude;
            if (sqrDistance < sqrVisibleRadius && room.Contains(targetObj.Position))
            {
                // 追跡対象が視界内にいるとき毎ターンパスを生成する
                var updateResult = PathBuilder.UpdatePath(self, targetObj.Position);
                if (!updateResult) return false;
            }
            return true;
        }
    }
}
