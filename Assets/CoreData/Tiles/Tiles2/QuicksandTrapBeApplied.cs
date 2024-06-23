using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class QuicksandTrapBeApplied : BaseApplyRogueMethod
    {
        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (arg.Other is UserRogueTile userTile && !StatsEffectedValues.AreVS(userTile.User, user))
            {
                // 敵対していないキャラが罠を踏んでも起動しないようにする
                return false;
            }

            return this.TryAffect(user, activationDepth, Effect.Callback);
        }

        [Objforming.Formable]
        private class Effect : TimeLimitedStackableStatusEffect, IValueEffect
        {
            public static IAffectCallback Callback { get; } = new AffectCallback(new Effect());

            public override string Name => ":QuicksandTrap";
            protected override int MaxStack => 1;
            protected override int InitialLifeTime => 10;
            public override IKeyword EffectCategory => null;

            float IValueEffect.Order => 0f;

            private Effect() { }

            public override void Open(RogueObj self)
            {
                base.Open(self);
                SpeedCalculator.SetDirty(self);
            }

            protected override void RemoveClose(RogueObj self, StatusEffectCloseType closeType = StatusEffectCloseType.Manual)
            {
                base.RemoveClose(self);
                SpeedCalculator.SetDirty(self);
            }

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.Speed)
                {
                    // 行動不可
                    value.SubValues[StatsKw.BeInhibited] = 1f;
                }
            }

            public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
            {
                return other is Effect effect && effect.Stack == Stack && effect.LifeTime == LifeTime;
            }

            public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                return new Effect()
                {
                    Stack = Stack,
                    LifeTime = LifeTime
                };
            }
        }
    }
}
