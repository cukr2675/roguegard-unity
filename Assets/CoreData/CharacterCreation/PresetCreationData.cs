using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Data/Preset")]
    [Objforming.IgnoreRequireRelationalComponent]
    public class PresetCreationData : ScriptableCharacterCreationData
    {
        [SerializeField] private string _descriptionName = null;
        public override string DescriptionName => _descriptionName;

        [SerializeField] private string _shortName = null;
        public string ShortName => _shortName;

        [SerializeField] private string _caption = null;
        public override string Caption => _caption;

        [SerializeField] private ScriptField<IRogueDetails> _details = null;
        public override IRogueDetails Details => _details.Ref;

        [SerializeField] private ScriptableRace _race = null;
        [SerializeField, ElementDescription("_option")] private ScriptableAppearance[] _appearances = null;
        [SerializeField, ElementDescription("_option")] private ScriptableIntrinsic[] _intrinsics = null;
        [SerializeField] private StartingItem[] _startingItemTable = null;

        [System.NonSerialized] private SortedIntrinsicList sortedIntrinsics;

        public override IReadOnlyRace Race => _race;
        public override Spanning<IReadOnlyAppearance> Appearances => _appearances;
        protected override ISortedIntrinsicList SortedIntrinsics => sortedIntrinsics;
        public override Spanning<IWeightedRogueObjGeneratorList> StartingItemTable => _startingItemTable;
        protected override bool HasNotInfoSet => true;

        public CharacterCreationDataBuilder ToBuilder()
        {
            var builder = new CharacterCreationDataBuilder();
            builder.Name = DescriptionName;
            builder.ShortName = ShortName;
            builder.Details = Details;
            builder.Cost = Cost;
            builder.CostIsUnknown = CostIsUnknown;
            builder.Race.Set(_race);
            builder.Appearances.AddClones(_appearances);
            builder.Intrinsics.AddClones(_intrinsics);
            builder.StartingItemTable.AddClones(_startingItemTable);
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
                cost += Mathf.Max(intrinsic.Option.GetCost(intrinsic, this, out var intrinsicCostIsUnknown), 0f);
                costIsUnknown |= intrinsicCostIsUnknown;
            }
        }

        /// <summary>
        /// プリセットは初期アイテムをランダムにしない。
        /// </summary>
        [System.Serializable]
        private class StartingItem : ScriptableStartingItem, IWeightedRogueObjGeneratorList, IEnumerable<IReadOnlyStartingItem>
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

            int IWeightedRogueObjGeneratorList.MinFrequency => 1;
            int IWeightedRogueObjGeneratorList.MaxFrequency => 1;

            public IEnumerator<IReadOnlyStartingItem> GetEnumerator()
            {
                yield return this;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
