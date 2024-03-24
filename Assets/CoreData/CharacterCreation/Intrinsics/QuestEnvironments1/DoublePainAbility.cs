using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class DoublePainAbility : PartyAbilityIntrinsicOptionScript
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
        private class MemberEffect : StatusEffectPartyMemberRogueEffect<SortedIntrinsic>, IValueEffect
        {
            // �h��͂ƃK�[�h��K�p�������Ɠ�{�ɂ���
            float IValueEffect.Order => 10f;

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.DEF && !value.SubValues.Is(StdKw.Heal))
                {
                    // �󂯂�_���[�W��{
                    value.MainValue *= 2f;

                    // �K�[�h�h��͂���{
                    value.SubValues[StatsKw.GuardDEF] *= 2f;
                }
            }
        }
    }
}
