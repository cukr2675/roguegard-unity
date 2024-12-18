using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;

namespace Roguegard.CharacterCreation
{
    public static class RogueColorUtility
    {
        private static readonly EffectableValue value = EffectableValue.Get();

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

        public static Color GetFirstColor(BoneKeyword boneName, IReadOnlyNodeBone rootNode, EffectableBoneSpriteTable boneSpriteTable)
        {
            var sprite = boneSpriteTable.GetSprite(boneName);
            if (sprite.OverridesSourceColor)
            {
                return sprite.FirstColor;
            }

            var bone = Recursion(rootNode);
            return bone.Color;

            IReadOnlyNodeBone Recursion(IReadOnlyNodeBone node)
            {
                if (node.Name == boneName) return node;

                for (int i = 0; i < node.Children.Count; i++)
                {
                    var result = Recursion(node.Children[i]);
                    if (result != null) return result;
                }
                return null;
            }
        }

        public static Color GetHairColor(ICharacterCreationData characterCreationData)
        {
            var appearances = characterCreationData.Appearances;
            for (int i = 0; i < appearances.Count; i++)
            {
                var appearance = appearances[i];
                if (appearance.Option?.BoneName != BoneKeyword.Hair) continue;

                return appearance.Color;
            }
            return Color.white;
        }
    }
}
