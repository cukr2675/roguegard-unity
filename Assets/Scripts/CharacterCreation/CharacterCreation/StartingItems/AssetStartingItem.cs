using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class AssetStartingItem : IReadOnlyStartingItem, IWeightedRogueObjGenerator
    {
        [SerializeField] private CharacterCreationDataAsset _option;
        public CharacterCreationDataAsset Option => _option;
        IStartingItemOption IReadOnlyStartingItem.Option => _option;

        [SerializeField] private OptionCustomAsset _optionCustom = null;
        private OptionCustomAsset OptionCustom => OptionCustomAsset.IdentityOr(_optionCustom);

        [SerializeField] private float _generatorWeight;
        public float GeneratorWeight
        {
            get => _generatorWeight;
            set => _generatorWeight = value;
        }

        [SerializeField] private int _stack;
        public int Stack => _stack;

        [SerializeField] private MemberList _members;

        public string Name => OptionCustom.DescriptionName ?? _option.DescriptionName;
        public Sprite Icon => OptionCustom.Icon ? OptionCustom.Icon : _option.Race.Icon;
        public Color Color => OptionCustom.ColorOfEnabled ?? _option.Race.Color;
        public string Caption => OptionCustom.Caption ?? _option.Caption;
        public IRogueDetails Details => OptionCustom.Details ?? _option.Details;

        string IReadOnlyStartingItem.CustomName => OptionCustom.DescriptionName;
        Sprite IReadOnlyStartingItem.CustomIcon => OptionCustom.Icon;
        Color? IReadOnlyStartingItem.CustomColor => OptionCustom.ColorOfEnabled;
        string IReadOnlyStartingItem.CustomCaption => OptionCustom.Caption;
        IRogueDetails IReadOnlyStartingItem.CustomDetails => OptionCustom.Details;
        IRogueGender IReadOnlyStartingItem.CustomGender => null;

        IMainInfoSet IRogueObjGenerator.InfoSet => _option.PrimaryInfoSet;
        int IRogueObjGenerator.Lv => _option.Race.Lv;
        Spanning<IWeightedRogueObjGeneratorList> IRogueObjGenerator.StartingItemTable => _option.StartingItemTable;
        float IWeightedRogueObjGenerator.Weight => _generatorWeight;
        Spanning<IMemberSource> IReadOnlyMemberable.MemberSources => _option.StartingItemOptionMemberSources;

        IReadOnlyMember IReadOnlyMemberable.GetMember(IMemberSource source)
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
