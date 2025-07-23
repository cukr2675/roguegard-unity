using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class Intrinsic : IReadOnlyIntrinsic
    {
        public IIntrinsicOption Option { get; set; }
        public string OptionName { get; set; }
        //public Sprite OptionIcon { get; set; }
        public Sprite OptionIcon { get => null; set { } }
        public Color32? OptionColor { get; set; }
        public string OptionCaption { get; set; }
        public IRogueDetails OptionDetails { get; set; }

        private readonly List<IMember> members = new();

        public string Name => OptionName ?? Option.Name;
        public Sprite Icon => OptionIcon ? OptionIcon : Option.Icon;
        public Color Color => OptionColor ?? Option.Color;
        public string Caption => OptionCaption ?? Option.Caption;
        public IRogueDetails Details => OptionDetails ?? Option.Details;
        Color? IReadOnlyIntrinsic.OptionColor => OptionColor;
        Spanning<IMemberSource> IReadOnlyMemberable.MemberSources => Option.MemberSources;

        public Intrinsic()
        {
        }

        public Intrinsic(IReadOnlyIntrinsic intrinsic)
        {
            Set(intrinsic);
        }

        public void Set(IReadOnlyIntrinsic intrinsic)
        {
            Option = intrinsic.Option;
            OptionName = intrinsic.OptionName;
            OptionIcon = intrinsic.OptionIcon;
            OptionColor = intrinsic.OptionColor;
            OptionCaption = intrinsic.OptionCaption;
            OptionDetails = intrinsic.OptionDetails;
            members.Clear();
            foreach (var memberSource in Option.MemberSources)
            {
                var member = intrinsic.GetMember(memberSource);
                members.Add(member.Clone());
            }
        }

        public IMember GetMember(IMemberSource source)
        {
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].Source == source) return members[i];
            }
            var member = source.CreateMember();
            members.Add(member);
            return member;
        }

        IReadOnlyMember IReadOnlyMemberable.GetMember(IMemberSource source)
        {
            foreach (var member in members)
            {
                if (member.Source == source) return member;
            }
            {
                var member = source.CreateMember();
                members.Add(member);
                return member;
            }
        }
    }
}
