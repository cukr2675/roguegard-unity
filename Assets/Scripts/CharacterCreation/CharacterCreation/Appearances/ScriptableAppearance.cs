using System.Collections;
using System.Collections.Generic;
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

        [SerializeField] private Color _color;
        public Color Color => _color;

        [SerializeField] private MemberList _members;

        public string Name => _optionDescription?.DescriptionName ?? _option.DescriptionName;
        public Sprite Icon => _option.Icon;
        public string Caption => _optionDescription?.Caption ?? _option.Caption;
        public IRogueDetails Details => _optionDescription?.Details ?? _option.Details;

        string IReadOnlyAppearance.OptionName => _optionDescription?.DescriptionName;
        string IReadOnlyAppearance.OptionCaption => _optionDescription?.Caption;
        IRogueDetails IReadOnlyAppearance.OptionDetails => _optionDescription?.Details;

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
