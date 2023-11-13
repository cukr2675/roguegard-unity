using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class BraveHeartAbility : PartyAbilityIntrinsicOption
    {
        public override string Name => ":BraveHeart";
        protected override int Lv => 1;
        protected override float Cost => 3f;

        protected override ISortedIntrinsic CreateSortedIntrinsic(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(lv);
        }

        private class SortedIntrinsic : AbilitySortedIntrinsic, IRogueObjUpdater
        {
            float IRogueObjUpdater.Order => 100f;

            private static readonly MemberEffect memberEffect = new MemberEffect();

            public SortedIntrinsic(int lv)
                : base(lv)
            {
            }

            RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                // パーティメンバーに効果を付与
                if (self.Main.Stats.Party == null)
                {
                    Add(self);
                }
                else
                {
                    var partyMembers = self.Main.Stats.Party.Members;
                    for (int i = 0; i < partyMembers.Count; i++)
                    {
                        Add(partyMembers[i]);
                    }
                }
                return default;

                void Add(RogueObj partyMember)
                {
                    if (partyMember.Main.RogueEffects.TryGetEffect<MemberEffect>(out _)) return;

                    partyMember.Main.RogueEffects.AddOpen(partyMember, memberEffect);
                }
            }
        }

        /// <summary>
        /// <see cref="PartyAbilityIntrinsicOption.BasePartyMemberRogueEffect.effecterEffect"/> のシリアル化が必要なため
        /// <see cref="ObjectFormer.FormableAttribute"/> を付与する
        /// </summary>
        [ObjectFormer.Formable]
        private class MemberEffect : BasePartyMemberRogueEffect<SortedIntrinsic>, IRogueMethodActiveAspect
        {
            float IRogueMethodActiveAspect.Order => 0f;

            bool IRogueMethodActiveAspect.ActiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj target, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.ActiveNext next)
            {
                var beforeLv = self.Main.Stats.Lv;
                var result = next.Invoke(keyword, method, self, target, activationDepth, arg);
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
                            closable.Close(self);
                        }
                    }
                    if (RogueDevice.Primary.VisibleAt(self.Location, self.Position))
                    {
                        RogueDevice.Add(DeviceKw.EnqueueSE, StdKw.Heal);
                        RogueDevice.Add(DeviceKw.AppendText, ":BraveHeartMsg::1");
                        RogueDevice.Add(DeviceKw.AppendText, self);
                        RogueDevice.Add(DeviceKw.AppendText, "\n");
                        RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateEffect(self.Position, CoreMotions.Heal, false));
                    }
                }
                return result;
            }
        }
    }
}
