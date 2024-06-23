using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class FoodAppraisalAbility : PartyAbilityIntrinsicOptionScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(lv);
        }

        private class SortedIntrinsic : AbilitySortedIntrinsic, IRogueObjUpdater
        {
            float IRogueObjUpdater.Order => 100f;

            private static readonly MemberEffect memberEffect = new MemberEffect();

            public SortedIntrinsic(int lv) : base(lv) { }

            RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                memberEffect.AffectToPartyMembersOf(self, true);
                return default;
            }
        }

        [Objforming.Formable]
        private class MemberEffect : StatusEffectPartyMemberRogueEffect<SortedIntrinsic>, IRogueMethodActiveAspect
        {
            float IRogueMethodActiveAspect.Order => 0f;

            bool IRogueMethodActiveAspect.ActiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj target, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.ActiveChain chain)
            {
                var stats = self.Main.Stats;
                var oldNutrition = stats.Nutrition;
                var result = chain.Invoke(keyword, method, self, target, activationDepth, arg);
                if (!result) return false;

                if (result && keyword == MainInfoKw.Eat)
                {
                    // 食べ物を食べたときの満腹度回復の20%を追加で回復する
                    var deltaNutrition = (stats.Nutrition - oldNutrition) / 5;
                    stats.SetNutrition(self, stats.Nutrition + deltaNutrition);
                }
                return true;
            }
        }
    }
}
