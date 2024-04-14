using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    [Objforming.Formable]
    public class SleepStatusEffect : TimeLimitedStackableStatusEffect, IValueEffect, IRogueMethodActiveAspect, IRogueMethodPassiveAspect, ISpriteMotionEffect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new SleepStatusEffect());

        public override string Name => "睡眠";
        public override IKeyword EffectCategory => EffectCategoryKw.StatusAilment;
        protected override int MaxStack => 1;
        protected override int InitialLifeTime => 100;

        float IValueEffect.Order => 1f;
        float IRogueMethodActiveAspect.Order => -100f;
        float IRogueMethodPassiveAspect.Order => 0f;
        float ISpriteMotionEffect.Order => 0f;

        private static ISpriteMotion _smoke;

        private SleepStatusEffect() { }

        protected override void NewAffectTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            if (MessageWorkListener.TryOpenHandler(target.Location, target.Position, out var h))
            {
                _smoke ??= new VariantSpriteMotion(CoreMotions.Smoke, new Color32(250, 100, 200, 255));
                using var handler = h;
                handler.EnqueueSE(StdKw.StatusEffect);
                handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(target, CoreMotions.Sleep, true));
                handler.EnqueueWork(RogueCharacterWork.CreateEffect(target.Position, _smoke, false));
                handler.AppendText(target).AppendText("は眠ってしまった！\n");
            }
            MainCharacterWorkUtility.EnqueueViewDequeueState(target);
        }

        protected override void PreAffectedTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            NewAffectTo(target, user, activationDepth, arg, statusEffect);
        }

        public override void Open(RogueObj self)
        {
            SpeedCalculator.SetDirty(self);
            base.Open(self);
        }

        protected override void RemoveClose(RogueObj self, StatusEffectCloseType closeType = StatusEffectCloseType.Manual)
        {
            SpeedCalculator.SetDirty(self);
            if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                using var handler = h;
                handler.AppendText(self).AppendText("は目を覚ました！\n");
            }
            base.RemoveClose(self);
        }

        void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
        {
            if (keyword == StatsKw.Speed)
            {
                value.SubValues[StatsKw.BeInhibited] = 1f;
            }
            else if (keyword == StdKw.View)
            {
                value.MainValue = 0f;
            }
        }

        void ISpriteMotionEffect.ApplyTo(
            ISpriteMotionSet motionSet, IKeyword keyword, int animationTime, RogueDirection direction, ref SkeletalSpriteTransform transform)
        {
            if (keyword != MainInfoKw.Hit)
            {
                CoreMotions.Sleep.ApplyTo(animationTime, direction, ref transform, out _);
            }
        }

        bool IRogueMethodActiveAspect.ActiveInvoke(
            IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj target, float activationDepth, in RogueMethodArgument arg,
            RogueMethodAspectState.ActiveNext next)
        {
            if (keyword == MainInfoKw.BeEaten)
            {
                // 食べる動作だけは可能
                return next.Invoke(keyword, method, self, target, activationDepth, arg);
            }

            if (RogueDevice.Primary.Player == self)
            {
                RogueDevice.Add(DeviceKw.AppendText, self);
                RogueDevice.Add(DeviceKw.AppendText, "は眠っている\n");
            }
            return false;
        }

        bool IRogueMethodPassiveAspect.PassiveInvoke(
            IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
            RogueMethodAspectState.PassiveNext next)
        {
            var result = next.Invoke(keyword, method, self, user, activationDepth, arg);
            if (result && keyword == MainInfoKw.Hit && arg.RefValue?.MainValue >= 1)
            {
                // ダメージを受けたとき目を覚ます
                RemoveClose(self);
            }
            return result;
        }

        protected override RogueObjUpdaterContinueType UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
        {
            // 毎ターン満腹度を 1 回復する
            self.Main.Stats.SetNutrition(self, self.Main.Stats.Nutrition + 1);

            return default;
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is SleepStatusEffect effect && effect.Stack == Stack && effect.LifeTime == LifeTime;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new SleepStatusEffect()
            {
                Stack = Stack,
                LifeTime = LifeTime
            };
        }
    }
}
