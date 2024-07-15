using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// 重複して付与できる、持続時間付きのステータスエフェクト。
    /// </summary>
    internal abstract class TimeLimitedStatusEffect : BaseStatusEffect, IRogueObjUpdater
    {
        protected int LifeTime { get; set; }

        protected abstract int InitialLifeTime { get; }

        float IRogueObjUpdater.Order => UpdaterOrder;
        protected virtual float UpdaterOrder => 100f;

        protected override IRogueEffect AffectTo(RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var statusEffect = (TimeLimitedStatusEffect)base.AffectTo(target, user, activationDepth, arg);

            using var lifeTimeValue = EffectableValue.Get();
            lifeTimeValue.Initialize(InitialLifeTime);
            //if (arg.Tool != null) { ValueEffectState.AffectValue(StdKw.ItemPower, lifeTimeValue, arg.Tool); }
            statusEffect.LifeTime = Mathf.FloorToInt(lifeTimeValue.MainValue);
            return statusEffect;
        }

        RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
        {
            var result = UpdateObj(self, activationDepth, ref sectionIndex);
            if (result == RogueObjUpdaterContinueType.Break)
            {
                LifeTime--;
                if (LifeTime <= 0)
                {
                    RemoveClose(self, StatusEffectCloseType.TimeLimit);
                }
            }
            return result;
        }

        protected virtual RogueObjUpdaterContinueType UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
        {
            return default;
        }
    }
}
