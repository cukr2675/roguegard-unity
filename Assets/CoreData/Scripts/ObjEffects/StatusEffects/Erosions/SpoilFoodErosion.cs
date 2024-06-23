using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class SpoilFoodErosion : StackableStatusEffect, IRogueMethodPassiveAspect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new SpoilFoodErosion());

        public override string Name => ":SpoilFoodEffect";
        public override IKeyword EffectCategory => EffectCategoryKw.Erosion;
        protected override int MaxStack => 1;

        float IRogueMethodPassiveAspect.Order => 0f;

        private SpoilFoodErosion() { }

        protected override void NewAffectTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            if (RogueDevice.Primary.Player.Main.Stats.Party.Members.Contains(target.Location))
            {
                RogueDevice.Add(DeviceKw.AppendText, ":SpoilFoodMsg::1");
                RogueDevice.Add(DeviceKw.AppendText, target);
                RogueDevice.Add(DeviceKw.AppendText, "\n");
            }
        }

        protected override void PreAffectedTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            NewAffectTo(target, user, activationDepth, arg, statusEffect);
        }

        bool IRogueMethodPassiveAspect.PassiveInvoke(
            IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
            RogueMethodAspectState.PassiveNext next)
        {
            var result = next.Invoke(keyword, method, self, user, activationDepth, arg);

            // くさった食べ物を食べると、ランダムで悪い効果を起こす
            if (result && keyword == MainInfoKw.BeEaten && activationDepth < 1f)
            {
                var randomValue = RogueRandom.Primary.Next(0, 2);
                switch (randomValue)
                {
                    case 0:
                        default(IAffectRogueMethodCaller).TryAffect(user, 1f, PoisonStatusEffect.Callback);
                        break;
                    case 1:
                        default(IAffectRogueMethodCaller).TryAffect(user, 1f, ConfusionStatusEffect.Callback);
                        break;
                }
            }
            return result;
        }

        public override void GetEffectedName(RogueNameBuilder refName, RogueObj self)
        {
            refName.Insert0("くさった");
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is SpoilFoodErosion erosion && erosion.Stack == Stack;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new SpoilFoodErosion() { Stack = Stack };
        }
    }
}
