using UnityEngine;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// Unity エディタからアイテムや敵などを定義するための抽象クラス。
    /// </summary>
    public abstract class CharacterCreationDataAsset : ScriptableObject, ICharacterCreationData, IStartingItemOption
    {
        [SerializeField] private bool _overridesCost = false;
        public bool OverridesCost => _overridesCost;

        [SerializeField, EnabledBy(nameof(_overridesCost))] private float _cost = 0f;
        public float Cost => _cost;

        [SerializeField, EnabledBy(nameof(_overridesCost))] private bool _costIsUnknown = false;
        public bool CostIsUnknown => _costIsUnknown;

        [System.NonSerialized] private GrowingInfoSetTable _infoSets;
        private GrowingInfoSetTable InfoSets
        {
            get
            {
                TryInitialize();
                return _infoSets;
            }
        }

        /// <summary>
        /// これが true のクラスでは <see cref="IMainInfoSet"/> を生成しない。（ランダム生成テーブルなどでの使用を想定）
        /// true にするとき <see cref="Objforming.IgnoreRequireRelationalComponentAttribute"/> と併用する。
        /// </summary>
        protected virtual bool HasNotInfoSet => false;

        public virtual IReadOnlyRace Race => null;

        public virtual Spanning<IReadOnlyAppearance> Appearances => Spanning<IReadOnlyAppearance>.Empty;

        /// <summary>
        /// このプロパティへアクセスする前に <see cref="Initialize"/> が実行される
        /// </summary>
        protected virtual ISortedIntrinsicList SortedIntrinsics => emptyIntrinsics;
        private static readonly ISortedIntrinsicList emptyIntrinsics = new SortedIntrinsicList(new IReadOnlyIntrinsic[0], null);
        ISortedIntrinsicList ICharacterCreationData.SortedIntrinsics
        {
            get
            {
                if (_infoSets == null) { Initialize(); }
                return SortedIntrinsics;
            }
        }

        public virtual Spanning<IWeightedRogueObjGeneratorList> StartingItemTable => Spanning<IWeightedRogueObjGeneratorList>.Empty;

        public virtual string DescriptionName => Race?.Name ?? "";
        [System.NonSerialized] private string _name; // null にするために NonSerialized にする
        string IRogueDescribable.Name => DescriptionName.Length >= 1 ? DescriptionName : (_name ??= $":{name}");
        public virtual Sprite Icon => Race?.Icon;
        public virtual Color Color => Race.Color;
        public virtual string Caption => null;
        public virtual Spanning<IKeyword> Tags => Spanning<IKeyword>.Empty;
        public virtual IRogueDetails Details => null;
        int IStartingItemOption.Lv => Race.Lv;

        public virtual Spanning<IMemberSource> StartingItemOptionMemberSources => Spanning<IMemberSource>.Empty;
        Spanning<IMemberSource> IStartingItemOption.MemberSources => StartingItemOptionMemberSources;

        public IMainInfoSet PrimaryInfoSet
        {
            get
            {
                var primaryGender = Race.Gender ?? Race.Option.Genders[0];
                return InfoSets[Race.Option, primaryGender];
            }
        }
        IMainInfoSet IStartingItemOption.InfoSet => PrimaryInfoSet;

        bool ICharacterCreationData.TryGetGrowingInfoSet(IRaceOption raceOption, IRogueGender gender, out IMainInfoSet growingInfoSet)
        {
            if (raceOption == null) throw new System.ArgumentNullException(nameof(raceOption));
            if (gender == null) throw new System.ArgumentNullException(nameof(gender));

            var result = InfoSets.TryGetValue(raceOption, gender, out var infoSet);
            growingInfoSet = infoSet;
            return result;
        }

        void IStartingItemOption.UpdateMemberRange(IMember member, IReadOnlyStartingItem startingItem, ICharacterCreationData characterCreationData)
        {
        }

        float IStartingItemOption.GetCost(IReadOnlyStartingItem startingItem, out bool costIsUnknown)
        {
            costIsUnknown = _costIsUnknown;
            return _cost;
        }

        private IRogueGender GetRandomGender(IRogueRandom random)
        {
            var index = random.Next(0, Race.Option.Genders.Length);
            return Race.Option.Genders[index];
        }

        public virtual RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var raceOption = Race.Option;
            var gender = startingItem.CustomGender ?? Race.Gender ?? GetRandomGender(random);
            if (!InfoSets.TryGetValue(raceOption, gender, out var infoSet)) throw new System.InvalidOperationException();

            var obj = infoSet.CreateObj(location, position, random, stackOption);

            obj.TrySetStack(startingItem.Stack);
            if (startingItem.CustomColor != null) { ColoringEffect.ColorChange(obj, startingItem.CustomColor.Value); }

            return obj;
        }

        public RogueObj CreateObj(
            RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default, IRogueGender optionGender = null)
        {
            var startingItem = new DefaultItem(this, optionGender);
            return CreateObj(startingItem, location, position, random, stackOption);
        }

        protected void TryInitialize()
        {
            if (HasNotInfoSet) throw new System.InvalidOperationException($"{DescriptionName} は {nameof(IMainInfoSet)} を持ちません。");
            if (_infoSets == null) { Initialize(); }
        }

        protected virtual void Initialize()
        {
            _infoSets = new GrowingInfoSetTable(this);
        }

        protected virtual void OnValidate()
        {
            if (!_overridesCost) { GetCost(out _cost, out _costIsUnknown); }
        }

        protected abstract void GetCost(out float cost, out bool costIsUnknown);

        private class DefaultItem : IReadOnlyStartingItem
        {
            public IStartingItemOption Option { get; }
            public string CustomName => null;
            public Sprite CustomIcon => null;
            public Color? CustomColor => null;
            public string CustomCaption => null;
            public IRogueDetails CustomDetails => null;
            public float GeneratorWeight => 0f;
            public int Stack => 1;
            public IRogueGender CustomGender { get; }

            public string Name => Option.Name;
            public Sprite Icon => Option.Icon;
            public Color Color => Option.Color;
            public string Caption => Option.Caption;
            public IRogueDetails Details => Option.Details;
            public Spanning<IKeyword> Tags => Option.Tags;

            Spanning<IMemberSource> IReadOnlyMemberable.MemberSources => Option.MemberSources;

            public IReadOnlyMember GetMember(IMemberSource source) => source.CreateMember();

            public DefaultItem(IStartingItemOption option, IRogueGender optionGender)
            {
                Option = option;
                CustomGender = optionGender;
            }
        }
    }
}
