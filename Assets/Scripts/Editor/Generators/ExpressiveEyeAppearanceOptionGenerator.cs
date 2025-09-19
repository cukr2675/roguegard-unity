using OchalikeSprites;
using Roguegard.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation.Editor
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Appearance/Generator/Expressive Eye Generator")]
    public class ExpressiveEyeAppearanceOptionGenerator : ScriptableObjectGenerator<ExpressiveEyeAppearanceOptionAsset>
    {
        [SerializeField] private string _descriptionNameFormat = null;
        public string DescriptionNameFormat { get => _descriptionNameFormat; set => _descriptionNameFormat = value; }

        [SerializeField] private BoneKeywordAsset _boneName = null;
        public BoneKeywordAsset BoneName { get => _boneName; set => _boneName = value; }

        [SerializeField] private bool _isBone = false;
        public bool IsBone { get => _isBone; set => _isBone = value; }

        [SerializeField] private string _spriteNameFormat = null;
        public string SpriteNameFormat { get => _spriteNameFormat; set => _spriteNameFormat = value; }

        [SerializeField] private bool _isColorRanged = false;
        public bool IsColorRanged { get => _isColorRanged; set => _isColorRanged = value; }

        private static readonly char[] alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        protected override bool TrySetObject(ExpressiveEyeAppearanceOptionAsset option, int index)
        {
            var neutralTable = new List<ColorRangedBoneSprite>();
            foreach (var j in alphabets)
            {
                var sprite = RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(_isColorRanged, "_Neutral", _spriteNameFormat, index, j);
                if (sprite == null) break;

                neutralTable.Add(sprite);
            }
            if (neutralTable.Count == 0) return false;

            var angryTable = new List<ColorRangedBoneSprite>();
            foreach (var j in alphabets)
            {
                var sprite = RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(_isColorRanged, "_Angry", _spriteNameFormat, index, j);
                if (sprite == null) break;

                angryTable.Add(sprite);
            }
            var cryingTable = new List<ColorRangedBoneSprite>();
            foreach (var j in alphabets)
            {
                var sprite = RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(_isColorRanged, "_Crying", _spriteNameFormat, index, j);
                if (sprite == null) break;

                cryingTable.Add(sprite);
            }
            var droopyTable = new List<ColorRangedBoneSprite>();
            foreach (var j in alphabets)
            {
                var sprite = RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(_isColorRanged, "_Droopy", _spriteNameFormat, index, j);
                if (sprite == null) break;

                droopyTable.Add(sprite);
            }
            var scornfulTable = new List<ColorRangedBoneSprite>();
            foreach (var j in alphabets)
            {
                var sprite = RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(_isColorRanged, "_Scornful", _spriteNameFormat, index, j);
                if (sprite == null) break;

                scornfulTable.Add(sprite);
            }

            option.DescriptionName = string.Format(_descriptionNameFormat, index);
            option.Icon = neutralTable[0].Icon.GetRepresentativeSprite();
            option.BoneNameSource = _boneName;
            option.IsBone = _isBone;
            option.NeutralTable = neutralTable;
            option.AngryTable = angryTable;
            option.CryingTable = cryingTable;
            option.DroopyTable = droopyTable;
            option.ScornfulTable = scornfulTable;
            return true;
        }
    }
}
