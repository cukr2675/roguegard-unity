using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class RaceBuilder : IReadOnlyRace
    {
        public IRaceOption Option { get; set; }
        public string OptionName { get; set; }
        public Color32 BodyColor { get; set; }
        public string OptionCaption { get; set; }
        public IRogueDetails OptionDetails { get; set; }
        public IRogueGender Gender { get; set; }
        public string HPName { get; set; }
        public string MPName { get; set; }

        private readonly List<IMember> members = new List<IMember>();

        public string Name => OptionName ?? Option.Name;
        public Sprite Icon => Option.Icon;
        Color IRogueDescription.Color => Option.Color;
        public string Caption => OptionCaption ?? Option.Caption;
        public IRogueDetails Details => OptionDetails ?? Option.Details;
        Color IReadOnlyRace.BodyColor => BodyColor;
        int IReadOnlyRace.Lv => 1;
        IMemberableOption IMemberable.MemberableOption => Option;

        public RaceBuilder()
        {
        }

        public RaceBuilder(IReadOnlyRace race)
        {
            Set(race);
        }

        public void Set(IReadOnlyRace race)
        {
            Option = race.Option;
            OptionName = race.OptionName;
            BodyColor = race.BodyColor;
            OptionCaption = race.OptionCaption;
            OptionDetails = race.OptionDetails;
            Gender = race.Gender;
            HPName = race.HPName;
            MPName = race.MPName;
            members.Clear();
            for (int i = 0; i < Option.MemberSources.Count; i++)
            {
                var memberSource = Option.MemberSources[i];
                var member = race.GetMember(memberSource);
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
