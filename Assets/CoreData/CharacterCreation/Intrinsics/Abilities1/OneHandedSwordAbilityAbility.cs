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

        private class SortedIntrinsic : AbilitySortedIntrinsic, IRogueMethodActiveAspect, IValueEffect, IRogueObjUpdater
        {
            /// <summary>
            /// 通常攻撃回数をカウントする
            /// </summary>
            [System.NonSerialized] private int attackCount;
            [System.NonSerialized] private float attackActivationDepth;

            private bool buffIsEnabled;

            float IRogueMethodActiveAspect.Order => 0f;
			float IValueEffect.Order => AttackUtility.BaseCupValueEffectOrder;
			float IRogueObjUpdater.Order => -100f; // 行動前にバフを無効化

			public SortedIntrinsic(int lv) : base(lv) { }

            bool IRogueMethodActiveAspect.ActiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj target, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.ActiveNext next)
            {
                if (keyword == MainInfoKw.Attack)
                {
                    // 通常攻撃する
                    attackCount = 0;
                    attackActivationDepth = activationDepth;
                    var result = next.Invoke(keyword, method, self, target, activationDepth, arg);

                    // 通常攻撃後、攻撃回数が 2 であれば剣を使ったとみなしてバフを有効化
                    buffIsEnabled = attackCount == 2;
                    return result;
                }
                if (keyword == MainInfoKw.Hit && activationDepth == attackActivationDepth && arg.RefValue?.MainValue > 0f)
                {
                    // 攻撃が失敗しても、攻撃すればそれだけでカウントする
                    attackCount++;
                }
                return next.Invoke(keyword, method, self, target, activationDepth, arg);
            }

			void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
			{
                if (buffIsEnabled && keyword == StatsKw.DEF)
                {
                    // 剣を振ったら次のターンまで 40% でガード
                    value.SubValues[StatsKw.GuardRate] = AttackUtility.Cup(value.SubValues[StatsKw.GuardRate], 0.4f);
                }
			}

			RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
			{
                buffIsEnabled = false;
                return default;
			}
		}
    }
}
