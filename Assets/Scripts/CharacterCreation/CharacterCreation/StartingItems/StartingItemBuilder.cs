using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class StartingItemBuilder : IReadOnlyStartingItem, IWeightedRogueObjGenerator, IMemberableBuilder
    {
        public IStartingItemOption Option { get; set; }
        public string OptionName { get; set; }
        //public Sprite OptionIcon { get; set; }
        public Sprite OptionIcon { get => null; set { } }
        public bool OptionColorIsEnabled { get; set; }
        public Color32 OptionColor { get; set; }
        public string OptionCaption { get; set; }
        public IRogueDetails OptionDetails { get; set; }
        public float GeneratorWeight { get; set; }
        public int Stack { get; set; }

        private readonly List<IMember> members = new List<IMember>();

        public string Name => OptionName ?? Option.Name;
        public Sprite Icon => OptionIcon ?? Option.Icon;
        public Color Color => OptionColorIsEnabled ? OptionColor : Option.Color;
        public string Caption => OptionCaption ?? Option.Caption;
        public IRogueDetails Details => OptionDetails ?? Option.Details;

        MainInfoSet IRogueObjGenerator.InfoSet => Option.InfoSet;
        int IRogueObjGenerator.Lv => Option.Lv;
        Spanning<IWeightedRogueObjGeneratorList> IRogueObjGenerator.StartingItemTable => Option.StartingItemTable;
        float IWeightedRogueObjGenerator.Weight => GeneratorWeight;
        Color IReadOnlyStartingItem.OptionColor => OptionColor;
        IRogueGender IReadOnlyStartingItem.OptionGender => null;
        IMemberableOption IMemberable.MemberableOption => Option;

        public StartingItemBuilder()
        {
        }

        public StartingItemBuilder(IReadOnlyStartingItem startingItem)
        {
            Set(startingItem);
        }

        public void Set(IReadOnlyStartingItem startingItem)
        {
            Option = startingItem.Option;
            OptionName = startingItem.OptionName;
            OptionIcon = startingItem.OptionIcon;
            OptionColorIsEnabled = startingItem.OptionColorIsEnabled;
            OptionColor = startingItem.OptionColor;
            OptionCaption = startingItem.OptionCaption;
            OptionDetails = startingItem.OptionDetails;
            GeneratorWeight = startingItem.GeneratorWeight;
            Stack = startingItem.Stack;
            members.Clear();
            for (int i = 0; i < Option.MemberSources.Count; i++)
            {
                var memberSource = Option.MemberSources[i];
                var member = startingItem.GetMember(memberSource);
                members.Add(member.Clone());
            }
        }

        RogueObj IRogueObjGenerator.CreateObj(RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption)
        {
            return Option.CreateObj(this, location, position, random, stackOption);
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
    }
}
