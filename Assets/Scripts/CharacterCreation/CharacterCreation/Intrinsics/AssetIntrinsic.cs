using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class AssetIntrinsic : IReadOnlyIntrinsic
    {
        [SerializeField] private IntrinsicOptionAsset _option;
        public IIntrinsicOption Option => _option;

        [SerializeField] private OptionCustomAsset _optionCustom = null;
        private OptionCustomAsset OptionCustom => OptionCustomAsset.IdentityOr(_optionCustom);

        [SerializeField] private MemberList _members;

        public string Name => OptionCustom.DescriptionName ?? Option.Name;
        public Sprite Icon => OptionCustom.Icon ? OptionCustom.Icon : Option.Icon;
        public Color Color => OptionCustom.ColorOfEnabled ?? Option.Color;
        public string Caption => OptionCustom.Caption ?? Option.Caption;
        public IRogueDetails Details => OptionCustom.Details ?? Option.Details;

        string IReadOnlyIntrinsic.CustomName => OptionCustom.DescriptionName;
        Sprite IReadOnlyIntrinsic.CustomIcon => OptionCustom.Icon;
        Color? IReadOnlyIntrinsic.CustomColor => OptionCustom.ColorOfEnabled;
        string IReadOnlyIntrinsic.CustomCaption => OptionCustom.Caption;
        IRogueDetails IReadOnlyIntrinsic.CustomDetails => OptionCustom.Details;
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
