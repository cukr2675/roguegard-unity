using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class BaseBeEatenRogueMethod : BaseApplyRogueMethod
    {
        protected virtual int GetNutritionHeal(RogueObj self) => 50;

        public sealed override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (self.Stack == 0)
            {
                Debug.LogError("存在しないオブジェクトを消費しようとしました。");
                return false;
            }

            // 効果を発動しなくても消費する。
            if (MainCharacterWorkUtility.VisibleAt(user.Location, user.Position))
            {
                RogueDevice.Add(DeviceKw.AppendText, user);
                RogueDevice.Add(DeviceKw.AppendText, "は");
                RogueDevice.Add(DeviceKw.AppendText, self);
                if (self?.Main.InfoSet.Category == CategoryKw.Drink)
                {
                    RogueDevice.Add(DeviceKw.AppendText, "を飲んだ！\n");
                }
                else
                {
                    RogueDevice.Add(DeviceKw.AppendText, "を食べた！\n");
                }
            }

            BeEaten(self, user, activationDepth);
            var nutrition = GetNutritionHeal(self);
            if (nutrition >= 1)
            {
                var userStats = user.Main.Stats;
                userStats.SetNutrition(user, userStats.Nutrition + nutrition);
            }

            self.TrySetStack(self.Stack - 1, user); // オブジェクトを消費する。
            return true;
        }

        protected abstract void BeEaten(RogueObj self, RogueObj user, float activationDepth);
    }
}
