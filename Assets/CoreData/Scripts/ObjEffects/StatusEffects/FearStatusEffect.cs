using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    [Objforming.Formable]
    public class FearStatusEffect : TimeLimitedStackableStatusEffect, IRogueMethodActiveAspect, IValueEffect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new FearStatusEffect());

        public override string Name => "恐怖";
        public override IKeyword EffectCategory => EffectCategoryKw.StatusAilment;
        protected override int MaxStack => 1;
        protected override int InitialLifeTime => 100;

        float IRogueMethodActiveAspect.Order => 1f; // 成否判定のため 1
        float IValueEffect.Order => 0f;

        private static ISpriteMotion _smoke;

        private FearStatusEffect() { }

        protected override void NewAffectTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            if (MessageWorkListener.TryOpenHandler(target.Location, target.Position, out var h))
            {
                _smoke ??= new VariantSpriteMotion(CoreMotions.Smoke, new Color32(100, 200, 250, 255));
                using var handler = h;
                handler.EnqueueSE(StdKw.StatusEffect);
                handler.EnqueueWork(RogueCharacterWork.CreateEffect(target.Position, _smoke, false));
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
            RogueMethodAspectState.ActiveChain chain)
        {
            // 恐怖状態のとき、攻撃を失敗させる。
            if (keyword == MainInfoKw.Attack)
            {
                if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
                {
                    using var handler = h;
                    handler.AppendText(self).AppendText("は怖くて手が出せない！\n");
                }
                return false;
            }

            return chain.Invoke(keyword, method, self, target, activationDepth, arg);
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
