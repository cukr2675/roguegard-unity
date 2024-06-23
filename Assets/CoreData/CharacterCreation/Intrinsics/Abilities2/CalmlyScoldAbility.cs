using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class CalmlyScoldAbility : AbilityIntrinsicOptionScript
    {
		public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(lv);
        }

        private class SortedIntrinsic : AbilitySortedIntrinsic, IRogueMethodPassiveAspect
        {
            float IRogueMethodPassiveAspect.Order => 0f;

            public SortedIntrinsic(int lv) : base(lv) { }

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveChain chain)
            {
                if (keyword == MainInfoKw.Hit && (self.Main.Stats.Party?.Members.Contains(user) ?? false) && AttackUtility.GetUseValue(arg.RefValue))
                {
                    // 味方からの被会心率-5%
                    arg.RefValue.SubValues[StatsKw.CriticalRate] -= .05f;
                }
                return chain.Invoke(keyword, method, self, user, activationDepth, arg);
            }
        }
    }
}
