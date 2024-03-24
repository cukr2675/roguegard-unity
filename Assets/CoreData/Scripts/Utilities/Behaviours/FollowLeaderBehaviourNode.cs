using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    /// <summary>
    /// パーティリーダーに追従する <see cref="IRogueBehaviourNode"/>
    /// </summary>
    public class FollowLeaderBehaviourNode : IRogueBehaviourNode
    {
        public IPathBuilder PathBuilder { get; set; }

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            var party = self.Main.Stats.Party;
            var leader = party.Members[0];
            if (party == null || leader == self || self.Location == null || self.Location != leader.Location) return RogueObjUpdaterContinueType.Continue;

            // リーダーと同じ空間にいればリーダーの方向を向く
            self.Main.Stats.Direction = RogueDirection.FromSignOrLowerLeft(leader.Position - self.Position);

            // リーダーと隣り合っていたら近づくだけ
            // 不用意な移動を避ける
            var sqrDistance = (leader.Position - self.Position).sqrMagnitude;
            if (sqrDistance <= 1) return RogueObjUpdaterContinueType.Continue;
            if (sqrDistance <= 2)
            {
                if (!MovementUtility.TryGetApproachDirection(self, leader.Position, true, out var direction)) return RogueObjUpdaterContinueType.Continue;

                // 斜め移動できる場合は近づかない
                var nearAngle = (int)RogueDirection.FromSignOrLowerLeft(leader.Position - self.Position);
                var angle = (int)direction;
                var near = new RogueDirection(nearAngle - (angle - nearAngle)).Forward;
                if (!self.Location.Space.CollideAt(self.Position + near)) return RogueObjUpdaterContinueType.Continue;

                if (!default(IActiveRogueMethodCaller).Walk(self, direction, activationDepth)) return RogueObjUpdaterContinueType.Continue;

                return RogueObjUpdaterContinueType.Break;
            }

            // リーダーの位置まで移動
            if (!PathBuilder.UpdatePath(self, leader.Position)) return RogueObjUpdaterContinueType.Continue;
            if (!PathBuilder.TryGetNextDirection(self, out var nextDirection)) return RogueObjUpdaterContinueType.Continue;
            if (!default(IActiveRogueMethodCaller).Walk(self, nextDirection, activationDepth)) return RogueObjUpdaterContinueType.Continue;

            return RogueObjUpdaterContinueType.Break;
        }
    }
}
