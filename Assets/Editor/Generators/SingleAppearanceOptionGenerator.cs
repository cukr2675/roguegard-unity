using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;
using Roguegard.Editor;

namespace Roguegard.CharacterCreation.Editor
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Appearance/SingleGenerator")]
    public class SingleAppearanceOptionGenerator : ScriptableObjectGenerator<SingleAppearanceOption>
    {
        [SerializeField] private string _descriptionNameFormat = null;
        public string DescriptionNameFormat { get => _descriptionNameFormat; set => _descriptionNameFormat = value; }

        [SerializeField] private BoneKeywordData _boneName = null;
        public BoneKeywordData BoneName { get => _boneName; set => _boneName = value; }

        [SerializeField] private bool _isBone = false;
        public bool IsBone { get => _isBone; set => _isBone = value; }

        [SerializeField] private string _spriteNameFormat = null;
        public string SpriteNameFormat { get => _spriteNameFormat; set => _spriteNameFormat = value; }

        [SerializeField] private bool _isColorRanged = false;
        public bool IsColorRanged { get => _isColorRanged; set => _isColorRanged = value; }

        protected override bool TrySetObject(SingleAppearanceOption option, int index)
        {
            var sprite = RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(_isColorRanged, _spriteNameFormat, index);
            if (sprite == null) return false;

            option.DescriptionName = string.Format(_descriptionNameFormat, index);
            option.Icon = sprite.Icon.GetRepresentativeSprite();
            option.BoneNameSource = _boneName;
            option.IsBone = _isBone;
            option.Sprite = sprite;
            return true;
        }
    }
}
