using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class AppearanceBuilder : IReadOnlyAppearance
    {
        public IAppearanceOption Option { get; set; }
        public string OptionName { get; set; }
        public Color Color { get; set; }
        public string OptionCaption { get; set; }
        public object OptionDetails { get; set; }

        private List<IMember> members = new List<IMember>();

        public string Name => Option.Name;
        public Sprite Icon => Option.Icon;
        public string Caption => Option.Caption;
        public object Details => Option.Details;

        public void AddMember(IMember member)
        {
            members.Add(member);
        }

        IReadOnlyMember IMemberable.GetMember(IMemberSource source)
        {
            foreach (var member in members)
            {
                if (member.Source == source) return member;
            }
            throw new System.ArgumentException();
        }

        public AppearanceBuilder Clone()
        {
            var clone = new AppearanceBuilder();
            clone.Option = Option;
            clone.Color = Color;
            foreach (var member in members)
            {
                clone.members.Add(member.Clone());
            }
            return clone;
        }
    }
}
