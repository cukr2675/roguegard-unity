using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class SpoilFoodTrapBeApplied : BaseApplyRogueMethod
    {
        private static readonly List<int> foodIndices = new();

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var userTile = arg.Other as UserRogueTile;
            if (userTile != null && !StatsEffectedValues.AreVS(userTile.User, user))
            {
                // 敵対していないキャラが罠を踏んでも起動しないようにする
                return false;
            }

            // ランダムな食べ物を取得
            var userItems = user.Space.Objs;
            foodIndices.Clear();
            for (int i = 0; i < userItems.Count; i++)
            {
                var item = userItems[i];
                if (item == null || item.Main.InfoSet.Category != CategoryKw.Food) continue;

                foodIndices.Add(i);
            }

            bool result;
            if (foodIndices.Count >= 1)
            {
                var index = RogueRandom.Primary.Choice(foodIndices);
                var food = userItems[index];

                // 腐らせる
                result = this.Affect(food, activationDepth, SpoilFoodErosion.Callback, user: userTile?.User);
            }
            else
            {
                result = false;
            }

            if (!result && MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                using var handler = h;
                handler.AppendText("何も起こらなかった").AppendText("\n");
                MainCharacterWorkUtility.TryAddSkill(self);
                MainCharacterWorkUtility.TryAddShot(self);
            }
            return true;
        }
    }
}
