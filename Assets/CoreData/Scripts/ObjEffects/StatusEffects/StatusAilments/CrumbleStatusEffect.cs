using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class CrumbleStatusEffect : StackableStatusEffect, IValueEffect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new CrumbleStatusEffect());

        public override string Name => "岩崩れ";
        public override IKeyword EffectCategory => null;
        protected override int MaxStack => 2;

        float IValueEffect.Order => -1;

        private CrumbleStatusEffect() { }

        protected override void NewAffectTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            if (MainCharacterWorkUtility.VisibleAt(target.Location, target.Position))
            {
                RogueDevice.Add(DeviceKw.AppendText, target);
                RogueDevice.Add(DeviceKw.AppendText, "は少し崩れた！\n");
            }
        }

        void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
        {
            if (keyword == StatsKw.DEF)
            {
                // 防御力 -1 (IValueEffect.Order: -1 につき防御力 0 未満にはならない)
                value.MainValue = Mathf.Min(value.MainValue + 2f, value.BaseMainValue);
            }
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is CrumbleStatusEffect effect && effect.Stack == Stack;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new CrumbleStatusEffect()
            {
                Stack = Stack
            };
        }
    }
}
