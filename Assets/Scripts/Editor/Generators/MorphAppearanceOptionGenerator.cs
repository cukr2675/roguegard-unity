using OchalikeSprites;
using Roguegard.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Roguegard.CharacterCreation.Editor
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Appearance/Generator/Morph Generator")]
    public class MorphAppearanceOptionGenerator : ScriptableObjectGenerator<MorphAppearanceOptionAsset>
    {
        [SerializeField] private string _descriptionNameFormat = null;
        public string DescriptionNameFormat { get => _descriptionNameFormat; set => _descriptionNameFormat = value; }

        [SerializeField] private BoneKeywordAsset _boneName = null;
        public BoneKeywordAsset BoneName { get => _boneName; set => _boneName = value; }

        [SerializeField] private bool _isBone = false;
        public bool IsBone { get => _isBone; set => _isBone = value; }

        [SerializeField] private string _spriteNameFormat = null;
        public string SpriteNameFormat { get => _spriteNameFormat; set => _spriteNameFormat = value; }

        [SerializeField] private string _morphNameFormat = null;
        public string MorphNameFormat { get => _morphNameFormat; set => _morphNameFormat = value; }

        protected override bool TrySetObject(MorphAppearanceOptionAsset option, int index)
        {
            var sprite = RoguegardAssetDatabase.CreateColorRangedBoneSpriteOrNull(false, _spriteNameFormat, index);
            if (sprite == null) return false;

            var morphSearchName = string.Format(_morphNameFormat, index);
            var morph = AssetDatabase.FindAssets($"{morphSearchName} t:{nameof(OchalikeMorphAsset)}")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<OchalikeMorphAsset>(path))
                .FirstOrDefault();
            if (morph == null) return false;

            option.DescriptionName = string.Format(_descriptionNameFormat, index);
            option.Icon = sprite.Icon.GetRepresentativeSprite();
            option.BoneNameSource = _boneName;
            option.IsBone = _isBone;
            option.Sprite = sprite;
            option.Morph = morph;
            return true;
        }
    }
}
