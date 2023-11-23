using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Data/Preset")]
    [ObjectFormer.IgnoreRequireRelationalComponent]
    public class PresetCreationData : ScriptableCharacterCreationData
    {
        [SerializeField] private string _descriptionName = null;
        public override string DescriptionName => _descriptionName;

        [SerializeField] private string _shortName = null;
        public string ShortName => _shortName;

        [SerializeField] private string _caption = null;
        public override string Caption => _caption;

        [SerializeField] private ScriptField<IRogueDetails> _details = null;
        public override object Details => _details.Ref;

        [SerializeField] private ScriptableRace _race = null;
        [SerializeField] private ScriptableAppearance[] _appearances = null;
        [SerializeField] private ScriptableIntrinsic[] _intrinsics = null;
        [SerializeField] private StartingItem[] _startingItemTable = null;

        [System.NonSerialized] private SortedIntrinsicList sortedIntrinsics;

        public override IReadOnlyRace Race => _race;
        public override Spanning<IReadOnlyAppearance> Appearances => _appearances;
        protected override ISortedIntrinsicList SortedIntrinsics => sortedIntrinsics;
        public override Spanning<IWeightedRogueObjGeneratorList> StartingItemTable => _startingItemTable;

        public CharacterCreationDataBuilder ToBuilder()
        {
            var builder = new CharacterCreationDataBuilder();
            builder.Name = DescriptionName;
            builder.ShortName = ShortName;
            builder.Cost = Cost;
            builder.CostIsUnknown = CostIsUnknown;
            builder.Race = new RaceBuilder(_race);
            builder.Appearances.AddRange(_appearances.Select(x => new AppearanceBuilder(x)));
            builder.Intrinsics.AddRange(_intrinsics.Select(x => new IntrinsicBuilder(x)));
            builder.StartingItemTable.AddRange(_startingItemTable.Select(x => x.ToBuilder()));
            return builder;
        }

        protected override void Initialize()
        {
            base.Initialize();
            sortedIntrinsics = new SortedIntrinsicList(_intrinsics, this);
        }

        public override RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            // このインスタンスのシリアル化を避けるため、 Builder の CreateObj を使う
            return ToBuilder().CreateObj(location, position, random, stackOption);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_startingItemTable != null)
            {
                foreach (var item in _startingItemTable)
                {
                    item.GeneratorWeight = 1f;
                }
            }
        }

        protected override void GetCost(out float cost, out bool costIsUnknown)
        {
            cost = Race.Option.Cost;
            costIsUnknown = Race.Option.CostIsUnknown;

            for (int i = 0; i < _intrinsics.Length; i++)
            {
                var intrinsic = _intrinsics[i];
                cost += intrinsic.Option.GetCost(intrinsic, this, out var intrinsicCostIsUnknown);
                costIsUnknown |= intrinsicCostIsUnknown;
            }
        }

        /// <summary>
        /// プリセットは初期アイテムをランダムにしない。
        /// </summary>
        [System.Serializable]
        private class StartingItem : ScriptableStartingItem, IWeightedRogueObjGeneratorList
        {
            [System.NonSerialized] private StartingItem[] array;

            float IWeightedRogueObjGeneratorList.TotalWeight => 1f;

            public Spanning<IWeightedRogueObjGenerator> Spanning
            {
                get
                {
                    array ??= new[] { this };
                    return array;
                }
            }

            public IEnumerable<StartingItemBuilder> ToBuilder()
            {
                yield return new StartingItemBuilder(this);
            }
        }
    }
}
