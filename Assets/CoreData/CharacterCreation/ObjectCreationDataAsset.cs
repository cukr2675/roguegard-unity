using UnityEngine;

namespace Roguegard.CharacterCreation
{
    // 命名メモ: インスタンスとしての「オブジェクト」は "Obj" で、分類としての「オブジェクト」は "Object"
    // ObjCreationDataAsset にするとクラス名に Obj がつかないもの (FoodCreationDataAsset 等) が Obj ではないと連想してしまう
    // FoodObjCreationDataAsset にするのは冗長
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Character Creation Data/Object")]
    [Objforming.Referable]
    public class ObjectCreationDataAsset : RaceOptionalCreationDataAsset
    {
        [SerializeField] protected InlineRace _race = null;
        [SerializeField, DescribeElement] private AssetAppearance[] _appearances = null;
        [SerializeField, DescribeElement] private AssetIntrinsic[] _intrinsics = null;
        [SerializeField] private AssetStartingItemList[] _startingItemTable = null;

        [System.NonSerialized] private SortedIntrinsicList sortedIntrinsics;

        public override IReadOnlyRace Race => _race;
        public override Spanning<IReadOnlyAppearance> Appearances => _appearances;
        protected override ISortedIntrinsicList SortedIntrinsics => sortedIntrinsics;
        public override Spanning<IWeightedRogueObjGeneratorList> StartingItemTable => _startingItemTable;

        protected override void Initialize()
        {
            base.Initialize();
            sortedIntrinsics = new SortedIntrinsicList(_intrinsics, this);
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
    }
}
