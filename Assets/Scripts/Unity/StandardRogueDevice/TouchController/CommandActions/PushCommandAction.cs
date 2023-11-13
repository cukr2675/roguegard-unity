using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PushCommandAction : IDeviceCommandAction
    {
        public static PushCommandAction Instance { get; } = new PushCommandAction();

        private static readonly PushRogueMethod pushRogueMethod = new PushRogueMethod();

        public bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var result = RogueMethodAspectState.Invoke(StdKw.Push, pushRogueMethod, self, user, activationDepth, arg);
            if (!result) return false;

            // 押すのに成功したとき押した方向に一歩移動する。
            var targetPosition = arg.TargetObj.Position;
            var walkArg = new RogueMethodArgument(targetPosition: targetPosition);
            result = WalkCommandAction.Instance.CommandInvoke(self, user, activationDepth, walkArg);
            return result;
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
