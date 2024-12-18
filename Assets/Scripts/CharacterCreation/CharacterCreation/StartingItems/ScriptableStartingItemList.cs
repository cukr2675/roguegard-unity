using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class ScriptableStartingItemList : IWeightedRogueObjGeneratorList//, IReadOnlyList<ScriptableStartingItem>
    {
        [SerializeField, ElementDescription("_option")] private ScriptableStartingItem[] _items;

        public ScriptableStartingItem this[int index] => _items[index];

        private int Count => _items.Length;

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

        public Spanning<IWeightedRogueObjGenerator> Spanning => _items;

        int IWeightedRogueObjGeneratorList.MinFrequency => 1;
        int IWeightedRogueObjGeneratorList.MaxFrequency => 1;

        private IEnumerator<ScriptableStartingItem> GetEnumerator()
        {
            return ((IEnumerable<ScriptableStartingItem>)_items).GetEnumerator();
        }
    }
}
