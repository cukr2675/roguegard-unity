using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CostedBeEaten : BaseApplyRogueMethod
    {
        public override IRogueMethodTarget Target => null;
        public override IRogueMethodRange Range => UserRogueMethodRange.Instance;

        public sealed override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (self.Stack == 0)
            {
                Debug.LogError("存在しないオブジェクトを消費しようとしました。");
                return false;
            }

            // 効果を発動しなくても消費する。
            var nutrition = self.Main.InfoSet.Cost > 0f ? 50 : 0;
            if (nutrition <= 0)
            {
                if (MessageWorkListener.TryOpenHandler(user.Location, user.Position, out var h1))
                {
                    using var handler = h1;
                    handler.AppendText("何も起こらなかった\n");
                }

                self.TrySetStack(self.Stack - 1, user); // オブジェクトを消費する。
                return true;
            }

            var userStats = user.Main.Stats;
            var oldNutrition = userStats.Nutrition;
            userStats.SetNutrition(user, userStats.Nutrition + nutrition);
            var deltaNutrition = userStats.Nutrition - oldNutrition;
            if (MessageWorkListener.TryOpenHandler(user.Location, user.Position, out var h2))
            {
                using var handler = h2;
                handler.AppendText(user).AppendText("の満腹度が").AppendText(deltaNutrition).AppendText("回復した！\n");
            }

            self.TrySetStack(self.Stack - 1, user); // オブジェクトを消費する。
            return true;
        }
    }
}
