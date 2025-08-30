using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class Race : IReadOnlyRace
    {
        public IRaceOption Option { get; set; }
        public string CustomName { get; set; }
        public Color32 BodyColor { get; set; }
        public string CustomCaption { get; set; }
        public IRogueDetails CustomDetails { get; set; }
        public IRogueGender Gender { get; set; }
        public string HpName { get; set; }
        public string MpName { get; set; }

        private readonly List<IMember> members = new();

        public string Name => CustomName ?? Option.Name;
        public Sprite Icon => Option.Icon;
        Color IRogueDescribable.Color => Option.Color;
        public string Caption => CustomCaption ?? Option.Caption;
        public IRogueDetails Details => CustomDetails ?? Option.Details;
        public Spanning<IKeyword> Tags => Option.Tags;
        Color IReadOnlyRace.BodyColor => BodyColor;
        int IReadOnlyRace.Lv => 1;
        Spanning<IMemberSource> IReadOnlyMemberable.MemberSources => Option.MemberSources;

        public Race()
        {
        }

        public Race(IReadOnlyRace race)
        {
            Set(race);
        }

        public void Set(IReadOnlyRace race)
        {
            Option = race.Option;
            CustomName = race.CustomName;
            BodyColor = race.BodyColor;
            CustomCaption = race.CustomCaption;
            CustomDetails = race.CustomDetails;
            Gender = race.Gender;
            HpName = race.HpName;
            MpName = race.MpName;
            members.Clear();
            foreach (var memberSource in Option.MemberSources)
            {
                var member = race.GetMember(memberSource);
                members.Add(member.Clone());
            }
        }

        IReadOnlyMember IReadOnlyMemberable.GetMember(IMemberSource source)
        {
            foreach (var member in members)
            {
                if (member.Source == source) return member;
            }
            throw new System.ArgumentException();
        }
    }
}
