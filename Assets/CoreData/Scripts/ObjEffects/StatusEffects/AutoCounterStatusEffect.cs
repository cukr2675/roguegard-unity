using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class AutoCounterStatusEffect : TimeLimitedStackableStatusEffect, IRogueMethodPassiveAspect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new AutoCounterStatusEffect());

        public override string Name => "オートカウンター";
        public override IKeyword EffectCategory => null;
        protected override int MaxStack => 1;
        protected override int InitialLifeTime => 10;

        float IRogueMethodPassiveAspect.Order => 0f;

        private static readonly AttackRogueMethod attack = new AttackRogueMethod();

        private AutoCounterStatusEffect() { }

        bool IRogueMethodPassiveAspect.PassiveInvoke(
            IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
            RogueMethodAspectState.PassiveChain chain)
        {
            var result = chain.Invoke(keyword, method, self, user, activationDepth, arg);
            if (!result) return false;

            if (keyword == MainInfoKw.Hit && activationDepth < 1f && user != null && arg.RefValue?.MainValue > 0f)
            {
                result |= Counter(arg.RefValue);
            }
            return true;

            bool Counter(EffectableValue baseDamage)
            {
                var distance = (user.Position - self.Position).sqrMagnitude;
                if (distance <= 2)
                {
                    // 受けたダメージを返す
                    using var damage = EffectableValue.Get();
                    damage.Initialize(baseDamage.MainValue);
                    var attackArg = new RogueMethodArgument(targetPosition: user.Position, value: damage);
                    RogueMethodAspectState.Invoke(MainInfoKw.Attack, attack, self, null, 1f, attackArg);
                }
                return false;
            }
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is AutoCounterStatusEffect effect && effect.Stack == Stack && effect.LifeTime == LifeTime;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new AutoCounterStatusEffect()
            {
                Stack = Stack,
                LifeTime = LifeTime
            };
        }

        private class AttackRogueMethod : IActiveRogueMethod
        {
            public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (RaycastAssert.RequireTarget(FrontRogueMethodRange.Instance, self, arg, out var target)) return false;
                if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
                {
                    using var handler = h;
                    handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(self, CoreMotions.Discus, false));
                }

                this.TryHurt(target, self, activationDepth, arg.RefValue);
                this.TryDefeat(target, self, activationDepth, arg.RefValue);
                return true;
            }
        }
    }
}
