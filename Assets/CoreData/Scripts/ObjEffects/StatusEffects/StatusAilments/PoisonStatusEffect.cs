using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;
using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class PoisonStatusEffect : TimeLimitedStackableStatusEffect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new PoisonStatusEffect());

        public override string Name => "毒";
        public override IKeyword EffectCategory => EffectCategoryKw.StatusAilment;
        protected override int MaxStack => 1;
        protected override int InitialLifeTime => 50;

        private int cooldown;

        private static ISpriteMotion _smoke;

        private PoisonStatusEffect() { }

        protected override IRogueEffect AffectTo(RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (MessageWorkListener.TryOpenHandler(target.Location, target.Position, out var h))
            {
                _smoke ??= new VariantSpriteMotion(CoreMotions.Smoke, new Color32(200, 50, 200, 255));
                using var handler = h;
                handler.AppendText(target).AppendText("は").AppendText(this).AppendText("を浴びた！\n");
                handler.EnqueueSE(StdKw.Poison);
                handler.EnqueueWork(RogueCharacterWork.CreateEffect(target.Position, _smoke, false));
            }
            return base.AffectTo(target, user, activationDepth, arg);
        }

        protected override void RemoveClose(RogueObj self, StatusEffectCloseType closeType = StatusEffectCloseType.Manual)
        {
            if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                using var handler = h;
                handler.AppendText(self).AppendText("から").AppendText(this).AppendText("が抜けた\n");
            }
            base.RemoveClose(self);
        }

        protected override RogueObjUpdaterContinueType UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
        {
            cooldown--;
            if (cooldown <= 0)
            {
                // 1ダメージ
                using var damage = EffectableValue.Get();
                damage.Initialize(1f);
                damage.SubValues[StatsKw.GuardRate] = -1000000f; // ガード不可
                this.Hurt(self, null, AttackUtility.GetActivationDepthCantCounter(activationDepth), damage);
                this.TryDefeat(self, null, activationDepth, damage);

                // 継続ダメージ
                cooldown = 5;
            }
            return default;
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is PoisonStatusEffect effect && effect.Stack == Stack && effect.LifeTime == LifeTime;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new PoisonStatusEffect()
            {
                Stack = Stack,
                LifeTime = LifeTime
            };
        }
    }
}
