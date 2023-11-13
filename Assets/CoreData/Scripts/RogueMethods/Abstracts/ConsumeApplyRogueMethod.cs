using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class ConsumeApplyRogueMethod : BaseApplyRogueMethod
    {
        public sealed override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (self.Stack == 0)
            {
                Debug.LogError("存在しないオブジェクトを消費しようとしました。");
                return false;
            }
            if (CommonAssert.ObjDoesNotHaveToolAbility(user)) return false;

            // 効果を発動しないとき消費しない。
            var result = BeApplied(self, user, activationDepth, arg);
            if (result)
            {
                self.TrySetStack(self.Stack - 1, user); // オブジェクトを消費する。
                return true;
            }

            return false;
        }

        protected abstract bool BeApplied(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);
    }
}
