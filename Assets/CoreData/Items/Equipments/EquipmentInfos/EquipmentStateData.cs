using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/EquipmentState")]
    public class EquipmentStateData : ScriptableObject, IEnumerable<KeyValuePair<IKeyword, int>>
    {
        [SerializeField] private List<Item> _items = null;

        [System.NonSerialized] private IKeyword[] partsList;

        public Spanning<IKeyword> Parts => partsList ??= _items.Select(x => x.Part).ToArray();

        public IEnumerator<KeyValuePair<IKeyword, int>> GetEnumerator()
        {
            foreach (var item in _items)
            {
                yield return new KeyValuePair<IKeyword, int>(item.Part, item.Length);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [System.Serializable]
        private class Item
        {
            [SerializeField] private KeywordData _part;
            public IKeyword Part => _part;

            [SerializeField] private int _length;
            public int Length => _length;
        }
    }
}
