using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class AssetRace : IReadOnlyRace
    {
        /// <summary>
        /// 進化によって変わることはない
        /// </summary>
        [SerializeField] private RaceOptionAsset _option;
        public RaceOptionAsset Option => _option;
        IRaceOption IReadOnlyRace.Option => Option;

        [SerializeField] private OptionDescriptionAsset _optionDescription = null;
        private OptionDescriptionAsset OptionDescription => OptionDescriptionAsset.IdentityOr(_optionDescription);

        [SerializeField] private Color _bodyColor;
        public Color BodyColor => _bodyColor;

        [SerializeField] private RogueGenderAsset _gender;
        public IRogueGender Gender => _gender;

        [SerializeField] private string _hpName;
        public string HpName => _hpName;

        [SerializeField] private string _mpName;
        public string MpName => _mpName;

        [SerializeField] private MemberList _members;

        public string Name => OptionDescription.DescriptionName ?? _option.Name;
        public Sprite Icon => _option.Icon;
        Color IRogueDescription.Color => _option.Color;
        public string Caption => OptionDescription.Caption ?? _option.Caption;
        public IRogueDetails Details => OptionDescription.Details ?? _option.Details;

        string IReadOnlyRace.OptionName => OptionDescription.DescriptionName;
        string IReadOnlyRace.OptionCaption => OptionDescription.Caption;
        IRogueDetails IReadOnlyRace.OptionDetails => OptionDescription.Details;
        int IReadOnlyRace.Lv => 1;
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
