using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class StartingItem : IReadOnlyStartingItem, IWeightedRogueObjGenerator, IMemberable
    {
        public IStartingItemOption Option { get; set; }
        public string CustomName { get; set; }
        //public Sprite CustomIcon { get; set; }
        public Sprite CustomIcon { get => null; set { } }
        public Color32? OptionColor { get; set; }
        public string CustomCaption { get; set; }
        public IRogueDetails CustomDetails { get; set; }
        public float GeneratorWeight { get; set; }
        public int Stack { get; set; }

        private readonly List<IMember> members = new();

        public string Name => CustomName ?? Option.Name;
        public Sprite Icon => CustomIcon ? CustomIcon : Option.Icon;
        public Color Color => OptionColor ?? Option.Color;
        public string Caption => CustomCaption ?? Option.Caption;
        public IRogueDetails Details => CustomDetails ?? Option.Details;
        public Spanning<IKeyword> Tags => Option.Tags;

        IMainInfoSet IRogueObjGenerator.InfoSet => Option.InfoSet;
        int IRogueObjGenerator.Lv => Option.Lv;
        Spanning<IWeightedRogueObjGeneratorList> IRogueObjGenerator.StartingItemTable => Option.StartingItemTable;
        float IWeightedRogueObjGenerator.Weight => GeneratorWeight;
        Color? IReadOnlyStartingItem.CustomColor => OptionColor;
        IRogueGender IReadOnlyStartingItem.CustomGender => null;
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
            CustomName = startingItem.CustomName;
            CustomIcon = startingItem.CustomIcon;
            OptionColor = startingItem.CustomColor;
            CustomCaption = startingItem.CustomCaption;
            CustomDetails = startingItem.CustomDetails;
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
