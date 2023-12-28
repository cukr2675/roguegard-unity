using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class CaringAbility : PartyAbilityIntrinsicOptionScript
    {
		public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(lv);
        }

        private class SortedIntrinsic : AbilitySortedIntrinsic, IRogueObjUpdater, IValueEffect
        {
            float IRogueObjUpdater.Order => 100f;
            float IValueEffect.Order => 0f;

            private static readonly MemberEffect memberEffect = new MemberEffect();

            public SortedIntrinsic(int lv) : base(lv) { }

            RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                memberEffect.AffectToPartyMembersOf(self, false);
                return default;
            }

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.DEF)
                {
                    // ガード率-10%
                    value.SubValues[StatsKw.GuardRate] -= 0.1f;
                }
            }
        }

        [ObjectFormer.Formable]
        private class MemberEffect : StatusEffectPartyMemberRogueEffect<SortedIntrinsic>, IValueEffect
        {
            float IValueEffect.Order => AttackUtility.CupValueEffectOrder;

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.DEF && value.SubValues[StatsKw.GuardRate] > 0f)
                {
                    // ガードしうるとき10%でガード
                    value.SubValues[StatsKw.GuardRate] = AttackUtility.Cup(value.SubValues[StatsKw.GuardRate], 0.1f);
                }
            }
        }
    }
}
