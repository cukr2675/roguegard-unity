using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class IntrinsicBuilder : IReadOnlyIntrinsic
    {
        public IIntrinsicOption Option { get; set; }
        public string OptionName { get; set; }
        //public Sprite OptionIcon { get; set; }
        public Sprite OptionIcon { get => null; set { } }
        public bool OptionColorIsEnabled { get; set; }
        public Color OptionColor { get; set; }
        public string OptionCaption { get; set; }
        public IRogueDetails OptionDetails { get; set; }

        private List<IMember> members = new List<IMember>();

        public string Name => OptionName ?? Option.Name;
        public Sprite Icon => OptionIcon ?? Option.Icon;
        public Color Color => OptionColorIsEnabled ? OptionColor : Option.Color;
        public string Caption => OptionCaption ?? Option.Caption;
        public IRogueDetails Details => OptionDetails ?? Option.Details;

        public IntrinsicBuilder()
        {
        }

        public IntrinsicBuilder(IReadOnlyIntrinsic intrinsic)
        {
            Set(intrinsic);
        }

        public void Set(IReadOnlyIntrinsic intrinsic)
        {
            Option = intrinsic.Option;
            OptionName = intrinsic.OptionName;
            OptionIcon = intrinsic.OptionIcon;
            OptionColorIsEnabled = intrinsic.OptionColorIsEnabled;
            OptionColor = intrinsic.OptionColor;
            OptionCaption = intrinsic.OptionCaption;
            OptionDetails = intrinsic.OptionDetails;
            members.Clear();
            for (int i = 0; i < Option.MemberSources.Count; i++)
            {
                var memberSource = Option.MemberSources[i];
                var member = intrinsic.GetMember(memberSource);
                members.Add(member.Clone());
            }
        }

        IReadOnlyMember IMemberable.GetMember(IMemberSource source)
        {
            foreach (var member in members)
            {
                if (member.Source == source) return member;
            }
            throw new System.ArgumentException();
        }
    }
}
