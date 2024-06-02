using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class ScriptableRace : IReadOnlyRace
    {
        /// <summary>
        /// 進化によって変わることはない
        /// </summary>
        [SerializeField] private RaceOption _option;
        public RaceOption Option => _option;
        IRaceOption IReadOnlyRace.Option => Option;

        [SerializeField] private ScriptableOptionDescription _optionDescription = null;

        [SerializeField] private Color _bodyColor;
        public Color BodyColor => _bodyColor;

        [SerializeField] private RogueGender _gender;
        public IRogueGender Gender => _gender;

        [SerializeField] private string _hpName;
        public string HPName => _hpName;

        [SerializeField] private string _mpName;
        public string MPName => _mpName;

        [SerializeField] private MemberList _members;

        public string Name => _optionDescription?.DescriptionName ?? _option.Name;
        public Sprite Icon => _option.Icon;
        Color IRogueDescription.Color => _option.Color;
        public string Caption => _optionDescription?.Caption ?? _option.Caption;
        public IRogueDetails Details => _optionDescription?.Details ?? _option.Details;

        string IReadOnlyRace.OptionName => _optionDescription?.DescriptionName;
        string IReadOnlyRace.OptionCaption => _optionDescription?.Caption;
        IRogueDetails IReadOnlyRace.OptionDetails => _optionDescription?.Details;
        int IReadOnlyRace.Lv => 1;
        Spanning<IMemberSource> IMemberable.MemberSources => _option.MemberSources;

        IReadOnlyMember IMemberable.GetMember(IMemberSource source)
        {
            var members = (Spanning<IMember>)_members;
            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                if (member.Source == source) return member;
            }
            throw new System.ArgumentException();
        }
    }
}
