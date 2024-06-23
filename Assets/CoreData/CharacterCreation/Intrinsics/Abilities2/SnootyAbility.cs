using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class SnootyAbility : AbilityIntrinsicOptionScript
    {
		public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(lv);
        }

        private class SortedIntrinsic : AbilitySortedIntrinsic, IRogueObjUpdater, IRogueMethodPassiveAspect, IValueEffect
        {
            float IRogueObjUpdater.Order => 100f;
            float IRogueMethodPassiveAspect.Order => 0f;
            float IValueEffect.Order => 10f; // 会心ダメージが発生しうるとき

            private bool readyToBuff;
            private bool buffIsEnabled;

            public SortedIntrinsic(int lv) : base(lv) { }

            RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                if (readyToBuff)
                {
                    buffIsEnabled = true;
                }
                readyToBuff = true;
                return default;
            }

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.ATK && buffIsEnabled && value.SubValues[StatsKw.CriticalRate] > 0f)
                {
                    // 会心ダメージが発生しうるとき会心率+5%
                    value.SubValues[StatsKw.CriticalRate] += 0.05f;
                }
            }

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveChain chain)
            {
                var result = chain.Invoke(keyword, method, self, user, activationDepth, arg);
                if (result && keyword == MainInfoKw.Hit && arg.RefValue?.MainValue > 0f)
                {
                    // ダメージを受けたときバフを解除
                    readyToBuff = false;
                    buffIsEnabled = false;
                }
                return result;
            }
        }
    }
}
