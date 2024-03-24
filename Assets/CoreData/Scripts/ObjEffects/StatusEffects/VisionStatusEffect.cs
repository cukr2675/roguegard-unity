using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class VisionStatusEffect : StackableStatusEffect, IValueEffect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new VisionStatusEffect());

        public override string Name => "千里眼";
        public override IKeyword EffectCategory => null;
        protected override int MaxStack => 1;

        float IValueEffect.Order => 0f;

        private VisionStatusEffect() { }

        void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
        {
            if (keyword == StdKw.View)
            {
                value.MainValue += 10000f;
            }
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is VisionStatusEffect effect && effect.Stack == Stack;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new VisionStatusEffect()
            {
                Stack = Stack
            };
        }
    }
}
