using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class AttackBehaviourNode : IRogueBehaviourNode
    {
        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (self.Location == null) return RogueObjUpdaterContinueType.Continue;
            if (!self.Location.Space.TryGetRoomView(self.Position, out var room, out _)) { room = new RectInt(); }

            // 敵がプレイヤーを壁越しに察知して近づいてしまわないように視界距離は固定
            var visibleRadius = RoguegardSettings.DefaultVisibleRadius;
            var random = RogueRandom.Primary;

            // スキル・アイテム使用
            if (AutoAction.TryOtherAction(self, activationDepth, visibleRadius, room, random))
            {
                self.Main.Stats.TargetObj = null;
                return RogueObjUpdaterContinueType.Break;
            }

            // 通常攻撃
            var attackSkill = AttackUtility.GetNormalAttackSkill(self);
            if (AutoAction.AutoSkill(MainInfoKw.Attack, attackSkill, self, self, activationDepth, null, visibleRadius, room, random))
            {
                self.Main.Stats.TargetObj = null;
                return RogueObjUpdaterContinueType.Break;
            }

            // 射撃
            var throwSkill = self.Main.InfoSet.Attack;
            if (AutoAction.AutoSkill(MainInfoKw.Throw, throwSkill, self, self, activationDepth, null, visibleRadius, room, random))
            {
                self.Main.Stats.TargetObj = null;
                return RogueObjUpdaterContinueType.Break;
            }

            return RogueObjUpdaterContinueType.Continue;
        }
    }
}
