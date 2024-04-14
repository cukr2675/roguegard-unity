using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class FillTheBellyBeEaten : BaseBeEatenRogueMethod
    {
        [SerializeField] private int _nutrition = 500;

        private FillTheBellyBeEaten() { }

        public override IRogueMethodTarget Target => null;
        public override IRogueMethodRange Range => UserRogueMethodRange.Instance;

        protected override int GetNutritionHeal(RogueObj self) => 0;

        protected override void BeEaten(RogueObj self, RogueObj user, float activationDepth)
        {
            var userStats = user.Main.Stats;
            var oldNutrition = userStats.Nutrition;
            userStats.SetNutrition(user, userStats.Nutrition + _nutrition);
            var deltaNutrition = userStats.Nutrition - oldNutrition;
            if (MessageWorkListener.TryOpenHandler(user.Location, user.Position, out var h))
            {
                using var handler = h;
                handler.AppendText(user).AppendText("の満腹度が").AppendText(deltaNutrition).AppendText("回復した！\n");
            }
        }
    }
}
