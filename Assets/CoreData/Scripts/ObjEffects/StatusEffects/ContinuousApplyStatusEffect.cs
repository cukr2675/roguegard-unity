using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class ContinuousApplyStatusEffect : TimeLimitedStackableStatusEffect, IValueEffect, IRogueMethodPassiveAspect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new ContinuousApplyStatusEffect());

        public override string Name => "継続使用";
        public override string Caption => "道具を連続で使用している状態　道具の使用が完了するか　攻撃を受けると解除される";
        public override IKeyword EffectCategory => null;
        protected override int MaxStack => 1;
        protected override int InitialLifeTime => 100;

        private RogueObj tool;

        float IValueEffect.Order => 1f;
        float IRogueMethodPassiveAspect.Order => 0f;

        private ContinuousApplyStatusEffect() { }

        protected override IRogueEffect AffectTo(RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var effect = (ContinuousApplyStatusEffect)base.AffectTo(target, user, activationDepth, arg);
            effect.tool = arg.Tool;
            return effect;
        }

        public override void Open(RogueObj self)
        {
            base.Open(self);
            SpeedCalculator.SetDirty(self);
        }

        protected override bool RemoveClose(RogueObj self, StatusEffectCloseType closeType = StatusEffectCloseType.Manual)
        {
            if (closeType == StatusEffectCloseType.TimeLimit) { Debug.LogWarning($"{nameof(ContinuousApplyStatusEffect)} をターン経過で解除しました。"); }

            SpeedCalculator.SetDirty(self);
            return base.RemoveClose(self, closeType);
        }

        protected override RogueObjUpdaterContinueType UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
        {
            // 自動で道具を使用する
            var beAppliedMethod = tool.Main.InfoSet.BeApplied;
            var arg = new RogueMethodArgument(count: InitialLifeTime - LifeTime);
            RogueMethodAspectState.Invoke(MainInfoKw.BeApplied, beAppliedMethod, tool, self, activationDepth, arg);

            return base.UpdateObj(self, activationDepth, ref sectionIndex);
        }

        void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
        {
            if (keyword == StatsKw.Speed)
            {
                value.SubValues[StatsKw.BeInhibited] = 1f;
            }
        }

        bool IRogueMethodPassiveAspect.PassiveInvoke(
            IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
            RogueMethodAspectState.PassiveNext next)
        {
            var result = next.Invoke(keyword, method, self, user, activationDepth, arg);

            if (keyword == MainInfoKw.Hit && result && arg.Other != Callback) // Callback との等価判定で、エフェクト付与直後に解除されないようにする
            {
                // 攻撃を受けると解除される
                RemoveClose(self);
                if (RogueDevice.Primary.VisibleAt(self.Location, self.Position))
                {
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, "は");
                    RogueDevice.Add(DeviceKw.AppendText, tool);
                    RogueDevice.Add(DeviceKw.AppendText, "の使用を中止した\n");
                }
            }

            return result;
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is ContinuousApplyStatusEffect effect && effect.Stack == Stack && effect.LifeTime == LifeTime;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new ContinuousApplyStatusEffect()
            {
                Stack = Stack,
                LifeTime = LifeTime
            };
        }
    }
}
