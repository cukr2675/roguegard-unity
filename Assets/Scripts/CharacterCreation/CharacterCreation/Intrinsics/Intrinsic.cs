using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class Intrinsic : IReadOnlyIntrinsic
    {
        public IIntrinsicOption Option { get; set; }
        public string CustomName { get; set; }
        //public Sprite CustomIcon { get; set; }
        public Sprite CustomIcon { get => null; set { } }
        public Color32? OptionColor { get; set; }
        public string CustomCaption { get; set; }
        public IRogueDetails CustomDetails { get; set; }

        private readonly List<IMember> members = new();

        public string Name => CustomName ?? Option.Name;
        public Sprite Icon => CustomIcon ? CustomIcon : Option.Icon;
        public Color Color => OptionColor ?? Option.Color;
        public string Caption => CustomCaption ?? Option.Caption;
        public IRogueDetails Details => CustomDetails ?? Option.Details;
        public Spanning<IKeyword> Tags => Option.Tags;
        Color? IReadOnlyIntrinsic.CustomColor => OptionColor;
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
            CustomName = intrinsic.CustomName;
            CustomIcon = intrinsic.CustomIcon;
            OptionColor = intrinsic.CustomColor;
            CustomCaption = intrinsic.CustomCaption;
            CustomDetails = intrinsic.CustomDetails;
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
