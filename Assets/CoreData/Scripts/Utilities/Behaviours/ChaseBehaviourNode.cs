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

            // �߂Â��o�H�������ł��Ȃ��ꍇ�͉������Ȃ�
            if (!TryUpdatePath(self)) return RogueObjUpdaterContinueType.Continue;

            var result = false;
            if (PathBuilder.TryGetNextPosition(self, out var nextDirection))
            {
                result = default(IActiveRogueMethodCaller).Walk(self, nextDirection, activationDepth);
            }

            if (result)
            {
                // �ړ���̈ʒu�Ńp�X���X�V����
                TryUpdatePath(self);
                return RogueObjUpdaterContinueType.Break;
            }
            else
            {
                // �ǐՂł��Ȃ������Ƃ��^�[�Q�b�g���O��
                self.Main.Stats.TargetObj = null;
                return RogueObjUpdaterContinueType.Continue;
            }
        }

        private bool TryUpdatePath(RogueObj self)
        {
            if (!self.Location.Space.TryGetRoomView(self.Position, out var room, out _)) { room = new RectInt(); }

            // �G���v���C���[��ǉz���Ɏ@�m���ċ߂Â��Ă��܂�Ȃ��悤�Ɏ��E�����͌Œ�
            var visibleRadius = RoguegardSettings.DefaultVisibleRadius;
            var sqrVisibleRadius = visibleRadius * visibleRadius;

            var targetObj = self.Main.Stats.TargetObj;

            var sqrDistance = (targetObj.Position - self.Position).sqrMagnitude;
            if (sqrDistance < sqrVisibleRadius && room.Contains(targetObj.Position))
            {
                // �ǐՑΏۂ����E���ɂ���Ƃ����^�[���p�X�𐶐�����
                var updateResult = PathBuilder.UpdatePath(self, targetObj.Position);
                if (!updateResult) return false;
            }
            return true;
        }
    }
}
