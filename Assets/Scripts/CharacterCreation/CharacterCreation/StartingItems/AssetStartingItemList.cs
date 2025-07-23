using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class AssetStartingItemList : IWeightedRogueObjGeneratorList
    {
        [SerializeField, ElementDescription("_option")] private AssetStartingItem[] _items;

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

        int IWeightedRogueObjGeneratorList.MinFrequency => 1;
        int IWeightedRogueObjGeneratorList.MaxFrequency => 1;
    }
}
