using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Option Custom")]
    public class OptionCustomAsset : RogueDescribableAsset
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

        private static OptionCustomAsset _identity;

        /// <summary>
        /// デフォルト値を持つインスタンスを取得する
        /// </summary>
        public static OptionCustomAsset Identity
        {
            get
            {
                if (_identity == null)
                {
                    _identity = CreateInstance<OptionCustomAsset>();
                }
                return _identity;
            }
        }

        public static OptionCustomAsset IdentityOr(OptionCustomAsset value)
        {
            if (value != null) return value;
            else return Identity;
        }
    }
}
