using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Data/Preset")]
    [ObjectFormer.Referable]
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
            builder.Race = _race.ToBuilder();
            builder.Appearances.SetValues(_appearances);
            builder.Intrinsics.SetValues(_intrinsics);
            builder.StartingItems.SetValues(_startingItemTable);
            return builder;
        }

        protected override void Initialize()
        {
            base.Initialize();
            sortedIntrinsics = new SortedIntrinsicList(_intrinsics, this);
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
            for (int i = 0; i < _startingItemTable.Length; i++)
            {
                var startingItem = _startingItemTable[i];
                if (!startingItem.IsIntrinsicItem) continue;

                // 固有アイテム化したアイテムのコストはキャラのコストとなる
                // 重いアイテムほどコストも重くなる
                var itemData = startingItem.Option;
                float weight;
                try
                {
                    weight = itemData.Race.Option.GetWeight(itemData.Race.Option, itemData);
                }
                catch (System.NullReferenceException)
                {
                    // null 例外のとき RaceWeight が設定されていないと判断する
                    weight = Mathf.Clamp01(itemData.Race.Option.Cost);
                }
                cost += startingItem.Option.Cost * weight * startingItem.Stack;
                costIsUnknown |= startingItem.Option.CostIsUnknown;
            }
        }

        /// <summary>
        /// プリセットは初期アイテムをランダムにしない。
        /// </summary>
        [System.Serializable]
        private class StartingItem : ScriptableStartingItem, IWeightedRogueObjGeneratorList, IEnumerable<ScriptableStartingItem>
        {
            [System.NonSerialized] private StartingItem[] array;

            //IWeightedRogueObjGenerator IReadOnlyList<IWeightedRogueObjGenerator>.this[int index]
            //    => index == 0 ? this : throw new System.IndexOutOfRangeException();

            //int IReadOnlyCollection<IWeightedRogueObjGenerator>.Count => 1;

            float IWeightedRogueObjGeneratorList.TotalWeight => 1f;

            public Spanning<IWeightedRogueObjGenerator> Spanning
            {
                get
                {
                    array ??= new[] { this };
                    return array;
                }
            }

            IEnumerator<ScriptableStartingItem> IEnumerable<ScriptableStartingItem>.GetEnumerator()
            {
                yield return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                yield return this;
            }
        }
    }
}
