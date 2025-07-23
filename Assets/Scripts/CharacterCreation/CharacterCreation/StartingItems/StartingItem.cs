using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class StartingItem : IReadOnlyStartingItem, IWeightedRogueObjGenerator, IMemberable
    {
        public IStartingItemOption Option { get; set; }
        public string OptionName { get; set; }
        //public Sprite OptionIcon { get; set; }
        public Sprite OptionIcon { get => null; set { } }
        public Color32? OptionColor { get; set; }
        public string OptionCaption { get; set; }
        public IRogueDetails OptionDetails { get; set; }
        public float GeneratorWeight { get; set; }
        public int Stack { get; set; }

        private readonly List<IMember> members = new();

        public string Name => OptionName ?? Option.Name;
        public Sprite Icon => OptionIcon ? OptionIcon : Option.Icon;
        public Color Color => OptionColor ?? Option.Color;
        public string Caption => OptionCaption ?? Option.Caption;
        public IRogueDetails Details => OptionDetails ?? Option.Details;

        IMainInfoSet IRogueObjGenerator.InfoSet => Option.InfoSet;
        int IRogueObjGenerator.Lv => Option.Lv;
        Spanning<IWeightedRogueObjGeneratorList> IRogueObjGenerator.StartingItemTable => Option.StartingItemTable;
        float IWeightedRogueObjGenerator.Weight => GeneratorWeight;
        Color? IReadOnlyStartingItem.OptionColor => OptionColor;
        IRogueGender IReadOnlyStartingItem.OptionGender => null;
        Spanning<IMemberSource> IReadOnlyMemberable.MemberSources => Option.MemberSources;

        public StartingItem()
        {
        }

        public StartingItem(IReadOnlyStartingItem startingItem)
        {
            Set(startingItem);
        }

        public void Set(IReadOnlyStartingItem startingItem)
        {
            Option = startingItem.Option;
            OptionName = startingItem.OptionName;
            OptionIcon = startingItem.OptionIcon;
            OptionColor = startingItem.OptionColor;
            OptionCaption = startingItem.OptionCaption;
            OptionDetails = startingItem.OptionDetails;
            GeneratorWeight = startingItem.GeneratorWeight;
            Stack = startingItem.Stack;
            members.Clear();
            foreach (var memberSource in Option.MemberSources)
            {
                var member = startingItem.GetMember(memberSource);
                members.Add(member.Clone());
            }
        }

        RogueObj IRogueObjGenerator.CreateObj(RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption)
        {
            return Option.CreateObj(this, location, position, random, stackOption);
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
