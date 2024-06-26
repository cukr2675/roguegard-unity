using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class AppearanceBuilder : IReadOnlyAppearance
    {
        public IAppearanceOption Option { get; set; }
        public string OptionName { get; set; }
        public Color32 Color { get; set; }
        public string OptionCaption { get; set; }
        public IRogueDetails OptionDetails { get; set; }

        private readonly List<IMember> members = new List<IMember>();

        public string Name => Option.Name;
        public Sprite Icon => Option.Icon;
        Color IRogueDescription.Color => Color;
        public string Caption => Option.Caption;
        public IRogueDetails Details => Option.Details;
        Spanning<IMemberSource> IMemberable.MemberSources => Option.MemberSources;

        public AppearanceBuilder()
        {
        }

        public AppearanceBuilder(IReadOnlyAppearance appearance)
        {
            Set(appearance);
        }

        public void Set(IReadOnlyAppearance appearance)
        {
            Option = appearance.Option;
            OptionName = appearance.OptionName;
            Color = appearance.Color;
            OptionCaption = appearance.OptionCaption;
            OptionDetails = appearance.OptionDetails;
            members.Clear();
            for (int i = 0; i < Option.MemberSources.Count; i++)
            {
                var memberSource = Option.MemberSources[i];
                var member = appearance.GetMember(memberSource);
                members.Add(member.Clone());
            }
        }

        IReadOnlyMember IMemberable.GetMember(IMemberSource source)
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

        public AppearanceBuilder Clone()
        {
            return new AppearanceBuilder(this);
        }
    }
}
