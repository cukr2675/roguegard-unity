using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class FearStatusEffect : TimeLimitedStackableStatusEffect, IRogueMethodActiveAspect, IValueEffect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new FearStatusEffect());

        public override string Name => "恐怖";
        public override IKeyword EffectCategory => EffectCategoryKw.StatusAilment;
        protected override int MaxStack => 1;
        protected override int InitialLifeTime => 100;

        float IRogueMethodActiveAspect.Order => 1f; // 成否判定のため 1
        float IValueEffect.Order => 0f;

        private static IBoneMotion _smoke;

        private FearStatusEffect() { }

        protected override void NewAffectTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            if (MainCharacterWorkUtility.VisibleAt(target.Location, target.Position))
            {
                RogueDevice.Add(DeviceKw.EnqueueSE, StdKw.StatusEffect);
                _smoke ??= new VariantBoneMotion(CoreMotions.Smoke, new Color32(100, 200, 250, 255));
                var work = RogueCharacterWork.CreateEffect(target.Position, _smoke, false);
                RogueDevice.AddWork(DeviceKw.EnqueueWork, work);
            }
        }

        public override void Open(RogueObj self)
        {
            MovementCalculator.SetDirty(self);
            base.Open(self);
        }

        protected override void RemoveClose(RogueObj self, StatusEffectCloseType closeType = StatusEffectCloseType.Manual)
        {
            MovementCalculator.SetDirty(self);
            base.RemoveClose(self);
        }

        void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
        {
            // 自律行動する敵は、恐怖状態のとき逃げる。
            // 恐怖状態の判定のために値を設定する。
            if (keyword == StatsKw.Movement)
            {
                value.SubValues[StdKw.Fear] = 1f;
            }
        }

        bool IRogueMethodActiveAspect.ActiveInvoke(
            IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj target, float activationDepth, in RogueMethodArgument arg,
            RogueMethodAspectState.ActiveNext next)
        {
            // 恐怖状態のとき、攻撃を失敗させる。
            if (keyword == MainInfoKw.Attack)
            {
                if (MainCharacterWorkUtility.VisibleAt(self.Location, self.Position))
                {
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, "は怖くて手が出せない！\n");
                }
                return false;
            }

            return next.Invoke(keyword, method, self, target, activationDepth, arg);
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is FearStatusEffect effect && effect.Stack == Stack && effect.LifeTime == LifeTime;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new FearStatusEffect()
            {
                Stack = Stack,
                LifeTime = LifeTime
            };
        }
    }
}
