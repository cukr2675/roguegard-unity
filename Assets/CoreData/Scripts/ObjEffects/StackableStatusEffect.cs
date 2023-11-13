using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// 重複して付与するとスタックするステータスエフェクト。
    /// </summary>
    public abstract class StackableStatusEffect : BaseStatusEffect
    {
        protected int Stack { get; set; }

        protected abstract int MaxStack { get; }

        protected override IRogueEffect AffectTo(RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var statusEffectState = target.Main.GetStatusEffectState(target);
            if (!statusEffectState.TryGetStatusEffect<StackableStatusEffect>(GetType(), out var statusEffect))
            {
                statusEffect = (StackableStatusEffect)base.AffectTo(target, user, activationDepth, arg);
                statusEffect.Stack = Mathf.Min(statusEffect.Stack + 1, MaxStack);
                NewAffectTo(target, user, activationDepth, arg, statusEffect);
            }
            else
            {
                statusEffect.Stack = Mathf.Min(statusEffect.Stack + 1, MaxStack);
                PreAffectedTo(target, user, activationDepth, arg, statusEffect);
            }
            return statusEffect;
        }

        /// <summary>
        /// このステータスエフェクトを新しく付与したとき呼び出されるメソッド。
        /// </summary>
        protected virtual void NewAffectTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
        }

        /// <summary>
        /// このステータスエフェクトをスタックしたとき呼び出されるメソッド。
        /// </summary>
        protected virtual void PreAffectedTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
        }
    }
}
