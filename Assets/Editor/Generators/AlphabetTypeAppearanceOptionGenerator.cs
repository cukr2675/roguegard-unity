using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;
using Roguegard.Editor;

namespace Roguegard.CharacterCreation.Editor
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Appearance/AlphabetGenerator")]
    public class AlphabetTypeAppearanceOptionGenerator : ScriptableObjectGenerator<AlphabetTypeAppearanceOption>
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

        private static readonly char[] alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        protected override bool TrySetObject(AlphabetTypeAppearanceOption option, int index)
        {
            var table = new List<ColorRangedBoneSprite>();
            foreach (var j in alphabets)
            {
                var sprite = RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(_isColorRanged, _spriteNameFormat, index, j);
                if (sprite == null) break;

                table.Add(sprite);
            }
            if (table.Count == 0) return false;

            option.DescriptionName = string.Format(_descriptionNameFormat, index);
            option.Icon = table[0].Icon.GetRepresentativeSprite();
            option.BoneNameSource = _boneName;
            option.IsBone = _isBone;
            option.Table = table;
            return true;
        }
    }
}
