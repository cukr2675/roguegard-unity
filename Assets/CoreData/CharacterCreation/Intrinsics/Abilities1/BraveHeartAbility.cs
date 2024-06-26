using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class BraveHeartAbility : PartyAbilityIntrinsicOptionScript
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
                var beforeLv = self.Main.Stats.Lv;
                var result = chain.Invoke(keyword, method, self, target, activationDepth, arg);
                if (result && keyword == StdKw.LoseExp && self.Main.Stats.Lv > beforeLv)
                {
                    // 経験値取得でレベルが上がったとき HP と MP を全回復し、状態異常をすべて解除する
                    self.Main.Stats.SetHP(self, int.MaxValue);
                    self.Main.Stats.SetMP(self, int.MaxValue);
                    var statusEffectState = self.Main.GetStatusEffectState(self);
                    var statusEffects = statusEffectState.StatusEffects;
                    for (int i = statusEffects.Count - 1; i >= 0; i--)
                    {
                        var statusEffect = statusEffects[i];
                        if (statusEffect.EffectCategory == EffectCategoryKw.StatusAilment &&
                            statusEffect is IClosableStatusEffect closable)
                        {
                            closable.RemoveClose(self);
                        }
                    }
                    if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
                    {
                        using var handler = h;
                        handler.EnqueueSE(StdKw.Heal);
                        handler.AppendText(":BraveHeartMsg::1").AppendText(self).AppendText("\n");
                        handler.EnqueueWork(RogueCharacterWork.CreateEffect(self.Position, CoreMotions.Heal, false));
                    }
                }
                return result;
            }
        }
    }
}
