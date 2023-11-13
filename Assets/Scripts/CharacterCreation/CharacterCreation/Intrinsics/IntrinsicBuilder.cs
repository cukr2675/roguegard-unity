using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class IntrinsicBuilder : IReadOnlyIntrinsic
    {
        public IIntrinsicOption Option { get; set; }
        public string OptionName { get; set; }
        public Sprite OptionIcon { get; set; }
        public bool OptionColorIsEnabled { get; set; }
        public Color OptionColor { get; set; }
        public string OptionCaption { get; set; }
        public object OptionDetails { get; set; }

        private List<IMember> members = new List<IMember>();

        public string Name => OptionName ?? Option.Name;
        public Sprite Icon => OptionIcon ?? Option.Icon;
        public Color Color => OptionColorIsEnabled ? OptionColor : Option.Color;
        public string Caption => OptionCaption ?? Option.Caption;
        public object Details => OptionDetails ?? Option.Details;

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
