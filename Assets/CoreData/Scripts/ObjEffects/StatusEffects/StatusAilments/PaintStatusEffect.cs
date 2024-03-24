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
            if (MainCharacterWorkUtility.VisibleAt(target.Location, target.Position))
            {
                RogueDevice.Add(DeviceKw.AppendText, ":PaintMsg::1");
                RogueDevice.Add(DeviceKw.AppendText, target);
                RogueDevice.Add(DeviceKw.AppendText, "\n");
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
