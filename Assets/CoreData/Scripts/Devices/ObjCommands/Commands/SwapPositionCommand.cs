using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class SwapPositionCommand : BaseObjCommand
    {
        public override string Name => "位置入替";

        public override bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            // 一時的に当たり判定を消す
            var target = arg.TargetObj;
            target.TryLocate(target.Position, target.AsTile, false, target.HasTileCollider, target.HasSightCollider);

            // ターゲットに重なるよう移動する
            var direction = RogueDirection.FromSignOrLowerLeft(target.Position - self.Position);
            this.Walk(self, direction, activationDepth);

            // ターゲットを入れ替わりの位置へ移動する
            return this.Walk(target, direction.Rotate(4), activationDepth);
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
