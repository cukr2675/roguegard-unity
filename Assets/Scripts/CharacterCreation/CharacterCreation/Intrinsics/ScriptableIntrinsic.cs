using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class ScriptableIntrinsic : IReadOnlyIntrinsic
    {
        [SerializeField] private ScriptField<IIntrinsicOption> _option;
        public IIntrinsicOption Option => _option.Ref;

        [SerializeField] private ScriptableOptionDescription _optionDescription = null;

        [SerializeField] private MemberList _members;

        public string Name => _optionDescription?.DescriptionName ?? Option.Name;
        public Sprite Icon => _optionDescription?.Icon ?? Option.Icon;
        public Color Color => (_optionDescription?.ColorIsEnabled ?? false) ? _optionDescription.Color : Option.Color;
        public string Caption => _optionDescription?.Caption ?? Option.Caption;
        public object Details => _optionDescription?.Details ?? Option.Details;

        string IReadOnlyIntrinsic.OptionName => _optionDescription?.DescriptionName;
        Sprite IReadOnlyIntrinsic.OptionIcon => _optionDescription?.Icon;
        bool IReadOnlyIntrinsic.OptionColorIsEnabled => _optionDescription?.ColorIsEnabled ?? false;
        Color IReadOnlyIntrinsic.OptionColor => _optionDescription?.Color ?? default;
        string IReadOnlyIntrinsic.OptionCaption => _optionDescription?.Caption;
        object IReadOnlyIntrinsic.OptionDetails => _optionDescription?.Details;

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

        public IntrinsicBuilder ToBuilder()
        {
            var builder = new IntrinsicBuilder();
            builder.Option = Option;
            builder.OptionName = _optionDescription?.DescriptionName;
            builder.OptionIcon = _optionDescription?.Icon;
            builder.OptionColorIsEnabled = _optionDescription?.ColorIsEnabled ?? false;
            builder.OptionColor = _optionDescription?.Color ?? default;
            builder.OptionCaption = _optionDescription?.Caption;
            builder.OptionDetails = _optionDescription?.Details;
            var members = (Spanning<IMember>)_members;
            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                builder.AddMember(member.Clone());
            }
            return builder;
        }
    }
}
