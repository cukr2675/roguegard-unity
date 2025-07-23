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

        [SerializeField] private OptionCustomAsset _optionCustom = null;
        private OptionCustomAsset OptionCustom => OptionCustomAsset.IdentityOr(_optionCustom);

        [SerializeField] private Color _bodyColor;
        public Color BodyColor => _bodyColor;

        [SerializeField] private RogueGenderAsset _gender;
        public IRogueGender Gender => _gender;

        [SerializeField] private string _hpName;
        public string HpName => _hpName;

        [SerializeField] private string _mpName;
        public string MpName => _mpName;

        [SerializeField] private MemberList _members;

        public string Name => OptionCustom.DescriptionName ?? _option.Name;
        public Sprite Icon => _option.Icon;
        Color IRogueDescribable.Color => _option.Color;
        public string Caption => OptionCustom.Caption ?? _option.Caption;
        public IRogueDetails Details => OptionCustom.Details ?? _option.Details;

        string IReadOnlyRace.CustomName => OptionCustom.DescriptionName;
        string IReadOnlyRace.CustomCaption => OptionCustom.Caption;
        IRogueDetails IReadOnlyRace.CustomDetails => OptionCustom.Details;
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
