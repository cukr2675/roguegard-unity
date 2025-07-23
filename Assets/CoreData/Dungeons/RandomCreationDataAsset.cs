using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Character Creation Data/Random")]
    [Objforming.IgnoreRequireRelationalComponent]
    public class RandomCreationDataAsset : CharacterCreationDataAsset
    {
        [SerializeField] private AssetStartingItemList _items = null;

        protected override bool HasNotInfoSet => true;

        // Intrinsic や Stack を無効にする
        public override RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var obj = WeightedRogueObjGeneratorUtility.CreateObj(_items, location, position, random, stackOption);
            return obj;
        }

        protected override void GetCost(out float cost, out bool costIsUnknown)
        {
            cost = 0f;
            costIsUnknown = true;
        }
    }
}
