using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/AppearanceOption/Single")]
    [ObjectFormer.Referable]
    public class SingleAppearanceOption : ColoredAppearanceOption
    {
        [SerializeField] private ColorRangedBoneSprite _sprite = null;
        public ColorRangedBoneSprite Sprite { get => _sprite; set => _sprite = value; }

        protected override BoneSprite GetSprite(IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
            var bodyColor = characterCreationData.Race.BodyColor;
            var colorRange = RogueColorUtility.GetColorRange(bodyColor);
            var sprite = Sprite.GetSprite(bodyColor);
            return sprite;
        }
    }
}
