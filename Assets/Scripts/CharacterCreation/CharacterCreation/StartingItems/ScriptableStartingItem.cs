using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class ScriptableStartingItem : IReadOnlyStartingItem, IWeightedRogueObjGenerator
    {
        [SerializeField] private ScriptableCharacterCreationData _option;
        public ScriptableCharacterCreationData Option => _option;
        IStartingItemOption IReadOnlyStartingItem.Option => _option;

        [SerializeField] private ScriptableOptionDescription _optionDescription = null;

        [SerializeField] private float _generatorWeight;
        public float GeneratorWeight
        {
            get => _generatorWeight;
            set => _generatorWeight = value;
        }

        [SerializeField] private int _stack;
        public int Stack => _stack;

        [SerializeField] private MemberList _members;

        public string Name => _optionDescription?.DescriptionName ?? _option.DescriptionName;
        public Sprite Icon => _optionDescription?.Icon ?? _option.Race.Icon;
        public Color Color => (_optionDescription?.ColorIsEnabled ?? false) ? _optionDescription.Color : _option.Race.Color;
        public string Caption => _optionDescription?.Caption ?? _option.Caption;
        public IRogueDetails Details => _optionDescription?.Details ?? _option.Details;

        string IReadOnlyStartingItem.OptionName => _optionDescription?.DescriptionName;
        Sprite IReadOnlyStartingItem.OptionIcon => _optionDescription?.Icon;
        bool IReadOnlyStartingItem.OptionColorIsEnabled => _optionDescription?.ColorIsEnabled ?? false;
        Color IReadOnlyStartingItem.OptionColor => _optionDescription?.Color ?? default;
        string IReadOnlyStartingItem.OptionCaption => _optionDescription?.Caption;
        IRogueDetails IReadOnlyStartingItem.OptionDetails => _optionDescription?.Details;
        IRogueGender IReadOnlyStartingItem.OptionGender => null;

        MainInfoSet IRogueObjGenerator.InfoSet => _option.PrimaryInfoSet;
        int IRogueObjGenerator.Lv => _option.Race.Lv;
        Spanning<IWeightedRogueObjGeneratorList> IRogueObjGenerator.StartingItemTable => _option.StartingItemTable;
        float IWeightedRogueObjGenerator.Weight => _generatorWeight;
        Spanning<IMemberSource> IMemberable.MemberSources => _option.StartingItemOptionMemberSources;

        IReadOnlyMember IMemberable.GetMember(IMemberSource source)
        {
            var members = (Spanning<IMember>)_members;
            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                if (member.Source == source) return member;
            }
            throw new System.ArgumentException();
        }

        RogueObj IRogueObjGenerator.CreateObj(RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption)
        {
            return _option.CreateObj(this, location, position, random, stackOption);
        }
    }
}
