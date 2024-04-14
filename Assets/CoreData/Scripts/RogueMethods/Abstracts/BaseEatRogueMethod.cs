using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public abstract class BaseEatRogueMethod : ReferableScript, IEatActiveRogueMethod
    {
        public abstract string Name { get; }
        public abstract Spanning<IKeyword> Edibles { get; }

        Sprite IRogueDescription.Icon => null;
        Color IRogueDescription.Color => Color.white;
        string IRogueDescription.Caption => null;
        IRogueDetails IRogueDescription.Details => null;

        private bool ContainsEdible(RogueObj tool)
        {
            // 食べ物アイテムは誰でも食べられる。
            if (tool.Main.InfoSet.Category == CategoryKw.Food || tool.Main.InfoSet.Category == CategoryKw.Drink) return true;

            using var value = AffectableValue.Get();
            StatsEffectedValues.GetMaterial(tool, value);
            var edible = false;
            for (int i = 0; i < Edibles.Count; i++)
            {
                if (value.SubValues.Is(Edibles[i]))
                {
                    edible = true;
                    break;
                }
            }
            return edible;
        }

        public virtual bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.RequireTool(arg, out var tool)) return false;

            if (!ContainsEdible(tool))
            {
                Debug.Log("これは食べられません。");
                return false;
            }

            if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                using var handler = h;
                handler.AppendText(self).AppendText("は").AppendText(tool);
                if (tool?.Main.InfoSet.Category == CategoryKw.Drink)
                {
                    handler.AppendText("を飲んだ！\n");
                }
                else
                {
                    handler.AppendText("を食べた！\n");
                }
                handler.EnqueueWork(RogueCharacterWork.CreateSync(self));
                handler.EnqueueSE(MainInfoKw.Eat);
                handler.EnqueueWork(RogueCharacterWork.CreateEffect(self.Position, CoreMotions.Eat, false));
            }

            var result = this.Eat(tool, self, activationDepth, RogueMethodArgument.Identity);
            return result;

        }
    }
}
