using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class PushCommand : BaseObjCommand
    {
        public override string Name => "押す";

        private static readonly PushRogueMethod pushRogueMethod = new PushRogueMethod();

        public override bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var direction = RogueDirection.FromSignOrLowerLeft(arg.TargetObj.Position - self.Position);
            var result = RogueMethodAspectState.Invoke(StdKw.Push, pushRogueMethod, self, user, activationDepth, arg);
            if (!result) return false;

            // 押すのに成功したとき押した方向に一歩移動する。
            this.Walk(self, direction, activationDepth);

            // 移動できなくても押せたらターン経過させる
            return true;
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            return null;
        }

        private class PushRogueMethod : IActiveRogueMethod
        {
            public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (CommonAssert.ObjDoesNotHaveToolAbility(self)) return false;

                var target = arg.TargetObj;
                var method = target.Main.InfoSet.BeApplied;
                var relativePosition = target.Position - self.Position;
                var pushArg = new RogueMethodArgument(targetPosition: target.Position + relativePosition);
                var result = RogueMethodAspectState.Invoke(StdKw.Push, method, target, self, activationDepth, pushArg);
                return result;
            }
        }
    }
}
