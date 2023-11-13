using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// パーティリーダーは最大満腹度が10倍になり、ターン経過で満腹度-1。
    /// 満腹度がゼロのときターン経過でHP-1。
    /// </summary>
    [ObjectFormer.Formable]
    public class UseNutritionLeaderEffect : PlayerLeaderRogueEffect, IRogueObjUpdater
    {
        float IRogueObjUpdater.Order => 100f;

        private static readonly UseNutritionLeaderEffect instance = new UseNutritionLeaderEffect();
        private static readonly MemberEffect memberEffect = new MemberEffect();
        private UseNutritionLeaderEffect() { }

        public static void Initialize(RogueObj playerObj)
        {
            var party = playerObj.Main.Stats.Party;
            if (party.Members[0] != playerObj) throw new RogueException(); // リーダーでなければ失敗する。

            playerObj.Main.RogueEffects.AddOpen(playerObj, instance);
        }

        RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
        {
            var mainStats = self.Main.Stats;
            const int nutritionCost = 1;
            if (mainStats.Nutrition > 0)
            {
                // リーダーの満腹度を減らす
                var nutrition = mainStats.Nutrition - nutritionCost;
                if (nutrition >= 1)
                {
                    mainStats.SetNutrition(self, nutrition);

                    if (nutrition == 200 && RogueDevice.Primary.Player == self)
                    {
                        RogueDevice.Add(DeviceKw.EnqueueSE, StatsKw.Hungry);
                        RogueDevice.Add(DeviceKw.AppendText, "おなかが空いてきた…\n");
                    }
                    else if (nutrition == 100 && RogueDevice.Primary.Player == self)
                    {
                        RogueDevice.Add(DeviceKw.EnqueueSE, StatsKw.Hungry);
                        RogueDevice.Add(DeviceKw.AppendText, "空腹で目が回ってきた…\n");
                    }
                }
                else
                {
                    // 満腹度がゼロになったら空腹状態にする。
                    mainStats.SetNutrition(self, 0);
                    HungerStatusEffect.Callback.AffectTo(self, null, activationDepth, RogueMethodArgument.Identity);
                }

                // リーダーの自然回復
                mainStats.Regenerate(self);
            }

            // パーティメンバーに自然回復効果を付与
            if (self.Main.Stats.Party == null)
            {
                Debug.LogError("プレイヤーキャラがパーティに所属していません。");
                return default;
            }

            var partyMembers = self.Main.Stats.Party.Members;
            for (int i = 1; i < partyMembers.Count; i++)
            {
                var member = partyMembers[i];
                if (member.Main.RogueEffects.Contains(memberEffect)) continue;

                member.Main.RogueEffects.AddOpen(member, memberEffect);
            }

            return default;
        }

        public override bool Equals(object obj) => obj.GetType() == GetType();
        public override int GetHashCode() => GetType().GetHashCode();

        /// <summary>
        /// このエフェクトが付与されているオブジェクトは自然回復を有効にする。
        /// </summary>
        [ObjectFormer.Formable]
        private class MemberEffect : BasePartyMemberRogueEffect<UseNutritionLeaderEffect>
        {
            protected override RogueObjUpdaterContinueType UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                if (self.Main.Stats.Nutrition > 0)
                {
                    self.Main.Stats.Regenerate(self);
                }
                return default;
            }
        }
    }
}
