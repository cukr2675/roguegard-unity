using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// 転倒して2ターン行動不能。さらに防御力-2。
    /// </summary>
    [ObjectFormer.Formable]
    public class DownStatusEffect : TimeLimitedStackableStatusEffect, IValueEffect, IBoneMotionEffect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new DownStatusEffect());

        public override string Name => "転倒";
        public override IKeyword EffectCategory => EffectCategoryKw.StatusAilment;
        protected override int MaxStack => 1;
        protected override int InitialLifeTime => 3;

        float IValueEffect.Order => -1;
        float IBoneMotionEffect.Order => 0f;

        private DownStatusEffect() { }

        protected override void NewAffectTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            if (RogueDevice.Primary.VisibleAt(target.Location, target.Position))
            {
                RogueDevice.Add(DeviceKw.AppendText, target);
                RogueDevice.Add(DeviceKw.AppendText, "は転倒した！\n");
            }
        }

        public override void Open(RogueObj self)
        {
            SpeedCalculator.SetDirty(self);
            base.Open(self);
        }

        protected override bool RemoveClose(RogueObj self, StatusEffectCloseType closeType = StatusEffectCloseType.Manual)
        {
            SpeedCalculator.SetDirty(self);
            return base.RemoveClose(self);
        }

        void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
        {
            if (keyword == StatsKw.Speed)
            {
                // 行動不可
                value.SubValues[StatsKw.BeInhibited] = 1f;
            }
            else if (keyword == StatsKw.DEF)
            {
                // 防御力 -2 (IValueEffect.Order: -1 につき防御力 0 未満にはならない)
                value.MainValue = Mathf.Min(value.MainValue + 2f, value.BaseMainValue);
            }
        }

        void IBoneMotionEffect.ApplyTo(
            IMotionSet motionSet, IKeyword keyword, int animationTime, RogueDirection direction, ref RogueObjSpriteTransform transform)
        {
            if (keyword != MainInfoKw.Hit && keyword != MainInfoKw.BeDefeated)
            {
                motionSet.GetPose(MainInfoKw.Hit, animationTime, direction, ref transform, out _);
            }
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is DownStatusEffect effect && effect.Stack == Stack && effect.LifeTime == LifeTime;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new DownStatusEffect()
            {
                Stack = Stack,
                LifeTime = LifeTime
            };
        }
    }
}
