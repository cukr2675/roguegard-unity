using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OchalikeSprites;

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

        public static Color GetMorphedBareColor(BoneKeyword boneName, IReadOnlyOchalikeBone rootBone, OchalikeMorph ochalikeMorph)
        {
            var sprite = ochalikeMorph.GetSprite(boneName);
            if (sprite.MorphBareColor.HasValue)
            {
                return sprite.MorphBareColor.Value;
            }

            var bone = Recursion(rootBone);
            return bone.BareColor;

            IReadOnlyOchalikeBone Recursion(IReadOnlyOchalikeBone bone)
            {
                if (bone.Name == boneName) return bone;

                for (int i = 0; i < bone.Children.Count; i++)
                {
                    var result = Recursion(bone.Children[i]);
                    if (result != null) return result;
                }
                return null;
            }
        }

        public static Color GetHairColor(ICharacterCreationData characterCreationData)
        {
            var appearances = characterCreationData.Appearances;
            foreach (var appearance in appearances)
            {
                if (appearance.Option?.BoneName == BoneKeyword.Hair) return appearance.Color;
            }
            return Color.white;
        }
    }
}
