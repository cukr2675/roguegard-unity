using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class AssetAppearance : IReadOnlyAppearance
    {
        [SerializeField] private AppearanceOptionAsset _option;
        public AppearanceOptionAsset Option => _option;
        IAppearanceOption IReadOnlyAppearance.Option => _option;

        [SerializeField] private OptionCustomAsset _optionCustom = null;
        private OptionCustomAsset OptionCustom => OptionCustomAsset.IdentityOr(_optionCustom);

        [SerializeField] private Color _color;
        public Color Color => _color;

        [SerializeField] private MemberList _members;

        public string Name => OptionCustom.DescriptionName ?? _option.DescriptionName;
        public Sprite Icon => _option.Icon;
        public string Caption => OptionCustom.Caption ?? _option.Caption;
        public IRogueDetails Details => OptionCustom.Details ?? _option.Details;

        string IReadOnlyAppearance.CustomName => OptionCustom.DescriptionName;
        string IReadOnlyAppearance.CustomCaption => OptionCustom.Caption;
        IRogueDetails IReadOnlyAppearance.CustomDetails => OptionCustom.Details;
        Spanning<IMemberSource> IReadOnlyMemberable.MemberSources => _option.MemberSources;

        IReadOnlyMember IReadOnlyMemberable.GetMember(IMemberSource source)
        {
            foreach (var member in _members.Span)
            {
                if (member.Source == source) return member;
            }
            throw new System.ArgumentException();
        }
    }
}
