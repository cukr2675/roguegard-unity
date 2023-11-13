using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public static class RogueColorUtility
    {
        private static readonly AffectableValue value = AffectableValue.Get();

        public static ColorRange GetColorRange(Color color)
        {
            if (color.r > color.g && color.r > color.b)
            {
                // Red
                if (color.maxColorComponent >= 0.9f) return ColorRange.LightRed;
                else if (color.maxColorComponent >= 0.5f) return ColorRange.DarkRed;
                else return ColorRange.DarkOther;
            }
            else
            {
                // Other
                if (color.maxColorComponent >= 0.9f) return ColorRange.LightOther;
                else return ColorRange.DarkOther;
            }
        }

        public static Color GetColor(RogueObj obj)
        {
            var baseColor = obj.Main.InfoSet.Color;
            value.Initialize(0f);
            value.SubValues[CharacterCreationKw.Red] = baseColor.r;
            value.SubValues[CharacterCreationKw.Green] = baseColor.g;
            value.SubValues[CharacterCreationKw.Blue] = baseColor.b;
            value.SubValues[CharacterCreationKw.Alpha] = baseColor.a;
            ValueEffectState.AffectValue(CharacterCreationKw.Color, value, obj);
            var red = value.SubValues[CharacterCreationKw.Red];
            var green = value.SubValues[CharacterCreationKw.Green];
            var blue = value.SubValues[CharacterCreationKw.Blue];
            var alpha = value.SubValues[CharacterCreationKw.Alpha];
            return new Color(red, green, blue, alpha);
        }

        public static Color GetFirstColor(IKeyword boneName, IBoneNode boneRoot, AffectableBoneSpriteTable boneSpriteTable)
        {
            var sprite = boneSpriteTable.GetSprite(boneName);
            if (sprite.OverridesSourceColor)
            {
                return sprite.FirstColor;
            }

            var bone = Recursion(boneRoot);
            return bone.Color;

            IBone Recursion(IBoneNode node)
            {
                if (node.Bone.Name == boneName) return node.Bone;

                for (int i = 0; i < node.Children.Count; i++)
                {
                    var result = Recursion(node.Children[i]);
                    if (result != null) return result;
                }
                return null;
            }
        }
    }
}
