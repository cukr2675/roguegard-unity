using UnityEngine;

namespace Roguegard
{
    public class SearchEnemyBehaviourNode : IRogueBehaviourNode
    {
        public int DistanceThreshold { get; set; }

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (self.Location == null) return RogueObjUpdaterContinueType.Continue;
            if (!self.Location.Space.TryGetRoomView(self.Position, out var room, out _)) { room = new RectInt(); }

            // 敵がプレイヤーを壁越しに察知して近づいてしまわないように視界距離は固定
            var visibleRadius = RoguegardSettings.DefaultVisibleRadius;
            var sqrVisibleRadius = visibleRadius * visibleRadius;

            RogueObj nearestEnemy = null;
            var nearestSqrDistance = DistanceThreshold * DistanceThreshold;
            foreach (var obj in self.Location.Space.Objs)
            {
                if (obj == null || !StatsEffectedValues.AreVS(self, obj)) continue;

                // 視界外の敵を除外する
                var sqrDistance = (obj.Position - self.Position).sqrMagnitude;
                if (sqrDistance >= sqrVisibleRadius && !room.Contains(obj.Position)) continue;

                if (sqrDistance < nearestSqrDistance)
                {
                    // 最短距離を更新したとき、敵を設定する
                    nearestSqrDistance = sqrDistance;
                    nearestEnemy = obj;
                }
            }

            // 最も近い敵を追跡対象に設定する
            self.Main.Stats.TargetObj = nearestEnemy;
            return RogueObjUpdaterContinueType.Continue;
        }
    }
}
