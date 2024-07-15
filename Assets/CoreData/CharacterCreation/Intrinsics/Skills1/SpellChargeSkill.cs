using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;
using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class SpellChargeSkill : MPSkillIntrinsicOptionScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(parent, lv);
        }

        [Objforming.Formable]
        private class SortedIntrinsic : MPSkillSortedIntrinsic<SortedIntrinsic>
        {
            public override IRogueMethodTarget Target => ForPartyMemberRogueMethodTarget.Instance;
            public override IRogueMethodRange Range => UserRogueMethodRange.Instance;
            public override int RequiredMP => 1;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                MainCharacterWorkUtility.TryAddSkill(self);

                using var value = EffectableValue.Get();
                value.Initialize(0f);
                value.SubValues[MainInfoKw.Skill] = 1f;
                this.Affect(self, activationDepth, Effect.Callback, refValue: value);
                return true;
            }

            [Objforming.Formable]
            private class Effect : StackableStatusEffect, IRogueMethodActiveAspect
            {
                public static IAffectCallback Callback { get; } = new AffectCallback(new Effect());

                public override string Name => ":SpellCharge";
                public override IKeyword EffectCategory => EffectCategoryKw.Buff;
                protected override int MaxStack => 99;

                float IRogueMethodActiveAspect.Order => 0f;

                private static ISpriteMotion effect;

                private Effect() { }

                protected override void NewAffectTo(
                    RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
                {
                    if (MessageWorkListener.TryOpenHandler(target.Location, target.Position, out var h))
                    {
                        effect ??= new VariantSpriteMotion(CoreMotions.Buff, StatsKw.ATK.Color);
                        using var handler = h;
                        handler.AppendText(":StatusUpMsg::4").AppendText(target).AppendText(MainInfoKw.Skill).AppendText(StatsKw.ATK).AppendText(2);
                        handler.AppendText("\n");
                        handler.EnqueueWork(RogueCharacterWork.CreateEffect(target.Position, effect, false));
                    }
                }

                protected override void PreAffectedTo(
                    RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
                {
                    NewAffectTo(target, user, activationDepth, arg, statusEffect);
                }

                bool IRogueMethodActiveAspect.ActiveInvoke(
                    IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj target, float activationDepth, in RogueMethodArgument arg,
                    RogueMethodAspectState.ActiveChain chain)
                {
                    // スキルダメージ +2
                    // スキルで攻撃時のみ解除する (MainValue を取得して状態異常付与スキルと区別する)
                    if (keyword == MainInfoKw.Hit && arg.RefValue != null && arg.RefValue.SubValues.Is(MainInfoKw.Skill) &&
                        AttackUtility.GetUseValue(arg.RefValue))
                    {
                        arg.RefValue.MainValue += 2 * Stack;
                        RemoveClose(self);
                    }
                    return chain.Invoke(keyword, method, self, target, activationDepth, arg);
                }

                public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
                {
                    return other is Effect effect && effect.Stack == Stack;
                }

                public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
                {
                    return new Effect() { Stack = Stack };
                }
            }
        }
    }
}
