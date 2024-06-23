using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class OneHandedSwordAbility : AbilityIntrinsicOptionScript
    {
		public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
		{
            return new SortedIntrinsic(lv);
		}

        private class SortedIntrinsic : AbilitySortedIntrinsic, IRogueMethodPassiveAspect, IRogueMethodActiveAspect
        {
            /// <summary>
            /// 通常攻撃回数をカウントする
            /// </summary>
            [System.NonSerialized] private int attackCount;
            [System.NonSerialized] private float attackActivationDepth;

            float IRogueMethodPassiveAspect.Order => 0f;
            float IRogueMethodActiveAspect.Order => 0f;

            public SortedIntrinsic(int lv) : base(lv) { }

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveChain chain)
            {
                if (keyword == MainInfoKw.Attack)
                {
                    // 通常攻撃する
                    attackCount = 0;
                    attackActivationDepth = activationDepth;
                    var result = chain.Invoke(keyword, method, self, user, activationDepth, arg);

                    if (attackCount == 2)
                    {
                        // 通常攻撃後、攻撃回数が 2 であれば剣を使ったとみなしてバフを有効化
                        Effect.AffectTo(self);
                    }
                    return result;
                }
                return chain.Invoke(keyword, method, self, user, activationDepth, arg);
            }

            bool IRogueMethodActiveAspect.ActiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj target, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.ActiveChain chain)
            {
                if (keyword == MainInfoKw.Hit && activationDepth == attackActivationDepth && AttackUtility.GetUseValue(arg.RefValue))
                {
                    // 攻撃が失敗しても、攻撃すればそれだけでカウントする
                    attackCount++;
                }
                return chain.Invoke(keyword, method, self, target, activationDepth, arg);
            }
        }

        [Objforming.Formable]
        private class Effect : TimeLimitedStackableStatusEffect, IValueEffect
        {
            public override string Name => ":OneHandedSwordAbility";
            public override IKeyword EffectCategory => null;
            protected override int InitialLifeTime => 2;
            protected override int MaxStack => 1;

            float IValueEffect.Order => AttackUtility.BaseCupValueEffectOrder;

            private static Effect instance = new Effect();

            public static void AffectTo(RogueObj obj)
            {
                instance.AffectTo(obj, null, 0f, RogueMethodArgument.Identity);
            }

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.DEF)
                {
                    // 剣を振ったら次のターンまで 40% でガード
                    value.SubValues[StatsKw.GuardRate] = AttackUtility.Cup(value.SubValues[StatsKw.GuardRate], 0.4f);
                }
            }

            public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => false;

            public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                var clone = new Effect();
                clone.LifeTime = LifeTime;
                clone.Stack = Stack;
                return clone;
            }
        }
    }
}
