using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class PaintStatusEffect : TimeLimitedStackableStatusEffect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new PaintStatusEffect());

        public override string Name => ":Paint";
        public override IKeyword EffectCategory => EffectCategoryKw.StatusAilment;
        protected override int InitialLifeTime => 100;
        protected override int MaxStack => 1;

        protected override void NewAffectTo(RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            if (MessageWorkListener.TryOpenHandler(target.Location, target.Position, out var h))
            {
                using var handler = h;
                handler.AppendText(":PaintMsg::1").AppendText(target).AppendText("\n");
            }
        }

        protected override void PreAffectedTo(RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            NewAffectTo(target, user, activationDepth, arg, statusEffect);
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is PaintStatusEffect effect && effect.LifeTime == LifeTime && effect.Stack == Stack;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new PaintStatusEffect()
            {
                LifeTime = LifeTime,
                Stack = Stack
            };
        }
    }
}
