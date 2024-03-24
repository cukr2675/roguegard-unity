using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    /// <summary>
    /// �p�[�e�B���[�_�[�ɒǏ]���� <see cref="IRogueBehaviourNode"/>
    /// </summary>
    public class FollowLeaderBehaviourNode : IRogueBehaviourNode
    {
        public IPathBuilder PathBuilder { get; set; }

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            var party = self.Main.Stats.Party;
            var leader = party.Members[0];
            if (party == null || leader == self || self.Location == null || self.Location != leader.Location) return RogueObjUpdaterContinueType.Continue;

            // ���[�_�[�Ɠ�����Ԃɂ���΃��[�_�[�̕���������
            self.Main.Stats.Direction = RogueDirection.FromSignOrLowerLeft(leader.Position - self.Position);

            // ���[�_�[�Ɨׂ荇���Ă�����߂Â�����
            // �s�p�ӂȈړ��������
            var sqrDistance = (leader.Position - self.Position).sqrMagnitude;
            if (sqrDistance <= 1) return RogueObjUpdaterContinueType.Continue;
            if (sqrDistance <= 2)
            {
                if (!MovementUtility.TryGetApproachDirection(self, leader.Position, true, out var direction)) return RogueObjUpdaterContinueType.Continue;

                // �΂߈ړ��ł���ꍇ�͋߂Â��Ȃ�
                var nearAngle = (int)RogueDirection.FromSignOrLowerLeft(leader.Position - self.Position);
                var angle = (int)direction;
                var near = new RogueDirection(nearAngle - (angle - nearAngle)).Forward;
                if (!self.Location.Space.CollideAt(self.Position + near)) return RogueObjUpdaterContinueType.Continue;

                if (!default(IActiveRogueMethodCaller).Walk(self, direction, activationDepth)) return RogueObjUpdaterContinueType.Continue;

                return RogueObjUpdaterContinueType.Break;
            }

            // ���[�_�[�̈ʒu�܂ňړ�
            if (!PathBuilder.UpdatePath(self, leader.Position)) return RogueObjUpdaterContinueType.Continue;
            if (!PathBuilder.TryGetNextDirection(self, out var nextDirection)) return RogueObjUpdaterContinueType.Continue;
            if (!default(IActiveRogueMethodCaller).Walk(self, nextDirection, activationDepth)) return RogueObjUpdaterContinueType.Continue;

            return RogueObjUpdaterContinueType.Break;
        }
    }
}
