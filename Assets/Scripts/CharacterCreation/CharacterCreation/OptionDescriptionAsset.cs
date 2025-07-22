using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Option Description")]
    public class OptionDescriptionAsset : RogueDescriptionAsset
    {
        /// <summary>
        /// デフォルト: null
        /// </summary>
        public override string DescriptionName
        {
            get => string.IsNullOrWhiteSpace(DescriptionNameSource) ? null : base.DescriptionName;
            set => base.DescriptionName = value;
        }

        [SerializeField] private bool _colorIsEnabled = false;

        /// <summary>
        /// デフォルト: false
        /// </summary>
        public bool ColorIsEnabled { get => _colorIsEnabled; set => _colorIsEnabled = value; }

        public Color? ColorOfEnabled => ColorIsEnabled ? Color : null;

        private static OptionDescriptionAsset _identity;

        /// <summary>
        /// デフォルト値を持つインスタンスを取得する
        /// </summary>
        public static OptionDescriptionAsset Identity
        {
            get
            {
                if (_identity == null)
                {
                    _identity = CreateInstance<OptionDescriptionAsset>();
                }
                return _identity;
            }
        }

        public static OptionDescriptionAsset IdentityOr(OptionDescriptionAsset value)
        {
            if (value != null) return value;
            else return Identity;
        }
    }
}
