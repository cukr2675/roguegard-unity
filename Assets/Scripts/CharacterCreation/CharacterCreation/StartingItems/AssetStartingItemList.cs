using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class AssetStartingItemList : IWeightedRogueObjGeneratorList
    {
        [SerializeField] private int _minFrequency = 1;
        public int MinFrequency => _minFrequency;

        [SerializeField] private int _maxFrequency = 1;
        public int MaxFrequency => _maxFrequency;

        [SerializeField, DescribeElement] private AssetStartingItem[] _items;

        public AssetStartingItem this[int index] => _items[index];

        public float TotalWeight
        {
            get
            {
                var totalWeight = 0f;
                foreach (var item in _items)
                {
                    totalWeight += item.GeneratorWeight;
                }
                return totalWeight;
            }
        }

        public Spanning<IWeightedRogueObjGenerator> Span => _items;
    }
}
