using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/AppearanceOption/Single")]
    [Objforming.Referable]
    public class SingleAppearanceOption : ColoredAppearanceOption
    {
        [SerializeField] private ColorRangedBoneSprite _sprite = null;
        public ColorRangedBoneSprite Sprite { get => _sprite; set => _sprite = value; }

        protected override BoneSprite GetSprite(IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
            var hairColor = RogueColorUtility.GetHairColor(characterCreationData);
            var bright = hairColor.maxColorComponent >= SkeletalSpriteUtility.LightDarkThreshold;
            var sprite = Sprite.GetSprite(bright);
            return sprite;
        }
    }
}
