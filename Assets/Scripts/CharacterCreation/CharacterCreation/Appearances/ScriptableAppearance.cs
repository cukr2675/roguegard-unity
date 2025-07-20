using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class ScriptableAppearance : IReadOnlyAppearance
    {
        [SerializeField] private AppearanceOption _option;
        public AppearanceOption Option => _option;
        IAppearanceOption IReadOnlyAppearance.Option => _option;

        [SerializeField] private ScriptableOptionDescription _optionDescription = null;
        private ScriptableOptionDescription OptionDescription => ScriptableOptionDescription.IdentityOr(_optionDescription);

        [SerializeField] private Color _color;
        public Color Color => _color;

        [SerializeField] private MemberList _members;

        public string Name => OptionDescription.DescriptionName ?? _option.DescriptionName;
        public Sprite Icon => _option.Icon;
        public string Caption => OptionDescription.Caption ?? _option.Caption;
        public IRogueDetails Details => OptionDescription.Details ?? _option.Details;

        string IReadOnlyAppearance.OptionName => OptionDescription.DescriptionName;
        string IReadOnlyAppearance.OptionCaption => OptionDescription.Caption;
        IRogueDetails IReadOnlyAppearance.OptionDetails => OptionDescription.Details;
        Spanning<IMemberSource> IMemberable.MemberSources => _option.MemberSources;

        IReadOnlyMember IMemberable.GetMember(IMemberSource source)
        {
            foreach (var member in _members.Span)
            {
                if (member.Source == source) return member;
            }
            throw new System.ArgumentException();
        }
    }
}
