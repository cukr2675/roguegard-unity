using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class ScriptableStartingItem : IReadOnlyStartingItem, IWeightedRogueObjGenerator
    {
        [SerializeField] private CharacterCreationDataAsset _option;
        public CharacterCreationDataAsset Option => _option;
        IStartingItemOption IReadOnlyStartingItem.Option => _option;

        [SerializeField] private OptionDescriptionAsset _optionDescription = null;
        private OptionDescriptionAsset OptionDescription => OptionDescriptionAsset.IdentityOr(_optionDescription);

        [SerializeField] private float _generatorWeight;
        public float GeneratorWeight
        {
            get => _generatorWeight;
            set => _generatorWeight = value;
        }

        [SerializeField] private int _stack;
        public int Stack => _stack;

        [SerializeField] private MemberList _members;

        public string Name => OptionDescription.DescriptionName ?? _option.DescriptionName;
        public Sprite Icon => OptionDescription.Icon ? OptionDescription.Icon : _option.Race.Icon;
        public Color Color => OptionDescription.ColorOfEnabled ?? _option.Race.Color;
        public string Caption => OptionDescription.Caption ?? _option.Caption;
        public IRogueDetails Details => OptionDescription.Details ?? _option.Details;

        string IReadOnlyStartingItem.OptionName => OptionDescription.DescriptionName;
        Sprite IReadOnlyStartingItem.OptionIcon => OptionDescription.Icon;
        Color? IReadOnlyStartingItem.OptionColor => OptionDescription.ColorOfEnabled;
        string IReadOnlyStartingItem.OptionCaption => OptionDescription.Caption;
        IRogueDetails IReadOnlyStartingItem.OptionDetails => OptionDescription.Details;
        IRogueGender IReadOnlyStartingItem.OptionGender => null;

        IMainInfoSet IRogueObjGenerator.InfoSet => _option.PrimaryInfoSet;
        int IRogueObjGenerator.Lv => _option.Race.Lv;
        Spanning<IWeightedRogueObjGeneratorList> IRogueObjGenerator.StartingItemTable => _option.StartingItemTable;
        float IWeightedRogueObjGenerator.Weight => _generatorWeight;
        Spanning<IMemberSource> IMemberable.MemberSources => _option.StartingItemOptionMemberSources;

        IReadOnlyMember IMemberable.GetMember(IMemberSource source)
        {
            foreach (var member in _members.Span)
            {
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
