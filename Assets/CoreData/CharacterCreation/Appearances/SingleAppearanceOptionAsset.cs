using OchalikeSprites;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Appearance/Single")]
    [Objforming.Referable]
    public class SingleAppearanceOptionAsset : ColoredAppearanceOptionAsset
    {
        [SerializeField] private ColorRangedBoneSprite _sprite = null;
        public ColorRangedBoneSprite Sprite { get => _sprite; set => _sprite = value; }

        protected override BoneSprite GetSprite(IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
            var hairColor = RogueColorUtility.GetHairColor(characterCreationData);
            var useDarkOutline = OchalikeSpritesUtility.IsSimilarToLightOutline(hairColor);
            var sprite = Sprite.GetSprite(useDarkOutline);
            return sprite;
        }
    }
}
