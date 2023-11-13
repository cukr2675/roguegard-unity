using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class StartingItemBuilder : IReadOnlyStartingItem, IWeightedRogueObjGenerator
    {
        public IStartingItemOption Option { get; set; }
        public string OptionName { get; set; }
        public Sprite OptionIcon { get; set; }
        public bool OptionColorIsEnabled { get; set; }
        public Color OptionColor { get; set; }
        public string OptionCaption { get; set; }
        public object OptionDetails { get; set; }
        public float GeneratorWeight { get; set; }
        public int Stack { get; set; }
        public bool IsIntrinsicItem { get; set; }

        private readonly List<IMember> members = new List<IMember>();

        public string Name => OptionName ?? Option.Name;
        public Sprite Icon => OptionIcon ?? Option.Icon;
        public Color Color => OptionColorIsEnabled ? OptionColor : Option.Color;
        public string Caption => OptionCaption ?? Option.Caption;
        public object Details => OptionDetails ?? Option.Details;

        MainInfoSet IRogueObjGenerator.InfoSet => Option.InfoSet;
        int IRogueObjGenerator.Lv => Option.Lv;
        Spanning<IWeightedRogueObjGeneratorList> IRogueObjGenerator.StartingItemTable => Option.StartingItemTable;
        float IWeightedRogueObjGenerator.Weight => GeneratorWeight;
        IRogueGender IReadOnlyStartingItem.OptionGender => null;

        public void AddMember(IMember member)
        {
            members.Add(member);
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
            throw new System.ArgumentException();
        }
    }
}
