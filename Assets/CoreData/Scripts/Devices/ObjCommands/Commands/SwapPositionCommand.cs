using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    /// <summary>
    /// ���̃L�����N�^�[�Əꏊ�����ւ���B���l�Ɠ���ւ�邱�Ƃ�㩂𓥂܂��Ă��܂�Ȃ��悤�� <see cref="IMainInfoSet.Walk"/> �Ƃ͋�ʂ���B
    /// </summary>
    public class SwapPositionCommand : BaseObjCommand
    {
        public override string Name => "�ʒu����";

        public override bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            // �ꎞ�I�ɓ����蔻�������
            var target = arg.TargetObj;
            target.TryLocate(target.Position, target.AsTile, false, target.HasTileCollider, target.HasSightCollider);

            // �^�[�Q�b�g�ɏd�Ȃ�悤�ړ�����
            var direction = RogueDirection.FromSignOrLowerLeft(target.Position - self.Position);
            this.Walk(self, direction, activationDepth);

            // �^�[�Q�b�g�����ւ��̈ʒu�ֈړ�����
            return this.Walk(target, direction.Rotate(4), activationDepth);
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
