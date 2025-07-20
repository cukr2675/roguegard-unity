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
        private ScriptableOptionDescription OptionDescription => ScriptableOptionDescription.IdentityOr(_optionDescription);

        [SerializeField] private MemberList _members;

        public string Name => OptionDescription.DescriptionName ?? Option.Name;
        public Sprite Icon => OptionDescription.Icon ? OptionDescription.Icon : Option.Icon;
        public Color Color => OptionDescription.ColorOfEnabled ?? Option.Color;
        public string Caption => OptionDescription.Caption ?? Option.Caption;
        public IRogueDetails Details => OptionDescription.Details ?? Option.Details;

        string IReadOnlyIntrinsic.OptionName => OptionDescription.DescriptionName;
        Sprite IReadOnlyIntrinsic.OptionIcon => OptionDescription.Icon;
        Color? IReadOnlyIntrinsic.OptionColor => OptionDescription.ColorOfEnabled;
        string IReadOnlyIntrinsic.OptionCaption => OptionDescription.Caption;
        IRogueDetails IReadOnlyIntrinsic.OptionDetails => OptionDescription.Details;
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
