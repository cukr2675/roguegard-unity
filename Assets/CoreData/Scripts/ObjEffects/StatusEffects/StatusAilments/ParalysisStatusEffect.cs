using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class ParalysisStatusEffect : TimeLimitedStackableStatusEffect, IValueEffect, IRogueMethodActiveAspect, IBoneMotionEffect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new ParalysisStatusEffect());

        public override string Name => "麻痺";
        public override IKeyword EffectCategory => EffectCategoryKw.StatusAilment;
        protected override int MaxStack => 1;
        protected override int InitialLifeTime => 5;

        float IValueEffect.Order => 0f;
        float IRogueMethodActiveAspect.Order => -100f;
        float IBoneMotionEffect.Order => 0f;

        private ParalysisStatusEffect() { }

        protected override IRogueEffect AffectTo(RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var statusEffect = base.AffectTo(target, user, activationDepth, arg);
            if (RogueDevice.Primary.VisibleAt(target.Location, target.Position))
            {
                RogueDevice.Add(DeviceKw.EnqueueSE, StdKw.Paralysis);
                RogueDevice.Add(DeviceKw.AppendText, target);
                RogueDevice.Add(DeviceKw.AppendText, "は");
                RogueDevice.Add(DeviceKw.AppendText, this);
                RogueDevice.Add(DeviceKw.AppendText, "した！\n");
                RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateEffect(target.Position, CoreMotions.Paralysis, false));
            }
            return statusEffect;
        }

        public override void Open(RogueObj self)
        {
            base.Open(self);
            SpeedCalculator.SetDirty(self);
        }

        protected override bool Close(RogueObj self, StatusEffectCloseType closeType = StatusEffectCloseType.Manual)
        {
            SpeedCalculator.SetDirty(self);
            if (RogueDevice.Primary.VisibleAt(self.Location, self.Position))
            {
                RogueDevice.Add(DeviceKw.AppendText, self);
                RogueDevice.Add(DeviceKw.AppendText, "の");
                RogueDevice.Add(DeviceKw.AppendText, this);
                RogueDevice.Add(DeviceKw.AppendText, "がおさまった\n");
            }
            return base.Close(self);
        }

        void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
        {
            if (keyword == StatsKw.Speed)
            {
                value.SubValues[StatsKw.BeInhibited] = 1f;
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

            if (RogueDevice.Primary.Player.Main.Stats.Party.Members.Contains(self))
            {
                RogueDevice.Add(DeviceKw.AppendText, self);
                RogueDevice.Add(DeviceKw.AppendText, "はしびれて動けない\n");
            }
            return false;
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is ParalysisStatusEffect effect && effect.Stack == Stack && effect.LifeTime == LifeTime;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new ParalysisStatusEffect()
            {
                Stack = Stack,
                LifeTime = LifeTime
            };
        }

        void IBoneMotionEffect.ApplyTo(
            IMotionSet motionSet, IKeyword keyword, int animationTime, RogueDirection direction, ref RogueObjSpriteTransform transform)
        {
            if (keyword == MainInfoKw.Wait)
            {
                motionSet.GetPose(MainInfoKw.Wait, 0, direction, ref transform, out _);

                // 左右に震えさせる
                var x = (float)(animationTime / 2 % 2) / RoguegardSettings.PixelPerUnit;
                transform.Position += Vector3.right * x;
            }
            if (keyword == MainInfoKw.Hit)
            {
                motionSet.GetPose(MainInfoKw.Hit, animationTime, direction, ref transform, out _);

                // 左右に震えさせる
                var x = (float)(animationTime / 2 % 2) / RoguegardSettings.PixelPerUnit;
                transform.Position += Vector3.right * x;
            }
        }
    }
}
