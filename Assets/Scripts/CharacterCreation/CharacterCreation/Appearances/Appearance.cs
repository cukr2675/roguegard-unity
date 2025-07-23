using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class Appearance : IReadOnlyAppearance
    {
        public IAppearanceOption Option { get; set; }
        public string CustomName { get; set; }
        public Color32 Color { get; set; }
        public string CustomCaption { get; set; }
        public IRogueDetails CustomDetails { get; set; }

        private readonly List<IMember> members = new();

        public string Name => Option.Name;
        public Sprite Icon => Option.Icon;
        Color IRogueDescribable.Color => Color;
        public string Caption => Option.Caption;
        public IRogueDetails Details => Option.Details;
        Spanning<IMemberSource> IReadOnlyMemberable.MemberSources => Option.MemberSources;

        public Appearance()
        {
        }

        public Appearance(IReadOnlyAppearance appearance)
        {
            Set(appearance);
        }

        public void Set(IReadOnlyAppearance appearance)
        {
            Option = appearance.Option;
            CustomName = appearance.CustomName;
            Color = appearance.Color;
            CustomCaption = appearance.CustomCaption;
            CustomDetails = appearance.CustomDetails;
            members.Clear();
            foreach (var memberSource in Option.MemberSources)
            {
                var member = appearance.GetMember(memberSource);
                members.Add(member.Clone());
            }
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

        public Appearance Clone()
        {
            return new Appearance(this);
        }
    }
}
