using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class FearMovementBehaviourNode : IRogueBehaviourNode
    {
        private WanderingWalker walker = new WanderingWalker(RoguegardSettings.MaxTilemapSize);

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            var movement = MovementCalculator.Get(self);
            if (!movement.SubIs(StdKw.Fear)) return RogueObjUpdaterContinueType.Continue;

            // �G���v���C���[��ǉz���Ɏ@�m���ċ߂Â��Ă��܂�Ȃ��悤�Ɏ��E�����͌Œ�
            var visibleRadius = RoguegardSettings.DefaultVisibleRadius;
            if (!self.Location.Space.TryGetRoomView(self.Position, out var room, out _)) { room = new RectInt(); }

            var sqrVisibleRadius = visibleRadius * visibleRadius;
            var objs = self.Location.Space.Objs;
            for (int i = 0; i < objs.Count; i++)
            {
                var obj = objs[i];
                if (obj == null || StatsEffectedValues.AreVS(self, obj)) continue;

                var distance = obj.Position - self.Position;
                if (distance.sqrMagnitude >= sqrVisibleRadius || !room.Contains(obj.Position)) continue;

                // ������
                var targetPosition = walker.GetWalk(self, true);
                if (RogueDirection.TryFromSign(targetPosition - self.Position, out var direction))
                {
                    default(IActiveRogueMethodCaller).Walk(self, direction, activationDepth);
                    walker.GetWalk(self, true); // �ړ���������̎��E�Ńp�X���X�V
                }
                return RogueObjUpdaterContinueType.Break;
            }
            return RogueObjUpdaterContinueType.Continue;
        }
    }
}
