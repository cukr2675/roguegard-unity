using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class ScriptableIntrinsic : IReadOnlyIntrinsic
    {
        [SerializeField] private IntrinsicOption _option;
        public IIntrinsicOption Option => _option;

        [SerializeField] private ScriptableOptionDescription _optionDescription = null;

        [SerializeField] private MemberList _members;

        public string Name => _optionDescription?.DescriptionName ?? Option.Name;
        public Sprite Icon => _optionDescription?.Icon ?? Option.Icon;
        public Color Color => (_optionDescription?.ColorIsEnabled ?? false) ? _optionDescription.Color : Option.Color;
        public string Caption => _optionDescription?.Caption ?? Option.Caption;
        public IRogueDetails Details => _optionDescription?.Details ?? Option.Details;

        string IReadOnlyIntrinsic.OptionName => _optionDescription?.DescriptionName;
        Sprite IReadOnlyIntrinsic.OptionIcon => _optionDescription?.Icon;
        bool IReadOnlyIntrinsic.OptionColorIsEnabled => _optionDescription?.ColorIsEnabled ?? false;
        Color IReadOnlyIntrinsic.OptionColor => _optionDescription?.Color ?? default;
        string IReadOnlyIntrinsic.OptionCaption => _optionDescription?.Caption;
        IRogueDetails IReadOnlyIntrinsic.OptionDetails => _optionDescription?.Details;

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
