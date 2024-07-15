using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    [Objforming.Formable]
    public class HungerStatusEffect : StackableStatusEffect, IValueEffect, IRogueObjUpdater
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new HungerStatusEffect());

        public override string Name => "空腹";
        public override IKeyword EffectCategory => null;
        protected override int MaxStack => 1;

        float IValueEffect.Order => 0f;
        float IRogueObjUpdater.Order => 100f;

        private int messageIndex = 0;

        private HungerStatusEffect() { }

        protected override void NewAffectTo(
            RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg, StackableStatusEffect statusEffect)
        {
            if (RogueDevice.Primary.Player == target)
            {
                RogueDevice.Add(DeviceKw.EnqueueSE, StatsKw.Hungry);
                RogueDevice.Add(DeviceKw.AppendText, "ダメだ！もう限界だ…\n");
            }
        }

        public override void Open(RogueObj self)
        {
            SpeedCalculator.SetDirty(self);
            base.Open(self);
        }

        void IValueEffect.AffectValue(IKeyword keyword, EffectableValue value, RogueObj self)
        {
            if (keyword == StatsKw.Speed)
            {
                // 仲間は空腹のとき動けない。
                value.SubValues[StatsKw.Hungry] = 1f;
            }
        }

        RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
        {
            var mainStats = self.Main.Stats;
            if (mainStats.Nutrition >= 1)
            {
                // 満腹度が 1 以上になったとき、空腹状態を回復する。
                SpeedCalculator.SetDirty(self);
                RogueEffectUtility.RemoveClose(self, this);
                return default;
            }

            if (self.Main.GetPlayerLeaderInfo(self) != null)
            {
                // リーダーが空腹のときはダメージを受ける。
                const int autoDamage = 1;
                mainStats.SetHP(self, mainStats.HP - autoDamage);
                if (messageIndex == 0f)
                {
                    if (RogueDevice.Primary.Player == self)
                    {
                        RogueDevice.Add(DeviceKw.AppendText, "はやく、何か食べないと！\n");
                    }
                    messageIndex++;
                }
                else if (messageIndex == 1)
                {
                    if (RogueDevice.Primary.Player == self)
                    {
                        RogueDevice.Add(DeviceKw.AppendText, "倒れてしまう！\n");
                    }
                    messageIndex++;
                }

                if (mainStats.HP <= 0f)
                {
                    this.Defeat(self, null, activationDepth);
                }
            }
            return default;
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => other == this;
        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => this;
    }
}
