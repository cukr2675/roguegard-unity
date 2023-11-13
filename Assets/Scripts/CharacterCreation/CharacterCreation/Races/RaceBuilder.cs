using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class RaceBuilder : IReadOnlyRace
    {
        public IRaceOption Option { get; set; }
        public string OptionName { get; set; }
        public Color BodyColor { get; set; }
        public string OptionCaption { get; set; }
        public object OptionDetails { get; set; }
        public IRogueGender Gender { get; set; }
        public string HPName { get; set; }
        public string MPName { get; set; }

        private readonly List<IMember> members = new List<IMember>();

        public string Name => OptionName ?? Option.Name;
        public Sprite Icon => Option.Icon;
        Color IRogueDescription.Color => Option.Color;
        public string Caption => OptionCaption ?? Option.Caption;
        public object Details => OptionDetails ?? Option.Details;
        int IReadOnlyRace.Lv => 1;

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
    }
}
