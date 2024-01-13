using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// <see cref="RogueObj"/> の色を変更するエフェクト
    /// </summary>
    public static class ColoringEffect
    {
        /// <summary>
        /// 既存のオブジェクトを着色する場合は
        /// <see cref="Extensions.RogueMethodExtension.Coloring(Extensions.IChangeStateRogueMethodCaller, RogueObj, RogueObj, Color, float)"/>
        /// を使用する。
        /// </summary>
        public static void ColorChange(RogueObj obj, Color color)
        {
            // すでに着色されていたらそれを解除する。
            RemoveColor(obj);

            var effect = new Effect(color);
            obj.Main.RogueEffects.AddOpen(obj, effect);
        }

        public static void RemoveColor(RogueObj obj)
        {
            if (obj.Main.RogueEffects.TryGetEffect<Effect>(out var effect))
            {
                effect.RemoveClose(obj);
            }
        }

        [ObjectFormer.Formable]
        private class Effect : IRogueEffect, IValueEffect
        {
            private readonly Color32 color;

            float IValueEffect.Order => 0f;

            [ObjectFormer.CreateInstance]
            private Effect() { }

            public Effect(Color32 color)
            {
                this.color = color;
            }

            void IRogueEffect.Open(RogueObj self)
            {
                RogueEffectUtility.AddFromRogueEffect(self, this);
            }

            public void RemoveClose(RogueObj self)
            {
                RogueEffectUtility.RemoveClose(self, this);
            }

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == CharacterCreationKw.Color)
                {
                    value.SubValues[CharacterCreationKw.Red] = color.r;
                    value.SubValues[CharacterCreationKw.Green] = color.g;
                    value.SubValues[CharacterCreationKw.Blue] = color.b;
                    value.SubValues[CharacterCreationKw.Alpha] = color.a;
                }
            }

            bool IRogueEffect.CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
            {
                return other is Effect colorOther && (Color)colorOther.color == color;
            }

            IRogueEffect IRogueEffect.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => this;
            IRogueEffect IRogueEffect.ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
