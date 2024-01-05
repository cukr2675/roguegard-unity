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

            // �G���v���C���[��ǉz���Ɏ@�m���ċ߂Â��Ă��܂�Ȃ��悤�Ɏ��E�����͌Œ�
            var visibleRadius = RoguegardSettings.DefaultVisibleRadius;
            var random = RogueRandom.Primary;

            // �X�L���E�A�C�e���g�p
            if (AutoAction.TryOtherAction(self, activationDepth, visibleRadius, room, random))
            {
                self.Main.Stats.TargetObj = null;
                return RogueObjUpdaterContinueType.Break;
            }

            // �ʏ�U��
            var attackSkill = AttackUtility.GetNormalAttackSkill(self);
            if (AutoAction.AutoSkill(MainInfoKw.Attack, attackSkill, self, self, activationDepth, null, visibleRadius, room, random))
            {
                self.Main.Stats.TargetObj = null;
                return RogueObjUpdaterContinueType.Break;
            }

            // �ˌ�
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
